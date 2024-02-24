using TenBis.Classes.Notifiers;
using TenBis.Enums;
using TenBis.Interfaces;
using TenBis.SettingsFolder.Models;

namespace TenBis.Factories
{
    internal class NotifierFactory
    {
        internal static ICommunication CreateNotifier(NotifySettingsModel notifySettingsModel)
        {
            switch (notifySettingsModel.NotifyType)
            {
                case NotifyType.Email:
                    return new EmailCommunication(notifySettingsModel.NotifyTo);
                case NotifyType.Telegram:
                    return new TelegramCommunication(notifySettingsModel.Token, notifySettingsModel.ChatId);
                default:
                    return null;
            }
        }
    }
}