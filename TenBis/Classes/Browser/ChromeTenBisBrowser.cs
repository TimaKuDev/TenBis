using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TenBis.Interfaces;

namespace TenBis.Classes.Browser
{
    internal class ChromeTenBisBrowser : IBrowser
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ChromeDriverService _chromeDriverService;
        private readonly ChromeOptions _chromeOptions;
        private readonly ChromeDriver _chromeDriver;
        private bool _aggregatedSuccessfully;

        public ChromeTenBisBrowser(string? userProfilePath)
        {
            _logger.Info($"{Helper.GetCurrentMethod()} Settings initialization began");

            _aggregatedSuccessfully = false;
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
            Task.Delay(1000).Wait();
            _chromeDriver?.Quit();
            Task.Delay(1000).Wait();
            _chromeDriver?.Dispose();
        }

        public void AggregateMoneyToPoints()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the process of money aggregation");
            if (!TenBisBrowser.TryClickingOnAggregateButton(_chromeDriver.FindElements(By.XPath(TenBisWebsiteInfo.LoadCardButtonElement))))
            {
                return;
            }

            if (!TenBisBrowser.TryClickingOnContinueButton(_chromeDriver.FindElements(By.XPath(TenBisWebsiteInfo.ContinueButtonElement))))
            {
                return;
            }

            if (!TenBisBrowser.TryClickingOnChargeButton(_chromeDriver.FindElements(By.XPath(TenBisWebsiteInfo.ChargeCardButtonElement))))
            {
                return;
            }

            _aggregatedSuccessfully = true;
            _logger.Info($"{Helper.GetCurrentMethod()}: The process of money aggregation has been successfully completed");
        }

        public void StartTenBisWebsite()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the launch of website 10Bis");

            _chromeDriver.Manage().Window.Maximize();
            _chromeDriver.Navigate().GoToUrl(TenBisWebsiteInfo.TenBisUrl);

            _logger.Info($"{Helper.GetCurrentMethod()}: Website 10Bis has been successfully launched.");
        }

        public void IsUserLoggedInValidation()
        {
            Task.Delay(4000);
            if (!TenBisBrowser.IsUserLoggedIn(_chromeDriver.FindElements(By.XPath(TenBisWebsiteInfo.DropDownImageElement))))
            {
                IsUserLoggedInValidation();
            }
        }

        public string GetMessage()
        {
            return TenBisBrowser.GetMessage(_aggregatedSuccessfully, _chromeDriver.FindElements(By.ClassName(TenBisWebsiteInfo.CurrentBalanceSpanElement)));
        }
    }
}