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
        private bool _isAggregatingProcessStarted;
        private readonly long m_ChatId;
        private bool? _isScriptNeededToRun;
        private readonly IAggregator _aggregate;
        private readonly ITelegramBotClient m_TelegramBotClient;
        private readonly DateTime _validUntilTime;
        private static readonly object _lockObject = new object();
        private Timer? _timer;
        private TelegramSettings m_TelegramSettings;

        public TelegramCommunicator(string? token, long? chatId, IAggregator aggregate)
        {
            if (string.IsNullOrEmpty(token) || !chatId.HasValue || aggregate is null)
            {
                throw new Exception();
            }

            _timer = null;
            _isAggregatingProcessStarted = false;
            _validUntilTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 30, 0);
            m_ChatId = chatId.Value;
            _aggregate = aggregate;
            m_TelegramBotClient = new TelegramBotClient(token);
        }

        public TelegramCommunicator(TelegramSettings telegramSettings)
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

            Logger.FunctionFinished();
            m_ChatId = telegramSettings.ChatId.Value;
            m_TelegramBotClient = new TelegramBotClient(telegramSettings!.BotToken);
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

        Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

        async Task<Result> ICommunicator.SendValidationMessage()
        {
            Logger.FunctionStarted();

            CancellationTokenSource? cancellationTokenSource = null;
            try
            {
                _userResponseTcs = new TaskCompletionSource<bool>();

                InlineKeyboardMarkup replyMarkup = new(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Yes", bool.TrueString),
                        InlineKeyboardButton.WithCallbackData("No", bool.FalseString)
                    }
                });

                // Flush old updates before starting receiving
                try
                {
                    var updates = await m_TelegramBotClient.GetUpdates();
                    if (updates.Any())
                    {
                        var latestUpdateId = updates.Max(u => u.Id);
                        // This will clear all updates up to the latest one
                        await m_TelegramBotClient.GetUpdates(offset: latestUpdateId + 1);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Warn("Failed to flush old updates: " + ex.Message);
                }

                cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromHours(4));
                CancellationToken cancellationToken = cancellationTokenSource.Token;

                Logger.Info("Sending validation message to user...");
                await m_TelegramBotClient.SendMessage(
                    chatId: m_ChatId,
                    text: "Would you like to aggregate your money to points?",
                    replyMarkup: replyMarkup,
                    cancellationToken: cancellationToken
                );

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

                Task timeoutTask = Task.Delay(TimeSpan.FromSeconds(4), cancellationToken);
                Task completedTask = await Task.WhenAny(_userResponseTcs.Task, timeoutTask);

                cancellationTokenSource.Cancel();

                if (completedTask == _userResponseTcs.Task)
                {
                    bool userChoice = await _userResponseTcs.Task;
                    Logger.Info($"User responded with: {(userChoice ? "Yes" : "No")}");
                    Logger.FunctionFinished();
                    return Result.Ok().WithSuccess($"User chose {(userChoice ? "Yes" : "No")}");
                }
                else
                {
                    Logger.Warn("User did not respond within the 4-hour timeout period");
                    Logger.FunctionFinished();
                    return Result.Fail("No response received within timeout.");
                }
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