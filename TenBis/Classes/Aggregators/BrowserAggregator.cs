using FluentResults;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using TenBis.Logging;

namespace TenBis.Classes.Aggregators
{
    internal abstract class BrowserAggregator
    {
        protected const string UserDataDirPrefix = "--user-data-dir=";
        protected const string TenBisUrl = "https://www.10bis.co.il/next/user-report?dateBias=0";
        private const string DropDownImageElement = @"//img[@class=""styled__Img-sc-1f9rnwm-0 bDSlBz ActionMenu__MenuTriangleButton-sc-15ye1k-1 kwtrcZ""]";
        private const string LoadCardButtonElement = @"//button[@class=""Button-sc-1n9hyby-0 AddToCredit__AddCreditButton-sc-1ux4vzy-3 fQLuJl kipuFE""]";
        private const string ContinueButtonElement = @"//button[@class=""Button-sc-1n9hyby-0 Styled__SubmitButton-sc-182pt85-0 fQLuJl dqfjan""]";
        private const string ChargeCardButtonElement = @"//button[@class=""Button-sc-1n9hyby-0 Styled__SubmitButton-sc-182pt85-0 fQLuJl dqfjan""]";
        private const string CurrentBalanceSpanElement = "PrepaidCard__Balance-sc-1yb9170-4";

        private static Result IsUserLoggedIn(WebDriver webDriver)
        {
            try
            {
                Logger.FunctionStarted();
                Task.Delay(2000);

                int triesAmount = 0;
                ReadOnlyCollection<IWebElement> dropDownImage = webDriver.FindElements(By.XPath(DropDownImageElement));
                bool userLoggedIn = dropDownImage.Count != 0;
                while (triesAmount < 3 && !userLoggedIn)
                {
                    Task.Delay(28000).Wait();

                    dropDownImage = webDriver.FindElements(By.XPath(DropDownImageElement));
                    userLoggedIn = dropDownImage.Count != 0;
                    triesAmount++;
                }

                Logger.FunctionFinished();
                return Result.Ok();
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                return Result.Fail(exception.Message);
            }
        }

        private static Result ClickingOnAggregateButton(WebDriver webDriver)
        {
            Logger.FunctionStarted();
            Task.Delay(millisecondsDelay: 1000);

            ReadOnlyCollection<IWebElement>? aggregateButton = webDriver.FindElements(By.XPath(LoadCardButtonElement));
            bool? isAggregateButtonEnabled = aggregateButton?[0].Enabled;
            if (!isAggregateButtonEnabled.HasValue || !isAggregateButtonEnabled.Value)
            {
                Logger.Error("The aggregation process failed because the ‘Load Card’ button was disabled");
                return Result.Fail("The aggregation process failed because the ‘Load Card’ button was disabled");
            }

            aggregateButton?[0].Click();

            Logger.FunctionFinished();
            return Result.Ok();
        }

        private static Result ClickingOnContinueButton(WebDriver webDriver)
        {
            Logger.FunctionStarted();
            Task.Delay(millisecondsDelay: 1000);

            ReadOnlyCollection<IWebElement>? continueButton = webDriver.FindElements(By.XPath(ContinueButtonElement));
            bool? isContinueButtonEnabled = continueButton?[0].Enabled;
            if (!isContinueButtonEnabled.HasValue || !isContinueButtonEnabled.Value)
            {
                Logger.Error("The aggregation process failed because the ‘Continue’ button was disabled");
                return Result.Fail("The aggregation process failed because the ‘Continue’ button was disabled");
            }


            continueButton?[0].Click();

            Logger.FunctionFinished();
            return Result.Ok();
        }

        private static Result ClickingOnChargeButton(WebDriver webDriver)
        {
            Logger.FunctionStarted();
            Task.Delay(1000);

            ReadOnlyCollection<IWebElement>? chargeCardButton = webDriver.FindElements(By.XPath(ChargeCardButtonElement));
            bool? isChargeCardButtonEnabled = chargeCardButton?[0].Enabled;
            if (!isChargeCardButtonEnabled.HasValue || !isChargeCardButtonEnabled.Value)
            {
                Logger.Error("The aggregation process failed because the ‘Charge Card’ button was disabled");
                return Result.Fail("The aggregation process failed because the ‘Charge Card’ button was disabled");
            }

            chargeCardButton?[0].Click();

            Logger.FunctionFinished();
            return Result.Ok();
        }

        protected static Result<string> Aggregate(WebDriver webDriver)
        {
            try
            {
                Logger.FunctionStarted();
                Result startTenBisWebsiteResult = StartTenBisWebsite(webDriver);
                if (startTenBisWebsiteResult.IsFailed)
                {
                    return startTenBisWebsiteResult;
                }

                Result isUserLoggedInResult = IsUserLoggedIn(webDriver);
                if (isUserLoggedInResult.IsFailed)
                {
                    return isUserLoggedInResult;
                }

                Result aggregateMoneyToPointsResult = AggregateMoneyToPoints(webDriver);
                if (aggregateMoneyToPointsResult.IsFailed)
                {
                    return aggregateMoneyToPointsResult;
                }

                Result<string> currentBalanceResult = GetCurrentBalance(webDriver);
                if (currentBalanceResult.IsFailed)
                {
                    return currentBalanceResult;
                }


                Logger.FunctionFinished();
                return Result.Ok(currentBalanceResult.Value);

            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                return new Result<string>().WithError(exception.Message);
            }
        }

        private static Result<string> GetCurrentBalance(WebDriver webDriver)
        {
            Logger.FunctionStarted();
            Task.Delay(1000);

            ReadOnlyCollection<IWebElement>? currentBalanceSpan = webDriver.FindElements(By.XPath(CurrentBalanceSpanElement));

            string? currentBalance = currentBalanceSpan[0].Text;
            if (string.IsNullOrWhiteSpace(currentBalance))
            {
                Logger.Error("Failed to retrieve the current balance.");
                return Result.Fail("Failed to retrieve the current balance.");
            }

            Logger.FunctionFinished();
            return Result.Ok(currentBalance);
        }

        private static Result AggregateMoneyToPoints(WebDriver webDriver)
        {
            Logger.Info($"Initiating the process of money aggregation");


            Result clickingOnAggregateButtonResult = ClickingOnAggregateButton(webDriver);
            if (clickingOnAggregateButtonResult.IsFailed)
            {
                return clickingOnAggregateButtonResult;
            }

            Result clickingOnContinueButtonResult = ClickingOnContinueButton(webDriver);
            if (clickingOnAggregateButtonResult.IsFailed)
            {
                return clickingOnContinueButtonResult;
            }

            Result clickingOnChargeButtonResult = ClickingOnChargeButton(webDriver);
            if (clickingOnChargeButtonResult.IsFailed)
            {
                return clickingOnChargeButtonResult;
            }


            Logger.Info($"The process of money aggregation has been successfully completed");
            return Result.Ok();
        }

        private static Result StartTenBisWebsite(WebDriver webDriver)
        {
            try
            {
                Logger.FunctionStarted();

                webDriver.Manage().Window.Maximize();
                webDriver.Navigate().GoToUrl(TenBisUrl);

                Logger.FunctionFinished();
                return Result.Ok();
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                return Result.Fail(exception.Message);
            }
        }
    }
}
