using FluentResults;
using OpenQA.Selenium.Chrome;
using TenBis.Interfaces;
using TenBis.Logging;

namespace TenBis.Classes.Aggregators
{
    internal class ChromeBrowserAggregator : BrowserAggregator, IAggregator
    {

        private readonly ChromeDriverService m_ChromeDriverService;
        private readonly ChromeOptions m_ChromeOptions;
        private readonly ChromeDriver m_ChromeDriver;

        public ChromeBrowserAggregator(SettingsFolder.Models.BrowserSettings browserSettings)
        {
            Logger.FunctionStarted();


            m_ChromeDriverService = ChromeDriverService.CreateDefaultService();
            m_ChromeOptions = new ChromeOptions();

            m_ChromeOptions.AddArgument($"{UserDataDirPrefix}{browserSettings.UserProfilePath}");

            m_ChromeDriver = new ChromeDriver(m_ChromeDriverService, m_ChromeOptions);

            Logger.FunctionFinished();
        }

        Task<Result<string>> IAggregator.Aggregate()
        {
            Logger.FunctionStarted();

            Result<string> aggregateResult = Aggregate(m_ChromeDriver);
            if (aggregateResult.IsFailed)
            {
                return Task.FromResult(aggregateResult);
            }

            Logger.FunctionFinished();
            return Task.FromResult(Result.Ok(aggregateResult.Value));
        }
    }
}