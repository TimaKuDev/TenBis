using TenBis.Classes.Notifiers;
using TenBis.Enums;
using TenBis.Interfaces;

namespace TenBis.Factories
{
    internal class NotifierFactory
    {
        internal static INotifier CreateNotifier(NotifyType notifyType, string notifyTo, string? currentBalanceAmount, bool? isSuccessfullyAggregation)
        {
            switch (notifyType)
            {
                case NotifyType.Email:
                    return new EmailNotifier(notifyTo, currentBalanceAmount, isSuccessfullyAggregation);
                default:
                    return null;
            }
        }
    }
}