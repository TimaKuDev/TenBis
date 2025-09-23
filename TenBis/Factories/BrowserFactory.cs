using FluentResults;
using TenBis.Classes.Aggregators;
using TenBis.Enum;
using TenBis.Interfaces;
using TenBis.SettingsFolder.Models;

namespace TenBis.Factories
{
    internal static class BrowserFactory
    {
        internal static Result<IAggregator?> Create(BrowserSettings? browser)
        {
            if (browser is null)
            {
                return Result.Fail<IAggregator?>("User profile path is missing or empty.");
            }

            return browser.BrowserType switch
            {
                Browser.Chrome => Result.Ok<IAggregator?>(new ChromeBrowserAggregator(browser)),
                Browser.Edge => Result.Ok<IAggregator?>(new EdgeBrowserAggregator(browser)),
                Browser.FireFox => Result.Ok<IAggregator?>(new FireFoxBrowserAggregator(browser)),
                _ => Result.Fail<IAggregator?>($"Unsupported browser type: {browser.BrowserType}")
            };
        }
    }
}
