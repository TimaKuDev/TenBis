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
        private bool _isAggregatingProcessStarted;
        private readonly long _chatId;
        private bool? _isScriptNeededToRun;
        private readonly IAggregate _aggregate;
        private readonly ITelegramBotClient _botClient;
        private readonly DateTime _validUntilTime;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private static readonly object _lockObject = new object();
        private Timer? _timer;

        public TelegramCommunication(string? token, long? chatId, IAggregate aggregate)
        {
            if (string.IsNullOrEmpty(token) || !chatId.HasValue || aggregate is null)
            {
                throw new Exception();
            }

            _timer = null;
            _isAggregatingProcessStarted = false;
            _validUntilTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 18, 30, 0);
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

        private void ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private void UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Started getting response from the contact");
            if (update.Type is not UpdateType.CallbackQuery || update.CallbackQuery?.Data is null)
            {
                return;
            }

            lock (_lockObject)
            {
                _isScriptNeededToRun = bool.Parse(update.CallbackQuery.Data);
            }

            _logger.Info($"{Helper.GetCurrentMethod()}: Finished getting response from the contact");
        }

        private void NotifyContactAboutDecision()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting notifying contact about his decision");
            string message;
            if (!_isScriptNeededToRun.HasValue)
            {
                message = "Since you didn’t make a selection, the default option will be applied, and the script will be closed immediately.";
            }
            else if (!_isScriptNeededToRun.Value)
            {
                message = "Since you’ve chosen not to aggregate your money, the script will be closed immediately.";
            }
            else
            {
                message = "Since you’ve chosen to aggregate your money, the script will run immediately.";
            }

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
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(10);
            _timer = new Timer((_) => CheckIfNeedToCloseScript(), null, startTimeSpan, periodTimeSpan);
        }

        private void CheckIfNeedToCloseScript()
        {
            lock (_lockObject)
            {

                if (DateTime.Now > _validUntilTime || (_isScriptNeededToRun.HasValue && !_isScriptNeededToRun.Value))
                {
                    NotifyContactAboutDecision();
                    Environment.Exit(1);
                }

                if (!_isScriptNeededToRun.HasValue)
                {
                    return;
                }

                if (_isAggregatingProcessStarted)
                {
                    return;
                }

                NotifyContactAboutDecision();
                if (_isScriptNeededToRun.Value)
                {
                    _isAggregatingProcessStarted = true;
                    _timer?.Change(Timeout.Infinite, Timeout.Infinite);
                    string? message = _aggregate?.Aggregate();
                    NotifyContact(message!);
                    Environment.Exit(1);
                }
            }
        }
    }
}