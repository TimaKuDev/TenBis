using TenBis.Factories;
using TenBis.Interfaces;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Aggregation
{
    internal class SeleniumTenBisAggregation : IAggregate
    {
        private readonly BrowserSettingsModel _browserSettingsModel;

        public SeleniumTenBisAggregation(BrowserSettingsModel browserSettingsModel)
        {
            _browserSettingsModel = browserSettingsModel;
        }

        public string Aggregate()
        {
            IBrowser browser = BrowserFactory.CreateBrowser(_browserSettingsModel.BrowserType, _browserSettingsModel.UserProfilePath);
            browser?.StartTenBisWebsite();
            browser?.ValidateUserLoggedIn();
            browser?.AggregateMoneyToPoints();
            return browser?.GetMessage() ?? "Failed to create a browser, in order to aggregate money to points";
        }
    }
}
