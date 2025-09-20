using FluentResults;
using TenBis.Classes.Aggregators;
using TenBis.Enum;
using TenBis.Interfaces;
using TenBis.Logging;
using TenBis.SettingsFolder.Models;

namespace TenBis.Factories
{
    internal static class AggregateFactory
    {
        internal static Result<IAggregator?> Create(AggregationSettings aggregationSettings)
        {
            Logger.FunctionStarted();

            if (aggregationSettings is null)
            {
                return Result.Fail<IAggregator?>("Aggregation settings are missing.");
            }

            Logger.FunctionFinished();
            return aggregationSettings.Aggregation switch
            {
                Aggregation.Browser => BrowserFactory.Create(aggregationSettings.Browser),
                Aggregation.Api => Result.Ok<IAggregator?>(new ApiTenBisAggregator(aggregationSettings.Api)),
                _ => Result.Fail<IAggregator?>($"Unsupported aggregation type: {aggregationSettings.Aggregation}.")
            };
        }
    }
}