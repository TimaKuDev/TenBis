using FluentResults;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using TenBis.Interfaces;
using TenBis.Logging;

namespace TenBis.Classes.Aggregators
{
    internal class EdgeBrowserAggregator : BrowserAggregator, IAggregator
    {
        private readonly EdgeDriverService m_EdgeDriverService;
        private readonly EdgeOptions m_EdgeOptions;
        private readonly EdgeDriver m_EdgeDriver;

        public EdgeBrowserAggregator(SettingsFolder.Models.BrowserSettings browserSettings)
        {
            Logger.FunctionStarted();

            m_EdgeDriverService = EdgeDriverService.CreateDefaultService();

            m_EdgeOptions = new EdgeOptions();
            m_EdgeOptions.AddArgument($"{UserDataDirPrefix}{browserSettings.UserProfilePath}");

            m_EdgeDriver = new EdgeDriver(m_EdgeDriverService, m_EdgeOptions);

            Logger.FunctionStarted();
        }

        Task<Result<string>> IAggregator.Aggregate()
        {
            Logger.FunctionStarted();

            Result<string> aggregateResult = Aggregate(m_EdgeDriver);
            if (aggregateResult.IsFailed)
            {
                return Task.FromResult(aggregateResult);
            }

            Logger.FunctionFinished();
            return Task.FromResult(Result.Ok(aggregateResult.Value));
        }
    }
}