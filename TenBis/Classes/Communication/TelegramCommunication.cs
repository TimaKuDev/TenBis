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
        public readonly ITelegramBotClient _botClient;
        private bool? runScript;
        public TelegramCommunication(string token, long chatId)
        {
            _chatId = chatId;
            _botClient = new TelegramBotClient(token) { Timeout = TimeSpan.FromSeconds(10) };
        }

        public void Notify(string message)
        {
            _botClient.SendTextMessageAsync(_chatId, message);
        }

        public bool? ValidateRunningScript()
        {
            ReceiverOptions receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new UpdateType[]
                {
                    UpdateType.CallbackQuery
                }
            };

            InlineKeyboardMarkup reply = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Yes", "True"),
                    InlineKeyboardButton.WithCallbackData("No", "False")
                }
            });

            string question = "Would you like to aggregate your money?";
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions);
            _botClient.SendTextMessageAsync(_chatId, question, replyMarkup: reply);

            return runScript;
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

            string userSelectionMessage = update.CallbackQuery.Data;
            runScript = bool.Parse(userSelectionMessage);
            if (!runScript.HasValue)
            {
                return;
            }

            string message = $"would run at {DateTime.Now}";
            if (!runScript.Value)
            {
                message = "wouldn't run";
            }


            string output = $"The script {message}";
            await (_ = _botClient.SendTextMessageAsync(_chatId, output, cancellationToken: token));
        }
    }
}