using System.Text.Json.Serialization;
using TenBis.Enum;

namespace TenBis.SettingsFolder.Models
{
    internal class CommunicationSettings
    {
        [JsonConverter(typeof(Communication))]
        public Communication Communication { get; set; }
        public EmailSettings? Email { get; set; }
        public TelegramSettings? Telegram { get; set; }
    }

    internal class EmailSettings
    {
        public string? SmtpServer { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? RecipientEmail { get; set; }
    }

    internal class TelegramSettings
    {
        public string? BotToken { get; set; }
        public long? ChatId { get; set; }
    }
}
