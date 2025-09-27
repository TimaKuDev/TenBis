using FluentResults;
using OpenQA.Selenium.Edge;
using TenBis.Interfaces;
using TenBis.Logging;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Aggregators
{
    internal class EdgeBrowserAggregator : BrowserAggregator, IAggregator
    {
        public EdgeBrowserAggregator(BrowserSettings browserSettings)
        {
            Logger.FunctionStarted();

            EdgeDriverService edgeDriverService = EdgeDriverService.CreateDefaultService();

            EdgeOptions edgeOptions = new();
            edgeOptions.AddArgument($"{UserDataDirPrefix}{browserSettings.UserProfilePath}");

            EdgeDriver edgeDriver = new(edgeDriverService, edgeOptions);

            m_WebDriver = edgeDriver;

            Logger.FunctionStarted();
        }

        Task<Result<string>> IAggregator.Aggregate()
        {
            Logger.FunctionStarted();

            Result<string> aggregateResult = Aggregate();
            if (aggregateResult.IsFailed)
            {
                return Task.FromResult(aggregateResult);
            }

            Logger.FunctionFinished();
            return Task.FromResult(Result.Ok(aggregateResult.Value));
        }
    }
}