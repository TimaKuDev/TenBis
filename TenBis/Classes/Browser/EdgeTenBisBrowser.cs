using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using TenBis.Interfaces;

namespace TenBis.Classes.Browser
{
    internal class EdgeTenBisBrowser : TenBisBrowser, IBrowser
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly EdgeDriverService _edgeDriverService;
        private readonly EdgeOptions _edgeOptions;
        private readonly EdgeDriver _edgeDriver;

        public EdgeTenBisBrowser(string? userProfilePath)
        {
            _logger.Info($"{Helper.GetCurrentMethod()} Settings initialization began");

            _amountOfTries = 0;
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

            _aggregateButton = _edgeDriver.FindElements(By.XPath(TenBisWebsiteInfo.LoadCardButtonElement));
            if (!TryClickingOnAggregateButton())
            {
                return;
            }

            _continueButton = _edgeDriver.FindElements(By.XPath(TenBisWebsiteInfo.ContinueButtonElement));
            if (!TryClickingOnContinueButton())
            {
                return;
            }

            _chargeCardButton = _edgeDriver.FindElements(By.XPath(TenBisWebsiteInfo.ChargeCardButtonElement));
            if (!TryClickingOnChargeButton())
            {
                return;
            }

            AggregatedSuccessfully = true;
            _logger.Info($"{Helper.GetCurrentMethod()}: The process of money aggregation has been successfully completed");
        }

        public void StartTenBisWebsite()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the launch of website 10Bis");

            _edgeDriver.Manage().Window.Maximize();
            _edgeDriver.Navigate().GoToUrl(TenBisWebsiteInfo.TenBisUrl);

            _logger.Info($"{Helper.GetCurrentMethod()}: Website 10Bis has been successfully launched.");
        }

        public void ValidateUserLoggedIn()
        {
            Task.Delay(1500).Wait();
            _dropDownImage = _edgeDriver.FindElements(By.XPath(TenBisWebsiteInfo.DropDownImageElement));
            base.ValidateUserLoggedIn();
        }

        public string GetMessage()
        {
            _currentBalanceSpan = _edgeDriver.FindElements(By.ClassName(TenBisWebsiteInfo.CurrentBalanceSpanElement));
            return base.GetMessage();
        }
    }
}