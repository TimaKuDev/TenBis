using System.Text.Json.Serialization;
using TenBis.Enums;

namespace TenBis.SettingsFolder.Models
{
    internal class NotifySettingsModel
    {
        [JsonConverter(typeof(NotifyType))]
        public NotifyType NotifyType { get; set; }
        public string NotifyTo { get; set; }
        public string Token { get; set; }
        public long ChatId { get; set; }
    }
}
