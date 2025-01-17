﻿using TenBis.Classes.Notifiers;
using TenBis.Enums;
using TenBis.Interfaces;
using TenBis.SettingsFolder.Models;

namespace TenBis.Factories
{
    internal static class NotifierFactory
    {
        internal static ICommunication? CreateNotifier(CommunicationSettingsModel? notifySettingsModel, IAggregate aggrgate)
        {
            if (notifySettingsModel == null)
            {
                throw new ArgumentNullException(nameof(notifySettingsModel));
            }

            switch (notifySettingsModel.NotifyType)
            {
                case NotifyType.Email:
                    return new EmailCommunication(notifySettingsModel.NotifyTo, aggrgate);
                case NotifyType.Telegram:
                    return new TelegramCommunication(notifySettingsModel.Token, notifySettingsModel.ChatId, aggrgate);
                default:
                    return null;
            }
        }
    }
}