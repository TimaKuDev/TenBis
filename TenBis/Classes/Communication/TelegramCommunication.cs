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
        private readonly IAggregate _aggrgate;
        public readonly ITelegramBotClient _botClient;
        private static bool runScript;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public TelegramCommunication(string token, long chatId, IAggregate aggrgate)
        {
            _chatId = chatId;
            _aggrgate = aggrgate;
            _botClient = new TelegramBotClient(token);
        }

        private void NotifyContact(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

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
            string? message = _aggrgate?.Aggregate();
            NotifyContact(message!);
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