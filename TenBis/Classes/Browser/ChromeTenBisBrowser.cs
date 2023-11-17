using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TenBis.Interfaces;

namespace TenBis.Classes.Browser
{
    internal class ChromeTenBisBrowser : TenBisBrowser, IBrowser
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ChromeDriverService _chromeDriverService;
        private readonly ChromeOptions _chromeOptions;
        private readonly ChromeDriver _chromeDriver;

        public ChromeTenBisBrowser(string? userProfilePath)
        {
            _logger.Info($"{Helper.GetCurrentMethod()} Settings initialization began");

            _amountOfTries = 0;
            _chromeDriverService = ChromeDriverService.CreateDefaultService();
            _chromeOptions = new ChromeOptions();
            _chromeOptions.AddArgument($"--user-data-dir={userProfilePath}");
            _chromeDriver = new ChromeDriver(_chromeDriverService, _chromeOptions);

            _logger.Info($"{Helper.GetCurrentMethod()} Settings initialization finished successfully");
        }

        ~ChromeTenBisBrowser()
        {
            Dispose();
        }

        public void Dispose()
        {
            _chromeDriver?.Close();
            _chromeDriver?.Quit();
            _chromeDriver?.Dispose();
        }

        public bool TryAggregateMoneyToPoints()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the process of money aggregation");

            _aggregateButton = _chromeDriver.FindElements(By.XPath(TenBisWebsiteInfo.LoadCardButtonElement));
            if (!TryClickingOnAggregateButton())
            {
                return false;
            }

            _continueButton = _chromeDriver.FindElements(By.XPath(TenBisWebsiteInfo.ContinueButtonElement));
            if (!TryClickingOnContinueButton())
            {
                return false;
            }

            _chargeCardButton = _chromeDriver.FindElements(By.XPath(TenBisWebsiteInfo.ChargeCardButtonElement));
            if (!TryClickingOnChargeButton())
            {
                return false;
            }

            _logger.Info($"{Helper.GetCurrentMethod()}: The process of money aggregation has been successfully completed");
            return true;
        }

        public void StartTenBisWebsite()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the launch of website 10Bis");

            _chromeDriver.Manage().Window.Maximize();
            _chromeDriver.Navigate().GoToUrl(TenBisWebsiteInfo.TenBisUrl);

            _logger.Info($"{Helper.GetCurrentMethod()}: Website 10Bis has been successfully launched.");
        }

        public void ValidateUserLoggedIn()
        {
            Task.Delay(1500).Wait();
            _dropDownImage = _chromeDriver.FindElements(By.XPath(TenBisWebsiteInfo.DropDownImageElement));
            base.ValidateUserLoggedIn();
        }

        public string GetCurrentPointsAmount()
        {
            _currentBalanceSpan = _chromeDriver.FindElements(By.ClassName(TenBisWebsiteInfo.CurrentBalanceSpanElement));
            return base.GetCurrentPointsAmount();
        }
    }
}