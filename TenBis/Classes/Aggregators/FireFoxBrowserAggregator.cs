using FluentResults;
using OpenQA.Selenium.Firefox;
using TenBis.Interfaces;
using TenBis.Logging;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Aggregators
{
    internal class FireFoxBrowserAggregator : BrowserAggregator, IAggregator
    {
        public FireFoxBrowserAggregator(BrowserSettings browserSettings)
        {
            Logger.FunctionStarted();

            FirefoxDriverService firefoxDriverService = FirefoxDriverService.CreateDefaultService();

            FirefoxOptions firefoxOptions = new();
            firefoxOptions.AddArgument($"{UserDataDirPrefix}{browserSettings.UserProfilePath}");

            FirefoxDriver firefoxDriver = new(firefoxDriverService, firefoxOptions);

            m_WebDriver = firefoxDriver;

            Logger.FunctionFinished();
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