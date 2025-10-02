using FluentResults;
using OpenQA.Selenium.Chrome;
using TenBis.Interfaces;
using TenBis.Logging;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Aggregators
{
    internal class ChromeBrowserAggregator : BrowserAggregator, IAggregator
    {
        public ChromeBrowserAggregator(BrowserSettings browserSettings)
        {
            Logger.FunctionStarted();

            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();

            ChromeOptions chromeOptions = new();
            chromeOptions.AddArgument($"{UserDataDirPrefix}{browserSettings.UserProfilePath}");

            ChromeDriver chromeDriver = new(chromeDriverService, chromeOptions);

            m_WebDriver = chromeDriver;

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