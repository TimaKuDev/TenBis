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
        private readonly IAggrgate _aggrgate;
        public readonly ITelegramBotClient _botClient;
        private static bool runScript;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public TelegramCommunication(string token, long chatId, IAggrgate aggrgate)
        {
            _chatId = chatId;
            _aggrgate = aggrgate;
            _botClient = new TelegramBotClient(token);
        }

        public void NotifyContact(string message)
        {
            _botClient.SendTextMessageAsync(_chatId, message);
        }

        public void AlertContactAboutScript()
        {
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
        }

        private async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            if (update.Type is not UpdateType.CallbackQuery || update.CallbackQuery?.Data is null)
            {
                return;
            }

            NotifyContactAboutDecision(update.CallbackQuery.Data);
            string? message = null;
            bool? x = _aggrgate?.TryAggrgate(out message);
            if (!x.HasValue || !x.Value)
            {
                NotifyContactAboutFailure(message);
            }

            NotifyContactAboutSuccess(message);
        }

        private void NotifyContactAboutSuccess(string? message)
        {
            NotifyContact(message);
        }

        private void NotifyContactAboutFailure(string? message)
        {
            message = string.IsNullOrEmpty(message) ? "The script failed to aggregate money to points" : message;
            NotifyContact(message);
            Environment.Exit(1);
        }

        private void NotifyContactAboutDecision(string userSelectionMessage)
        {
            runScript = bool.Parse(userSelectionMessage);
            string message = runScript ? "would run immediately" : "wouldn't run";
            message = $"The script {message}";
            NotifyContact(message);
            if (!runScript)
            {
                Environment.Exit(1);
            }
        }

        public void ValidateRunningScript()
        {
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