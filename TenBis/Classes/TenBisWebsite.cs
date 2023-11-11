using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Text;

namespace TenBis.Classes
{
    internal class TenBisWebsite
    {
        private const string TEN_BIS_URL = "https://www.10bis.co.il/next/user-report?dateBias=0";
        ChromeDriverService _chromeDriverService;
        ChromeOptions _chromeOptions;
        ChromeDriver _chromeDriver;
        System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> _dropDownImage;
        public TenBisWebsite()
        {
            _chromeDriverService = ChromeDriverService.CreateDefaultService();
            _chromeOptions = new ChromeOptions();
            _chromeOptions.AddArgument(@"--user-data-dir=C:\Users\User\AppData\Local\Google\Chrome\User Data\Default");
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
            _chromeDriver.Manage().Window.Maximize();
            _chromeDriver.Navigate().GoToUrl(TEN_BIS_URL);
        }

        public void ValidateUserLoggedIn()
        {
            Task.Delay(2000).Wait();
            const string DROP_DOWN_IMAGE = @"//img[@class=""styled__Img-sc-1f9rnwm-0 bDSlBz ActionMenu__MenuTriangleButton-sc-15ye1k-1 kwtrcZ""]";
            _dropDownImage = _chromeDriver.FindElements(By.XPath(DROP_DOWN_IMAGE));
            if (_dropDownImage.Count == 0)
            {
                Task.Delay(28000);
                ValidateUserLoggedIn();
            }
        }

        internal void AggregateMoneyToPoints()
        {
            Task.Delay(1000).Wait();
            const string DROP_DOWN_IMAGE = @"//button[@class=""Button-sc-1n9hyby-0 AddToCredit__AddCreditButton-sc-1ux4vzy-3 fQLuJl kipuFE""]";
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> aggregateButton = _chromeDriver.FindElements(By.XPath(DROP_DOWN_IMAGE));
            if (!aggregateButton[0].Enabled)
            {
                return;
            }

            aggregateButton[0].Click();
           
        }
    }
}
