using System.Text.Json.Serialization;
using TenBis.Enum;

namespace TenBis.SettingsFolder.Models
{
    internal class AggregationSettings
    {
        [JsonConverter(typeof(Aggregation))]
        internal Aggregation Aggregation { get; set; }
        internal BrowserSettings? Browser { get; set; }
        internal ApiSettings? Api { get; set; }
    }

    internal class BrowserSettings
    {
        [JsonConverter(typeof(Browser))]
        internal Browser BrowserType { get; set; }
        internal string? UserProfilePath { get; set; }
    }

    internal class ApiSettings
    {
        internal string? BaseUrl { get; set; }
        internal string? ApiKey { get; set; }
        internal int TimeoutSeconds { get; set; }
    }
}
