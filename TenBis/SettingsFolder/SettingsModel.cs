using System.Text.Json.Serialization;
using TenBis.Enums;

namespace TenBis.SettingsFolder
{
    internal class SettingsModel
    {
        [JsonConverter(typeof(BrowserType))]
        public BrowserType BrowserType {  get; set; }
        [JsonConverter(typeof(NotifyType))]
        public NotifyType NotifyType {  get; set; }
        public string UserProfilePath { get; set; }
        public string NotifyTo { get; set; }
    }
}
