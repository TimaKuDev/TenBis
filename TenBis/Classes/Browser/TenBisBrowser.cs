﻿using NLog;
using OpenQA.Selenium;
using System.Collections.ObjectModel;

namespace TenBis.Classes.Browser
{
    internal abstract class TenBisBrowser
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        protected ReadOnlyCollection<IWebElement> _aggregateButton;
        protected ReadOnlyCollection<IWebElement> _continueButton;
        protected ReadOnlyCollection<IWebElement> _chargeCardButton;
        protected ReadOnlyCollection<IWebElement> _dropDownImage;
        protected ReadOnlyCollection<IWebElement> _currentBalanceSpan;
        protected int _amountOfTries;
        public TenBisBrowser()
        {
            _amountOfTries = 0;
        }

        protected bool TryClickingOnAggregateButton()
        {
            Task.Delay(1000).Wait();
            if (_aggregateButton?[0].Enabled != true)
            {
                _logger.Info($"{Helper.GetCurrentMethod()}: The aggregation process failed because the ‘Load Card’ button was disabled");
                return false;
            }

            _aggregateButton[0].Click();
            return true;
        }

        protected bool TryClickingOnContinueButton()
        {
            Task.Delay(1000).Wait();
            if (_continueButton?[0].Enabled != true)
            {
                _logger.Info($"{Helper.GetCurrentMethod()}: The aggregation process failed because the ‘Continue’ button was disabled");
                return false;
            }

            _continueButton[0].Click();
            return true;
        }

        protected bool TryClickingOnChargeButton()
        {
            Task.Delay(1000).Wait();
            if (_chargeCardButton?[0].Enabled != true)
            {
                _logger.Info($"{Helper.GetCurrentMethod()}: The aggregation process failed because the ‘Charge Card’ button was disabled");
                return false;
            }

            _chargeCardButton[0].Click();
            Task.Delay(1000).Wait();
            return true;
        }


        protected string GetCurrentPointsAmount()
        {
            Task.Delay(1000).Wait();
            return _currentBalanceSpan[0].Text;
        }

        protected void ValidateUserLoggedIn()
        {
            _logger.Info($"{Helper.GetCurrentMethod()}: Initiating the process of log-in validation.");
            Task.Delay(2000).Wait();
            if (_dropDownImage.Count == 0)
            {
                RetryUserLoggedInValidation();
            }
        }

        private void RetryUserLoggedInValidation()
        {
            Task.Delay(28000).Wait();
            if (_amountOfTries == 3)
            {
                throw new Exception($"The maximum number of validation attempts, which is {_amountOfTries}, has been reached");
            }

            _logger.Info($"{Helper.GetCurrentMethod()}: Log-in validation Retrying attempt: {_amountOfTries + 1}");
            _amountOfTries++;
            ValidateUserLoggedIn();
        }
    }
}
