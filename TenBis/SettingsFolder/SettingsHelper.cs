using Newtonsoft.Json;
using NLog;
using TenBis.Classes;
using TenBis.SettingsFolder.Models;

namespace TenBis.SettingsFolder
{
    internal static class SettingsHelper
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public static BrowserSettingsModel? GetBrowserSettings()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting getting browser settings");
            using StreamReader reader = new StreamReader("BrowserSettings.js");
            string browserSettingsPathJS = reader.ReadToEnd();
            BrowserSettingsModel? deserializedBrowserSettinsModel = JsonConvert.DeserializeObject<BrowserSettingsModel>(browserSettingsPathJS);

            _logger.Info($"{Helper.GetCurrentMethod()}: Finished, getting browser settings");
            return deserializedBrowserSettinsModel;
        }

        public static CommunicationSettingsModel? GetCommunicationSettings()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting getting communication settings");
            using StreamReader reader = new StreamReader("CommunicationSettings.js");
            string communicationSettingsPathJs = reader.ReadToEnd();
            CommunicationSettingsModel? deserializedChromePathModel = JsonConvert.DeserializeObject<CommunicationSettingsModel>(communicationSettingsPathJs);

            _logger.Info($"{Helper.GetCurrentMethod()}: Finished, getting communication settings");
            return deserializedChromePathModel;
        }
    }
}
