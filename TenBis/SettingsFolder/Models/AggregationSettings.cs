using System.Text.Json.Serialization;
using TenBis.Enum;

namespace TenBis.SettingsFolder.Models
{
    internal class AggregationSettings
    {
        [JsonConverter(typeof(Aggregation))]
        public Aggregation Aggregation { get; set; }
        public BrowserSettings? Browser { get; set; }
        public ApiSettings? Api { get; set; }
    }

    internal class BrowserSettings
    {
        [JsonConverter(typeof(Browser))]
        public Browser BrowserType { get; set; }
        public string? UserProfilePath { get; set; }
    }

    internal class ApiSettings
    {
        public string? BaseUrl { get; set; }
        public string? Cookie { get; set; }
        public int? TimeoutSeconds { get; set; }
    }
}
