using System.Text.Json.Serialization;
using TenBis.Enums;

namespace TenBis.SettingsFolder.Models
{
    internal class BrowserSettingsModel
    {
        [JsonConverter(typeof(BrowserType))]
        public BrowserType BrowserType { get; set; }

        public string UserProfilePath { get; set; }

    }
}
