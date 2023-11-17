using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Collections.ObjectModel;
using TenBis.Interfaces;

namespace TenBis.Classes.Browser
{
    internal class FireFoxTenBisBrowser : TenBisBrowser, IBrowser
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly FirefoxDriverService _firefoxDriverService;
        private readonly FirefoxOptions _firefoxOptions;
        private readonly FirefoxDriver _firefoxDriver;
        private ReadOnlyCollection<IWebElement> _dropDownImage;
        private int _amountOfTries;

        public FireFoxTenBisBrowser(string? userProfilePath)
        {
            _logger.Info($"{Helper.GetCurrentMethod()} Settings initialization began");

            _amountOfTries = 0;
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
            _firefoxDriver?.Quit();
            _firefoxDriver?.Dispose();
        }

        public bool TryAggregateMoneyToPoints()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the process of money aggregation");

            _aggregateButton = _firefoxDriver.FindElements(By.XPath(TenBisWebsiteInfo.LoadCardButtonElement));
            if (!TryClickingOnAggregateButton())
            {
                return false;
            }

            _continueButton = _firefoxDriver.FindElements(By.XPath(TenBisWebsiteInfo.ContinueButtonElement));
            if (!TryClickingOnContinueButton())
            {
                return false;
            }

            _chargeCardButton = _firefoxDriver.FindElements(By.XPath(TenBisWebsiteInfo.ChargeCardButtonElement));
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

            _firefoxDriver.Manage().Window.Maximize();
            _firefoxDriver.Navigate().GoToUrl(TenBisWebsiteInfo.TenBisUrl);

            _logger.Info($"{Helper.GetCurrentMethod()}: Website 10Bis has been successfully launched.");
        }

        public void ValidateUserLoggedIn()
        {
            Task.Delay(1500).Wait();
            _dropDownImage = _firefoxDriver.FindElements(By.XPath(TenBisWebsiteInfo.DropDownImageElement));
            base.ValidateUserLoggedIn();
        }

        public string GetCurrentPointsAmount()
        {
            _currentBalanceSpan = _firefoxDriver.FindElements(By.ClassName(TenBisWebsiteInfo.CurrentBalanceSpanElement));
            return base.GetCurrentPointsAmount();
        }
    }
}