using Newtonsoft.Json;
using NLog;
using TenBis.Classes;

namespace TenBis.SettingsFolder
{
    internal static class Settings
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public static SettingsModel GetSettins()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting getting the user path");
            using StreamReader reader = new StreamReader("Settings.js");
            string chromeUserPathJS = reader.ReadToEnd();
            SettingsModel? deserializedChromePathModel = JsonConvert.DeserializeObject<SettingsModel>(chromeUserPathJS);

            _logger.Info($"{Helper.GetCurrentMethod()}: Finished, the user path: {deserializedChromePathModel.UserProfilePath}");
            return deserializedChromePathModel;
        }
    }
}
