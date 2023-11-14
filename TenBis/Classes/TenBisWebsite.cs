using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;

namespace TenBis.Classes
{
    internal class TenBisWebsite
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ChromeDriverService _chromeDriverService;
        private readonly ChromeOptions _chromeOptions;
        private readonly ChromeDriver _chromeDriver;
        private ReadOnlyCollection<IWebElement> _dropDownImage;
        private int _amountOfTries;

        public TenBisWebsite(string? chromeUserPath)
        {
            _logger.Info($"{Helper.GetCurrentMethod()} Settings initialization began");

            _amountOfTries = 0;
            _chromeDriverService = ChromeDriverService.CreateDefaultService();
            _chromeOptions = new ChromeOptions();
            _chromeOptions.AddArgument($"--user-data-dir={chromeUserPath}");
            _chromeDriver = new ChromeDriver(_chromeDriverService, _chromeOptions);

            _logger.Info($"{Helper.GetCurrentMethod()} Settings initialization finished successfully");
        }

        ~TenBisWebsite()
        {
            Dispose();
        }

        public void Dispose()
        {
            _chromeDriver?.Close();
            _chromeDriver?.Quit();
            _chromeDriver?.Dispose();
        }

        public void StartTenBisWebsite()
        {
            const string TEN_BIS_URL = "https://www.10bis.co.il/next/user-report?dateBias=0";
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the launch of website 10Bis");

            _chromeDriver.Manage().Window.Maximize();
            _chromeDriver.Navigate().GoToUrl(TEN_BIS_URL);

            _logger.Info($"{Helper.GetCurrentMethod()}: Website 10Bis has been successfully launched.");
        }

        public void ValidateUserLoggedIn()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the process of log-in validation.");
            Task.Delay(2000).Wait();
            const string DROP_DOWN_IMAGE = @"//img[@class=""styled__Img-sc-1f9rnwm-0 bDSlBz ActionMenu__MenuTriangleButton-sc-15ye1k-1 kwtrcZ""]";
            _dropDownImage = _chromeDriver.FindElements(By.XPath(DROP_DOWN_IMAGE));
            if (_dropDownImage.Count == 0)
            {
                Task.Delay(28000);
                RetryUserLoggedInValidation();
            }

            _logger.Info($"{Helper.GetCurrentMethod()}: Log-in validation process has been successfully completed");
        }

        private void RetryUserLoggedInValidation()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Log-in validation Retrying attempt: {_amountOfTries + 1}");

            if (_amountOfTries == 3)
            {
                throw new Exception($"The maximum number of validation attempts, which is {_amountOfTries}, has been reached");
            }

            ValidateUserLoggedIn();
            _amountOfTries++;
        }

        internal void AggregateMoneyToPoints()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the process of money aggregation");

            Task.Delay(1000).Wait();
            const string LOAD_CARD_BUTTON = @"//button[@class=""Button-sc-1n9hyby-0 AddToCredit__AddCreditButton-sc-1ux4vzy-3 fQLuJl kipuFE""]";
            ReadOnlyCollection<IWebElement> aggregateButton = _chromeDriver.FindElements(By.XPath(LOAD_CARD_BUTTON));
            if (!aggregateButton[0].Enabled)
            {
                _logger.Info($"{Helper.GetCurrentMethod()}: The aggregation process failed because the ‘Load Card’ button was disabled");
                return;
            }

            aggregateButton[0].Click();

            Task.Delay(1000).Wait();
            const string CONTINIUE_BUTTON = @"//button[@class=""Button-sc-1n9hyby-0 Styled__SubmitButton-sc-182pt85-0 fQLuJl dqfjan""]";
            ReadOnlyCollection<IWebElement> continiueButton = _chromeDriver.FindElements(By.XPath(CONTINIUE_BUTTON));
            if (!continiueButton[0].Enabled)
            {
                _logger.Info($"{Helper.GetCurrentMethod()}: The aggregation process failed because the ‘Continue’ button was disabled");
                return;
            }

            continiueButton[0].Click();

            Task.Delay(1000).Wait();
            const string CHARGE_CARD_BUTTON = @"//button[@class=""Button-sc-1n9hyby-0 Styled__SubmitButton-sc-182pt85-0 fQLuJl dqfjan""]";
            ReadOnlyCollection<IWebElement> chargeCardButton = _chromeDriver.FindElements(By.XPath(CHARGE_CARD_BUTTON));
            if (!chargeCardButton[0].Enabled)
            {
                _logger.Info($"{Helper.GetCurrentMethod()}: The aggregation process failed because the ‘Charge Card’ button was disabled");
                return;
            }

            chargeCardButton[0].Click();
            Task.Delay(1000).Wait();

            _logger.Info($"{Helper.GetCurrentMethod()}: The process of money aggregation has been successfully completed");
        }
    }
}
