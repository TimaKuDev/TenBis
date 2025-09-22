using FluentResults;
using TenBis.Classes.Notifiers;
using TenBis.Enum;
using TenBis.Interfaces;
using TenBis.SettingsFolder.Models;

namespace TenBis.Factories
{
    internal static class CommunicationFactory
    {
        internal static Result<ICommunicator?> Create(CommunicationSettings? communicationSettings)
        {
            if (communicationSettings is null)
            {
                return Result.Fail<ICommunicator?>("Communication settings are missing.");
            }

            return communicationSettings.Communication switch
            {
                Communication.Email => Result.Ok<ICommunicator?>(new EmailCommunicator(communicationSettings.Email, communicationSettings.ValidationMessageConfig)),
                Communication.Telegram => Result.Ok<ICommunicator?>(new TelegramCommunicator(communicationSettings?.Telegram, communicationSettings?.ValidationMessageConfig)),
                _ => Result.Fail<ICommunicator?>($"Unsupported communication type: {communicationSettings.Communication}.")
            };
        }
    }
}
