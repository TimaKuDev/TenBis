using Newtonsoft.Json;
using NLog;

namespace TenBis.Classes
{
    internal static class UserPath 
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public static string GetChromePath()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Starting getting the user path");
            using StreamReader reader = new StreamReader("ChromeUserPath.js");
            string chromeUserPathJS = reader.ReadToEnd();
            ChromePathModel? deserializedChromePathModel = JsonConvert.DeserializeObject<ChromePathModel>(chromeUserPathJS);

            _logger.Info($"{Helper.GetCurrentMethod()}: Finished, the user path: {deserializedChromePathModel.ChromeUserPath}");
            return deserializedChromePathModel.ChromeUserPath;
        }
    }
}
