using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using TenBis.Interfaces;

namespace TenBis.Classes.Browser
{
    internal class FireFoxTenBisBrowser : IBrowser
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly FirefoxDriverService _firefoxDriverService;
        private readonly FirefoxOptions _firefoxOptions;
        private readonly FirefoxDriver _firefoxDriver;
        private bool _aggregatedSuccessfully;

        public FireFoxTenBisBrowser(string? userProfilePath)
        {
            _logger.Info($"{Helper.GetCurrentMethod()} Settings initialization began");

            _aggregatedSuccessfully = false;
            _firefoxDriverService = FirefoxDriverService.CreateDefaultService();
            _firefoxOptions = new FirefoxOptions();
            _firefoxOptions.AddArgument($"--user-data-dir={userProfilePath}");
            _firefoxDriver = new FirefoxDriver(_firefoxDriverService, _firefoxOptions);

            _logger.Info($"{Helper.GetCurrentMethod()} Settings initialization finished successfully");
        }

        ~FireFoxTenBisBrowser()
        {
            Dispose();
        }

        public void Dispose()
        {
            _firefoxDriver?.Close();
            Task.Delay(1000).Wait();
            _firefoxDriver?.Quit();
            Task.Delay(1000).Wait();
            _firefoxDriver?.Dispose();
        }

        public void AggregateMoneyToPoints()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the process of money aggregation");

            if (!TenBisBrowser.TryClickingOnAggregateButton(_firefoxDriver.FindElements(By.XPath(TenBisWebsiteInfo.LoadCardButtonElement))))
            {
                return;
            }

            if (!TenBisBrowser.TryClickingOnContinueButton(_firefoxDriver.FindElements(By.XPath(TenBisWebsiteInfo.ContinueButtonElement))))
            {
                return;
            }

            if (!TenBisBrowser.TryClickingOnChargeButton(_firefoxDriver.FindElements(By.XPath(TenBisWebsiteInfo.ChargeCardButtonElement))))
            {
                return;
            }

            _aggregatedSuccessfully = true;
            _logger.Info($"{Helper.GetCurrentMethod()}: The process of money aggregation has been successfully completed");
        }

        public void StartTenBisWebsite()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the launch of website 10Bis");

            _firefoxDriver.Manage().Window.Maximize();
            _firefoxDriver.Navigate().GoToUrl(TenBisWebsiteInfo.TenBisUrl);

            _logger.Info($"{Helper.GetCurrentMethod()}: Website 10Bis has been successfully launched.");
        }

        public void IsUserLoggedInValidation()
        {
            Task.Delay(2000);
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating validation if user is logged in.");
            if (!TenBisBrowser.IsUserLoggedIn(_firefoxDriver.FindElements(By.XPath(TenBisWebsiteInfo.DropDownImageElement))))
            {
                IsUserLoggedInValidation();
            }

            _logger.Info($"{Helper.GetCurrentMethod()}: Finished validation if user is logged in.");
        }

        public string GetMessage()
        {
            return TenBisBrowser.GetMessage(_aggregatedSuccessfully, _firefoxDriver.FindElements(By.ClassName(TenBisWebsiteInfo.CurrentBalanceSpanElement)));
        }
    }
}