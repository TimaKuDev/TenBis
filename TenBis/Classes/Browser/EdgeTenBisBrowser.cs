using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using TenBis.Interfaces;

namespace TenBis.Classes.Browser
{
    internal class EdgeTenBisBrowser : IBrowser
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly EdgeDriverService _edgeDriverService;
        private readonly EdgeOptions _edgeOptions;
        private readonly EdgeDriver _edgeDriver;
        private bool _aggregatedSuccessfully;

        public EdgeTenBisBrowser(string? userProfilePath)
        {
            _logger.Info($"{Helper.GetCurrentMethod()} Settings initialization began");

            _aggregatedSuccessfully = false;
            _edgeDriverService = EdgeDriverService.CreateDefaultService();
            _edgeOptions = new EdgeOptions();
            _edgeOptions.AddArgument($"--user-data-dir={userProfilePath}");
            _edgeDriver = new EdgeDriver(_edgeDriverService, _edgeOptions);

            _logger.Info($"{Helper.GetCurrentMethod()} Settings initialization finished successfully");
        }

        ~EdgeTenBisBrowser()
        {
            Dispose();
        }

        public void Dispose()
        {
            _edgeDriver?.Close();
            Task.Delay(1000).Wait();
            _edgeDriver?.Quit();
            Task.Delay(1000).Wait();
            _edgeDriver?.Dispose();
        }

        public void AggregateMoneyToPoints()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the process of money aggregation");

            if (!TenBisBrowser.TryClickingOnAggregateButton(_edgeDriver.FindElements(By.XPath(TenBisWebsiteInfo.LoadCardButtonElement))))
            {
                return;
            }

            if (!TenBisBrowser.TryClickingOnContinueButton(_edgeDriver.FindElements(By.XPath(TenBisWebsiteInfo.ContinueButtonElement))))
            {
                return;
            }

            if (!TenBisBrowser.TryClickingOnChargeButton(_edgeDriver.FindElements(By.XPath(TenBisWebsiteInfo.ChargeCardButtonElement))))
            {
                return;
            }

            _aggregatedSuccessfully = true;
            _logger.Info($"{Helper.GetCurrentMethod()}: The process of money aggregation has been successfully completed");
        }

        public void StartTenBisWebsite()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the launch of website 10Bis");

            _edgeDriver.Manage().Window.Maximize();
            _edgeDriver.Navigate().GoToUrl(TenBisWebsiteInfo.TenBisUrl);

            _logger.Info($"{Helper.GetCurrentMethod()}: Website 10Bis has been successfully launched.");
        }

        public void IsUserLoggedInValidation()
        {
            Task.Delay(2000);
            if (!TenBisBrowser.IsUserLoggedIn(_edgeDriver.FindElements(By.XPath(TenBisWebsiteInfo.DropDownImageElement))))
            {
                IsUserLoggedInValidation();
            }
        }

        public string GetMessage()
        {
            return TenBisBrowser.GetMessage(_aggregatedSuccessfully, _edgeDriver.FindElements(By.ClassName(TenBisWebsiteInfo.CurrentBalanceSpanElement)));
        }
    }
}