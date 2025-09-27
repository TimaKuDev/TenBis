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
        private const string DropDownImageElement = @"//div[@class=""OpenMenuContent-dfTXpZ fHZBl""]";
        private const string LoadCardButtonElement = @"//button[@class=""Button-dtEEMF AddCreditButton-bFDXtp iXOfjm zjGbl""]";
        private const string ContinueButtonElement = @"//button[@class=""Button-sc-1n9hyby-0 Styled__SubmitButton-sc-182pt85-0 fQLuJl dqfjan""]";
        private const string ChargeCardButtonElement = @"//button[@class=""Button-sc-1n9hyby-0 Styled__SubmitButton-sc-182pt85-0 fQLuJl dqfjan""]";
        private const string CurrentBalanceSpanElement = @"//span[@class=""Balance-gcvUbW ewtkSs""]";
        protected WebDriver? m_WebDriver;

        private Result IsUserLoggedIn()
        {
            try
            {
                Logger.FunctionStarted();
                Task.Delay(2000);

                int triesAmount = 0;
                ReadOnlyCollection<IWebElement>? dropDownImage = m_WebDriver?.FindElements(By.XPath(DropDownImageElement));
                bool userLoggedIn = dropDownImage?.Count != 0;
                while (triesAmount < 3 && !userLoggedIn)
                {
                    Task.Delay(28000).Wait();

                    dropDownImage = m_WebDriver?.FindElements(By.XPath(DropDownImageElement));
                    userLoggedIn = dropDownImage?.Count != 0;
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

        private Result ClickingOnAggregateButton()
        {
            Logger.FunctionStarted();
            Task.Delay(millisecondsDelay: 1000);

            ReadOnlyCollection<IWebElement>? aggregateButton = m_WebDriver?.FindElements(By.XPath(LoadCardButtonElement));
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

        private Result ClickingOnContinueButton()
        {
            Logger.FunctionStarted();
            Task.Delay(millisecondsDelay: 1000);

            ReadOnlyCollection<IWebElement>? continueButton = m_WebDriver?.FindElements(By.XPath(ContinueButtonElement));
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

        private Result ClickingOnChargeButton()
        {
            Logger.FunctionStarted();
            Task.Delay(1000);

            ReadOnlyCollection<IWebElement>? chargeCardButton = m_WebDriver?.FindElements(By.XPath(ChargeCardButtonElement));
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

        protected Result<string> Aggregate()
        {
            try
            {
                Logger.FunctionStarted();
                Result startTenBisWebsiteResult = StartTenBisWebsite();
                if (startTenBisWebsiteResult.IsFailed)
                {
                    return startTenBisWebsiteResult;
                }

                Result isUserLoggedInResult = IsUserLoggedIn();
                if (isUserLoggedInResult.IsFailed)
                {
                    return isUserLoggedInResult;
                }

                Result aggregateMoneyToPointsResult = AggregateMoneyToPoints();
                if (aggregateMoneyToPointsResult.IsFailed)
                {
                    return aggregateMoneyToPointsResult;
                }

                Result<string> currentBalanceResult = GetCurrentBalance();
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

        private Result<string> GetCurrentBalance()
        {
            Logger.FunctionStarted();
            Task.Delay(1000);

            ReadOnlyCollection<IWebElement>? currentBalanceSpan = m_WebDriver?.FindElements(By.XPath(CurrentBalanceSpanElement));
            if (currentBalanceSpan is null)
            {
                Logger.Error("Failed to retrieve the current balance span.");
                return Result.Fail("Failed to retrieve the current balance span.");
            }

            string? currentBalance = currentBalanceSpan[0].Text;
            if (string.IsNullOrWhiteSpace(currentBalance))
            {
                Logger.Error("Failed to retrieve the current balance.");
                return Result.Fail("Failed to retrieve the current balance.");
            }

            Logger.FunctionFinished();
            return Result.Ok(currentBalance);
        }

        private Result AggregateMoneyToPoints()
        {
            Logger.Info($"Initiating the process of money aggregation");


            Result clickingOnAggregateButtonResult = ClickingOnAggregateButton();
            if (clickingOnAggregateButtonResult.IsFailed)
            {
                return clickingOnAggregateButtonResult;
            }

            Result clickingOnContinueButtonResult = ClickingOnContinueButton(); //Tima
            if (clickingOnAggregateButtonResult.IsFailed)
            {
                return clickingOnContinueButtonResult;
            }

            Result clickingOnChargeButtonResult = ClickingOnChargeButton(); //Tima
            if (clickingOnChargeButtonResult.IsFailed)
            {
                return clickingOnChargeButtonResult;
            }


            Logger.Info($"The process of money aggregation has been successfully completed");
            return Result.Ok();
        }

        private Result StartTenBisWebsite()
        {
            try
            {
                Logger.FunctionStarted();

                m_WebDriver?.Manage().Window.Maximize();
                m_WebDriver?.Navigate().GoToUrl(TenBisUrl);

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
