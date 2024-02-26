using Newtonsoft.Json;
using NLog;
using TenBis.Classes;
using TenBis.SettingsFolder.Models;

namespace TenBis.SettingsFolder
{
    internal static class SettingsHelper
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public static BrowserSettingsModel GetBrowserSettins()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting getting the user path");
            using StreamReader reader = new StreamReader("BrowserSettings.js");
            string browserSettingsPathJS = reader.ReadToEnd();
            BrowserSettingsModel? deserializedBrowserSettinsModel = JsonConvert.DeserializeObject<BrowserSettingsModel>(browserSettingsPathJS);

            _logger.Info($"{Helper.GetCurrentMethod()}: Finished, the user path: {deserializedBrowserSettinsModel.UserProfilePath}");
            return deserializedBrowserSettinsModel;
        }

        public static CommunicationSettingsModel GetNotifySettins()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting getting the user path");
            using StreamReader reader = new StreamReader("NotifySettings.js");
            string chromeUserPathJS = reader.ReadToEnd();
            CommunicationSettingsModel? deserializedChromePathModel = JsonConvert.DeserializeObject<CommunicationSettingsModel>(chromeUserPathJS);

           // _logger.Info($"{Helper.GetCurrentMethod()}: Finished, the user path: {deserializedChromePathModel.UserProfilePath}");
            return deserializedChromePathModel;
        }
    }
}
