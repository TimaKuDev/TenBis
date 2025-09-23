using FluentResults;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using TenBis.Interfaces;
using TenBis.Logging;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Aggregators
{
    internal class FireFoxBrowserAggregator : BrowserAggregator, IAggregator
    {
        private readonly FirefoxDriverService m_FirefoxDriverService;
        private readonly FirefoxOptions m_FirefoxOptions;
        private readonly FirefoxDriver m_FirefoxDriver;

        public FireFoxBrowserAggregator(BrowserSettings browserSettings)
        {
            Logger.FunctionStarted();

            m_FirefoxDriverService = FirefoxDriverService.CreateDefaultService();

            m_FirefoxOptions = new FirefoxOptions();
            m_FirefoxOptions.AddArgument($"{UserDataDirPrefix}{browserSettings.UserProfilePath}");

            m_FirefoxDriver = new FirefoxDriver(m_FirefoxDriverService, m_FirefoxOptions);

            Logger.FunctionFinished();
        }

        Task<Result<string>> IAggregator.Aggregate()
        {
            Logger.FunctionStarted();

            Result<string> aggregateResult = Aggregate(m_FirefoxDriver);
            if (aggregateResult.IsFailed)
            {
                return Task.FromResult(aggregateResult);
            }

            Logger.FunctionFinished();
            return Task.FromResult(Result.Ok(aggregateResult.Value));
        }
    }
}