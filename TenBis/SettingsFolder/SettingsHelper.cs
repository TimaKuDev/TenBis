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
            using StreamReader reader = new StreamReader("ChromeSettings.js");
            string chromeUserPathJS = reader.ReadToEnd();
            BrowserSettingsModel? deserializedChromePathModel = JsonConvert.DeserializeObject<BrowserSettingsModel>(chromeUserPathJS);

            _logger.Info($"{Helper.GetCurrentMethod()}: Finished, the user path: {deserializedChromePathModel.UserProfilePath}");
            return deserializedChromePathModel;
        }

        public static NotifySettingsModel GetNotifySettins()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting getting the user path");
            using StreamReader reader = new StreamReader("ChromeSettings.js");
            string chromeUserPathJS = reader.ReadToEnd();
            NotifySettingsModel? deserializedChromePathModel = JsonConvert.DeserializeObject<NotifySettingsModel>(chromeUserPathJS);

           // _logger.Info($"{Helper.GetCurrentMethod()}: Finished, the user path: {deserializedChromePathModel.UserProfilePath}");
            return deserializedChromePathModel;
        }
    }
}
