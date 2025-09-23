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

        public Result<string> Aggregate()
        {
            Logger.FunctionStarted();

            Result<string> aggregateResult = Aggregate(m_ChromeDriver);
            if (aggregateResult.IsFailed)
            {
                return aggregateResult;
            }

            Logger.FunctionFinished();
            return Result.Ok(aggregateResult.Value);
        }
    }
}