using NLog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TenBis.Interfaces;

namespace TenBis.Classes.Notifiers
{
    internal class TelegramCommunication : ICommunication
    {
        private readonly long _chatId;
        private readonly IAggregate _aggregate;
        public readonly ITelegramBotClient _botClient;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public TelegramCommunication(string? token, long? chatId, IAggregate aggregate)
        {
            if (string.IsNullOrEmpty(token) || !chatId.HasValue || aggregate is null)
            {
                throw new Exception();
            }

            _chatId = chatId.Value;
            _aggregate = aggregate;
            _botClient = new TelegramBotClient(token);
        }

        private void NotifyContact(string message)
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting notifying contact");
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            _botClient.SendTextMessageAsync(_chatId, message);
            _logger.Info($"{Helper.GetCurrentMethod()}: Finished notifying contact");
        }

        public void AlertContactAboutScript()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting alerting contact about the script");

            InlineKeyboardMarkup reply = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Yes", bool.TrueString),
                    InlineKeyboardButton.WithCallbackData("No", bool.FalseString)
                }
            });

            const string QUESTION = "Would you like to aggregate your money to points?";
            _botClient.SendTextMessageAsync(_chatId, QUESTION, replyMarkup: reply);
            _logger.Info($"{Helper.GetCurrentMethod()}: Finished alerting contact about the script");
        }

        private async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Started getting response from the contact");
            if (update.Type is not UpdateType.CallbackQuery || update.CallbackQuery?.Data is null)
            {
                return;
            }

            bool runScript = bool.Parse(update.CallbackQuery.Data);
            NotifyContactAboutDecision(runScript);
            if (runScript)
            {
                string? message = _aggregate?.Aggregate();
                NotifyContact(message!);
            }

            _logger.Info($"{Helper.GetCurrentMethod()}: Finished getting response from the contact");
            Environment.Exit(1);
        }

        private void NotifyContactAboutDecision(bool runScript)
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting notifying contact about his decision");
            string message = runScript ? "would run immediately" : "wouldn't run";
            message = $"The script {message}";
            NotifyContact(message);
            _logger.Info($"{Helper.GetCurrentMethod()}: Finished notifying contact about his decision");
        }

        public void ValidateRunningScript()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting listening to contact response");
            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[]
    {
                    UpdateType.CallbackQuery
    }
            };

            _botClient.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions);
        }
    }
}