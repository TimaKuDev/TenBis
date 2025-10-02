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

        private async Task<Result> IsUserLoggedInAsync()
        {
            try
            {
                Logger.FunctionStarted();
                await Task.Delay(2000);

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

        private async Task<Result> ClickingOnAggregateButtonAsync()
        {
            Logger.FunctionStarted();
            await Task.Delay(millisecondsDelay: 1000);

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

        private async Task<Result> ClickingOnContinueButtonAsync()
        {
            Logger.FunctionStarted();
            await Task.Delay(millisecondsDelay: 1000);

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

        private async Task<Result> ClickingOnChargeButtonAsync()
        {
            Logger.FunctionStarted();
            await Task.Delay(1000);

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

                Task<Result> isUserLoggedInTask = IsUserLoggedInAsync();
                if (isUserLoggedInTask.Result.IsFailed)
                {
                    return isUserLoggedInTask.Result;
                }

                Result<string> aggregateMoneyToPointsResult = AggregateMoneyToPoints();
                if (aggregateMoneyToPointsResult.IsFailed)
                {
                    return aggregateMoneyToPointsResult;
                }

                Task<Result<string>> currentBalanceTask = GetCurrentBalanceAsync();
                if (currentBalanceTask.Result.IsFailed)
                {
                    return currentBalanceTask.Result;
                }

                string message = $"Aggregation completed successfully added: {aggregateMoneyToPointsResult.Value}, the current balance is: {currentBalanceTask.Result.Value}";

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

        private async Task<Result<string>> GetCurrentBalanceAsync()
        {
            Logger.FunctionStarted();
            await Task.Delay(1000);

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


            Task<Result> clickingOnAggregateButtonTask = ClickingOnAggregateButtonAsync();
            if (clickingOnAggregateButtonTask.Result.IsFailed)
            {
                return clickingOnAggregateButtonTask.Result;
            }

            Task<Result> clickingOnContinueButtonTask = ClickingOnContinueButtonAsync();
            if (clickingOnContinueButtonTask.Result.IsFailed)
            {
                return clickingOnContinueButtonTask.Result;
            }

            Task<Result<string>> inputAnmountTask = InputAnmountAsync();
            if (inputAnmountTask.Result.IsFailed)
            {
                return inputAnmountTask.Result;
            }

            Task<Result> clickingOnChargeButtonTask = ClickingOnChargeButtonAsync();
            if (clickingOnChargeButtonTask.Result.IsFailed)
            {
                return clickingOnChargeButtonTask.Result;
            }

            Task<Result> clickingOnCloseButtonTask = ClickingOnCloseButtonAsync();
            if (clickingOnCloseButtonTask.Result.IsFailed)
            {
                return clickingOnCloseButtonTask.Result;
            }


            Logger.Info($"The process of money aggregation has been successfully completed");
            return Result.Ok(inputAnmountTask.Result.Value);
        }

        private async Task<Result> ClickingOnCloseButtonAsync()
        {
            Logger.FunctionStarted();
            await Task.Delay(1000);

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

        private async Task<Result<string>> InputAnmountAsync()
        {
            Logger.FunctionStarted();
            await Task.Delay(millisecondsDelay: 1000);

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
