using NLog;
using TenBis.Classes.Aggregation;
using TenBis.Factories;
using TenBis.Interfaces;
using TenBis.SettingsFolder;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes
{
    internal class TenBisScript
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public static void RunScript()
        {
            try
            {
                BrowserSettingsModel? browserSettingsModel = SettingsHelper.GetBrowserSettings();
                CommunicationSettingsModel? notifySettingsModel = SettingsHelper.GetCommunicationSettings();
                SeleniumTenBisAggregation seleniumTenBisAggregation = new SeleniumTenBisAggregation(browserSettingsModel);
                ICommunication communcation = NotifierFactory.CreateNotifier(notifySettingsModel, seleniumTenBisAggregation);
                communcation?.AlertContactAboutScript();
                communcation?.ValidateRunningScript();
                Console.ReadLine();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
                throw;
            }
        }
    }
}
