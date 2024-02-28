using TenBis.Factories;
using TenBis.Interfaces;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Aggregation
{
    internal class SeleniumTenBisAggregation : IAggrgate
    {
        private readonly BrowserSettingsModel _browserSettingsModel;

        public SeleniumTenBisAggregation(BrowserSettingsModel browserSettingsModel)
        {
            _browserSettingsModel = browserSettingsModel;
        }

        public bool TryAggrgate(out string? message)
        {
            IBrowser browser = BrowserFactory.CreateBrowser(_browserSettingsModel.BrowserType, _browserSettingsModel.UserProfilePath);
            browser?.StartTenBisWebsite();
            browser?.ValidateUserLoggedIn();
            browser?.AggregateMoneyToPoints();
            message =  browser?.GetMessage();
            return !string.IsNullOrEmpty(message);
        }
    }
}
