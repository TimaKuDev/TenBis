using TenBis.Factories;
using TenBis.Interfaces;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Aggregation
{
    internal class SeleniumTenBisAggregation
    {
        public static int Aggrgate(BrowserSettingsModel browserSettingsModel, out string? message)
        {
            IBrowser browser = BrowserFactory.CreateBrowser(browserSettingsModel.BrowserType, browserSettingsModel.UserProfilePath);
            browser?.StartTenBisWebsite();
            browser?.ValidateUserLoggedIn();
            browser?.AggregateMoneyToPoints();
            message = browser?.GetMessage();

            return 0;
        }
    }
}
