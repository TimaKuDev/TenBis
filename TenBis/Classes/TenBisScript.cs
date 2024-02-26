using Telegram.Bot.Types.Enums;
using TenBis.Classes.Aggregation;
using TenBis.Factories;
using TenBis.Interfaces;
using TenBis.SettingsFolder.Models;
using TenBis.SettingsFolder;
using NLog;

namespace TenBis.Classes
{
    internal class TenBisScript
    {
        private static Timer _timer;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        static TenBisScript()
        {

            _timer = new Timer(ValidateRunningScriptFunction, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        public static int RunScript()
        {
            string? message = null;
            ICommunication communcation = null;
            try
            {
                BrowserSettingsModel? browserSettingsModel = SettingsHelper.GetBrowserSettins();
                CommunicationSettingsModel? notifySettingsModel = SettingsHelper.GetNotifySettins();
                communcation = NotifierFactory.CreateNotifier(notifySettingsModel);
                communcation.ValidateWithUserMessage();
                bool? runScript = communcation?.ValidateRunningScript();
                if (!runScript.HasValue || !runScript.Value)
                {
                    return 1;
                }

                return SeleniumTenBisAggregation.Aggrgate(browserSettingsModel, out message);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
                return 1;
            }
            finally
            {
                communcation?.Notify(message);
            }
        }



        private static void ValidateRunningScriptFunction(object? state)
        {

        }
    }
}
