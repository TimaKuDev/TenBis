using FluentResults;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TenBis.Classes;
using TenBis.Logging;
using TenBis.SettingsFolder.Models;

namespace TenBis.SettingsFolder
{
    internal static class SettingsHelper
    {
        public static Result<AggregationSettings?> GetAggregationSettings()
        {
            try
            {
                Logger.FunctionStarted();

                using StreamReader reader = new(path: "AggregationSettings.js");
                string aggregationSettingsValue = reader.ReadToEnd();
                AggregationSettings? aggregationSettings = JsonConvert.DeserializeObject<AggregationSettings?>(aggregationSettingsValue);

                Logger.FunctionFinished();
                Logger.Info($"{Helper.GetCurrentMethod()}: Finished, getting aggregation settings");
                return Result.Ok(aggregationSettings);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                return Result.Fail<AggregationSettings?>(exception.Message);
            }
        }

        public static Result<CommunicationSettings?> GetCommunicationSettings()
        {
            try
            {
                Logger.FunctionStarted();

                using StreamReader reader = new(path: "CommunicationSettings.js");
                string communicationSettingsValue = reader.ReadToEnd();
                CommunicationSettings? communicationSettings = JsonConvert.DeserializeObject<CommunicationSettings?>(communicationSettingsValue);

                Logger.FunctionFinished();
                return Result.Ok(communicationSettings);
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                return Result.Fail<CommunicationSettings?>(exception.Message);
            }
        }
    }
}
