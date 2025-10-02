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
        private const string AmountElement = @"//div[@class=""Amount-ebPevQ dCjGSF""]";
        private const string ContinueButtonElement = @"//button[@class=""Button-dtEEMF SubmitButton-etwBPp iXOfjm hHtDlm""]";
        private const string ChargeCardButtonElement = @"//button[@class=""Button-dtEEMF SubmitButton-etwBPp iXOfjm hHtDlm""]";
        private const string CurrentBalanceSpanElement = @"//span[@class=""Balance-gcvUbW ewtkSs""]";
        private const string CloseButtonElement = @"//button[@class=""Button-dtEEMF SubmitButton-etwBPp iXOfjm hHtDlm""]";
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

                Result<string> aggregateMoneyToPointsResult = AggregateMoneyToPoints();
                if (aggregateMoneyToPointsResult.IsFailed)
                {
                    return aggregateMoneyToPointsResult;
                }

                Result<string> currentBalanceResult = GetCurrentBalance();
                if (currentBalanceResult.IsFailed)
                {
                    return currentBalanceResult;
                }

                string message = $"Aggregation completed successfully added: {aggregateMoneyToPointsResult.Value}, the current balance is: {currentBalanceResult.Value}";

                Logger.FunctionFinished();
                return Result.Ok(message);

            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                return new Result<string>().WithError(exception.Message);
            }
            finally
            {
                m_WebDriver?.Quit();
                m_WebDriver?.Dispose();
                Logger.Info("The browser has been closed and resources have been released.");
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

        private Result<string> AggregateMoneyToPoints()
        {
            Logger.Info($"Initiating the process of money aggregation");


            Result clickingOnAggregateButtonResult = ClickingOnAggregateButton();
            if (clickingOnAggregateButtonResult.IsFailed)
            {
                return clickingOnAggregateButtonResult;
            }

            Result clickingOnContinueButtonResult = ClickingOnContinueButton();
            if (clickingOnAggregateButtonResult.IsFailed)
            {
                return clickingOnContinueButtonResult;
            }

            Result<string> inputAnmountResult = InputAnmount();
            if (inputAnmountResult.IsFailed)
            {
                return inputAnmountResult;
            }

            Result clickingOnChargeButtonResult = ClickingOnChargeButton();
            if (clickingOnChargeButtonResult.IsFailed)
            {
                return clickingOnChargeButtonResult;
            }

            Result clickingOnCloseButtonResult = ClickingOnCloseButton();
            if (clickingOnCloseButtonResult.IsFailed)
            {
                return clickingOnCloseButtonResult;
            }


            Logger.Info($"The process of money aggregation has been successfully completed");
            return Result.Ok(inputAnmountResult.Value);
        }

        private Result ClickingOnCloseButton()
        {
            Logger.FunctionStarted();
            Task.Delay(1000);

            ReadOnlyCollection<IWebElement>? closeCardButton = m_WebDriver?.FindElements(By.XPath(CloseButtonElement));
            bool? isCloseCardButtonEnabled = closeCardButton?[0].Enabled;
            if (!isCloseCardButtonEnabled.HasValue || !isCloseCardButtonEnabled.Value)
            {
                Logger.Error("The aggregation process failed because the ‘Charge Card’ button was disabled");
                return Result.Fail("The aggregation process failed because the ‘Charge Card’ button was disabled");
            }

            closeCardButton?[0].Click();

            Logger.FunctionFinished();
            return Result.Ok();
        }

        private Result<string> InputAnmount()
        {
            Logger.FunctionStarted();
            Task.Delay(millisecondsDelay: 1000);

            ReadOnlyCollection<IWebElement>? inputAmountElement = m_WebDriver?.FindElements(By.XPath(AmountElement));
            if (inputAmountElement is null)
            {
                Logger.Error("Failed to retrieve the current input amount.");
                return Result.Fail("Failed to retrieve the current input amount.");
            }

            string? inputAmount = inputAmountElement[0].Text;
            if (string.IsNullOrWhiteSpace(inputAmount))
            {
                Logger.Error("Failed to retrieve the current input amount.");
                return Result.Fail("Failed to retrieve the current input amount.");
            }

            Logger.FunctionFinished();
            return Result.Ok(inputAmount);
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
