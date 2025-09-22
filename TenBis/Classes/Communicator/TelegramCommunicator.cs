using FluentResults;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TenBis.Interfaces;
using TenBis.Logging;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Notifiers
{
    internal class TelegramCommunicator : ICommunicator
    {
        private readonly long m_ChatId;
        private readonly ITelegramBotClient m_TelegramBotClient;
        private readonly ValidationMessageConfig m_ValidationMessageConfig;

        public TelegramCommunicator(TelegramSettings ?telegramSettings, ValidationMessageConfig? validationMessageConfig)
        {
            Logger.FunctionStarted();

            if (telegramSettings is null)
            {
                throw new ArgumentNullException(nameof(telegramSettings));
            }

            if (!telegramSettings.ChatId.HasValue)
            {
                throw new ArgumentNullException(nameof(telegramSettings));
            }

            if (string.IsNullOrWhiteSpace(telegramSettings.BotToken))
            {
                throw new ArgumentNullException(nameof(telegramSettings));
            }

            if (validationMessageConfig is null)
            {
                throw new ArgumentNullException(nameof(validationMessageConfig));
            }

            if (validationMessageConfig.ResendIntervalMinutes is null)
            {
                throw new ArgumentNullException(nameof(validationMessageConfig));
            }

            if (validationMessageConfig.ResponseTimeoutMinutes is null)
            {
                throw new ArgumentNullException(nameof(validationMessageConfig));
            }

            m_ChatId = telegramSettings.ChatId.Value;
            m_ValidationMessageConfig = validationMessageConfig;
            m_TelegramBotClient = new TelegramBotClient(telegramSettings!.BotToken);

            Logger.FunctionFinished();
        }


        private TaskCompletionSource<bool>? _userResponseTcs;


        private async Task ReceiveUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.CallbackQuery &&
                update.CallbackQuery?.Message?.Chat.Id == m_ChatId &&
                bool.TryParse(update.CallbackQuery.Data, out bool response))
            {
                _userResponseTcs?.TrySetResult(response);

                await botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: cancellationToken);
                await botClient.SendMessage(m_ChatId, $"You selected: {(response ? "Yes" : "No")}", cancellationToken: cancellationToken);
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }

        async Task<Result> ICommunicator.SendMessage(string message)
        {
            Logger.FunctionStarted();

            try
            {
                await m_TelegramBotClient.SendMessage(m_ChatId, message);

                Logger.FunctionFinished();
                return Result.Ok();
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                return Result.Fail("Failed to send message via Telegram.");
            }
        }

        async Task<Result<bool>> ICommunicator.SendValidationMessage()
        {
            Logger.FunctionStarted();
            CancellationTokenSource? cancellationTokenSource = null;

            try
            {
                try
                {
                    Update[] updates = await m_TelegramBotClient.GetUpdates();
                    if (updates.Any())
                    {
                        int latestUpdateId = updates.Max(u => u.Id);
                        await m_TelegramBotClient.GetUpdates(offset: latestUpdateId + 1);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("Failed to flush old updates: " + ex.Message);
                    return Result.Fail("Failed to flush old updates: " + ex.Message);
                }

                _userResponseTcs = new TaskCompletionSource<bool>();

                InlineKeyboardMarkup replyMarkup = new(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Yes", bool.TrueString),
                        InlineKeyboardButton.WithCallbackData("No", bool.FalseString)
                    }
                });

                cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(m_ValidationMessageConfig!.ResponseTimeoutMinutes!.Value));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                Logger.Info("Starting to receive updates from Telegram...");
                m_TelegramBotClient.StartReceiving(
                    updateHandler: (client, update, token) => ReceiveUpdateAsync(client, update, token),
                    errorHandler: (client, exception, token) => HandleErrorAsync(client, exception, token),
                    receiverOptions: new ReceiverOptions
                    {
                        AllowedUpdates = new[] { UpdateType.CallbackQuery }
                    },
                    cancellationToken: cancellationToken
                );

                while (!cancellationToken.IsCancellationRequested)
                {
                    Logger.Info("Sending validation message to user...");
                    await m_TelegramBotClient.SendMessage(
                        chatId: m_ChatId,
                        text: "Would you like to aggregate your money to points?",
                        replyMarkup: replyMarkup,
                        cancellationToken: cancellationToken
                    );

                    Task timeoutTask = Task.Delay(TimeSpan.FromMinutes(m_ValidationMessageConfig!.ResendIntervalMinutes!.Value), cancellationToken);
                    Task completedTask = await Task.WhenAny(_userResponseTcs.Task, timeoutTask);

                    if (completedTask == _userResponseTcs.Task)
                    {
                        bool userChoice = await _userResponseTcs.Task;
                        Logger.Info($"User responded with: {(userChoice ? "Yes" : "No")}");
                        Logger.FunctionFinished();
                        return Result.Ok(userChoice);
                    }

                    Logger.Info($"No response received, will retry in {m_ValidationMessageConfig!.ResendIntervalMinutes!.Value} minutes if within timeout period");
                }

                Logger.Warn("User did not respond within the 4-hour timeout period");
                Logger.FunctionFinished();
                return Result.Fail("No response received within timeout.");
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to send or receive validation message");
                return Result.Fail($"Failed to process validation message: {ex.Message}");
            }
            finally
            {
                _userResponseTcs = null;
                cancellationTokenSource?.Dispose();
            }
        }
    }
}