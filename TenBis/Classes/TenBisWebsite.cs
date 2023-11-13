using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;

namespace TenBis.Classes
{
    internal class TenBisWebsite
    {
        private ChromeDriverService _chromeDriverService;
        private ChromeOptions _chromeOptions;
        private ChromeDriver _chromeDriver;
        private ReadOnlyCollection<IWebElement> _dropDownImage;
        private int _amountOfTries;

        public TenBisWebsite()
        {
            _amountOfTries = 0;
            _chromeDriverService = ChromeDriverService.CreateDefaultService();
            _chromeOptions = new ChromeOptions();
            _chromeOptions.AddArgument(@"--user-data-dir=C:\Users\TimaK\AppData\Local\Google\Chrome\User Data\Default");
            _chromeDriver = new ChromeDriver(_chromeDriverService, _chromeOptions);
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
            _chromeDriver.Manage().Window.Maximize();
            _chromeDriver.Navigate().GoToUrl(TEN_BIS_URL);
        }

        public void ValidateUserLoggedIn()
        {
            //Think about better name for the method
            Task.Delay(2000).Wait();
            const string DROP_DOWN_IMAGE = @"//img[@class=""styled__Img-sc-1f9rnwm-0 bDSlBz ActionMenu__MenuTriangleButton-sc-15ye1k-1 kwtrcZ""]";
            _dropDownImage = _chromeDriver.FindElements(By.XPath(DROP_DOWN_IMAGE));
            if (_dropDownImage.Count == 0)
            {
                Task.Delay(28000);
                RetryUserLoggedInValidation();
            }
        }

        private void RetryUserLoggedInValidation()
        {
            if (_amountOfTries == 3)
            {
                throw new Exception();
            }

            ValidateUserLoggedIn();
            _amountOfTries++;
        }

        internal void AggregateMoneyToPoints()
        {
            Task.Delay(1000).Wait();
            const string LOAD_CARD_BUTTON = @"//button[@class=""Button-sc-1n9hyby-0 AddToCredit__AddCreditButton-sc-1ux4vzy-3 fQLuJl kipuFE""]";
            ReadOnlyCollection<IWebElement> aggregateButton = _chromeDriver.FindElements(By.XPath(LOAD_CARD_BUTTON));
            if (!aggregateButton[0].Enabled)
            {
                return;
            }

            aggregateButton[0].Click();

            Task.Delay(1000).Wait();
            const string CONTINIUE_BUTTON = @"//button[@class=""Button-sc-1n9hyby-0 Styled__SubmitButton-sc-182pt85-0 fQLuJl dqfjan""]";
            ReadOnlyCollection<IWebElement> continiueButton = _chromeDriver.FindElements(By.XPath(CONTINIUE_BUTTON));
            if (!continiueButton[0].Enabled)
            {
                return;
            }

            continiueButton[0].Click();

            Task.Delay(1000).Wait();
            const string CHARGE_CARD_BUTTON = @"//button[@class=""Button-sc-1n9hyby-0 Styled__SubmitButton-sc-182pt85-0 fQLuJl dqfjan""]";
            ReadOnlyCollection<IWebElement> chargeCardButton = _chromeDriver.FindElements(By.XPath(CHARGE_CARD_BUTTON));
            if (!chargeCardButton[0].Enabled)
            {
                return;
            }

            chargeCardButton[0].Click();
            Task.Delay(1000).Wait();
        }
    }
}
