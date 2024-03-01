using NLog;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace TenBis.Classes.Browser
{
    internal static class TenBisBrowser
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private static int _amountOfTries;
        static TenBisBrowser()
        {
            _amountOfTries = 0;
        }

        internal static bool TryClickingOnAggregateButton(ReadOnlyCollection<IWebElement> aggregateButton)
        {
            Task.Delay(1000).Wait();
            bool? isAggregateButtonEnabled = aggregateButton?[0].Enabled;
            if (!isAggregateButtonEnabled.HasValue || !isAggregateButtonEnabled.Value)
            {
                _logger.Info($"{Helper.GetCurrentMethod()}: The aggregation process failed because the ‘Load Card’ button was disabled");
                return false;
            }

            aggregateButton?[0].Click();
            return true;
        }

        internal static bool TryClickingOnContinueButton(ReadOnlyCollection<IWebElement> continueButton)
        {
            Task.Delay(1000).Wait();
            bool? isContinueButtonEnabled = null;
            try
            {
                isContinueButtonEnabled = continueButton?[0].Enabled;

            }
            catch (Exception exception)
            {

            }
            if (!isContinueButtonEnabled.HasValue || !isContinueButtonEnabled.Value)
            {
                _logger.Info($"{Helper.GetCurrentMethod()}: The aggregation process failed because the ‘Continue’ button was disabled");
                return false;
            }

            continueButton?[0].Click();
            return true;
        }

        internal static bool TryClickingOnChargeButton(ReadOnlyCollection<IWebElement> chargeCardButton)
        {
            Task.Delay(1000).Wait();
            bool? isChargeCardButtonEnabled = chargeCardButton?[0].Enabled;
            if (!isChargeCardButtonEnabled.HasValue || !isChargeCardButtonEnabled.Value)
            {
                _logger.Info($"{Helper.GetCurrentMethod()}: The aggregation process failed because the ‘Charge Card’ button was disabled");
                return false;
            }

            chargeCardButton?[0].Click();
            return true;
        }

        internal static string GetMessage(bool aggregatedSuccessfully, ReadOnlyCollection<IWebElement> currentBalanceSpan)
        {
            Task.Delay(1000).Wait();

            string updateStatus = aggregatedSuccessfully ? "The script successfully aggregated money to points" : "The script failed to aggregate money to points";
            string? currentBalance = string.IsNullOrEmpty(currentBalanceSpan[0].Text) ? null : $"As of {DateTime.Now:dd-MM-yyyy}, you have {currentBalanceSpan[0].Text} points";
            return @$"{updateStatus}
{currentBalance}";
        }

        internal static bool IsUserLoggedIn(ReadOnlyCollection<IWebElement> dropDownImage)
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the process of log-in validation.");
            bool isUserLoggedIn = dropDownImage.Count != 0;
            if (!isUserLoggedIn)
            {
                RetryUserLoggedInValidation();
            }

            return isUserLoggedIn;
        }

        internal static void RetryUserLoggedInValidation()
        {
            Task.Delay(28000).Wait();
            if (_amountOfTries == 3)
            {
                throw new Exception($"The maximum number of validation attempts, which is {_amountOfTries}, has been reached");
            }

            _logger.Info($"{Helper.GetCurrentMethod()}: Log-in validation Retrying attempt: {_amountOfTries + 1}");
            _amountOfTries++;
        }
    }
}
