using NLog;
using TenBis.Factories;
using TenBis.Interfaces;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Aggregation
{
    internal class SeleniumTenBisAggregation : IAggregate
    {
        private readonly BrowserSettingsModel _browserSettingsModel;
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        public SeleniumTenBisAggregation(BrowserSettingsModel? browserSettingsModel)
        {
            if (browserSettingsModel is null)
            {
                throw new ArgumentNullException(nameof(browserSettingsModel));
            }

            _browserSettingsModel = browserSettingsModel;
        }

        public string Aggregate()
        {
            IBrowser? browser = null;
            try
            {
                _logger.Info($"{Helper.GetCurrentMethod()}: Starting aggregating");
                browser = BrowserFactory.CreateBrowser(_browserSettingsModel.BrowserType, _browserSettingsModel.UserProfilePath);
                browser?.StartTenBisWebsite();
                browser?.IsUserLoggedInValidation();
                browser?.AggregateMoneyToPoints();
                string messsage = browser?.GetMessage() ?? "Failed to create a browser, in order to aggregate money to points";
                _logger.Info($"{Helper.GetCurrentMethod()}: Finished, aggregating");
                return messsage;

            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message);
                return "There was an error while trying to aggregate check the logs files.";
            }
            finally
            {
                browser?.Dispose();
            }
        }
    }
}
