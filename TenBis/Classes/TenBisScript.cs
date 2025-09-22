using FluentResults;
using TenBis.Enum;
using TenBis.Factories;
using TenBis.Interfaces;
using TenBis.Logging;
using TenBis.SettingsFolder;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes
{
    internal static class TenBisScript
    {
        public static async Task<ExitCode> Execute()
        {
            ICommunicator? communicator = null;
            try
            {
                Logger.FunctionStarted();

                Result<CommunicationSettings?> communicationSettingsResult = SettingsHelper.GetCommunicationSettings();
                if (communicationSettingsResult.IsFailed)
                {
                    return ExitCode.CommunicationSettingsFailure;
                }

                CommunicationSettings communicationSettings = communicationSettingsResult.Value!;
                Result<ICommunicator?> communcationResult = CommunicationFactory.Create(communicationSettings);
                if (communcationResult.IsFailed)
                {
                    return ExitCode.CommunicationFactoryFailure;
                }

                communicator = communcationResult.Value!;

                Result<bool> taskValidateResult = await communicator.SendValidationMessage();
                if (taskValidateResult.IsFailed)
                {
                    string errors = string.Join(Environment.NewLine, taskValidateResult.Errors);
                    Result sendMessageResult = await communicator.SendMessage(errors);

                    return ExitCode.CommunicationFailure;
                }

                bool isValidationValid = taskValidateResult.Value;
                if (!isValidationValid)
                {
                    Result sendMessageResult = await communicator.SendMessage("Choosed not to aggregate money");
                    if (sendMessageResult.IsFailed)
                    {
                        return ExitCode.CommunicationFailure;
                    }

                    return ExitCode.UserCancelled;
                }

                Result<AggregationSettings?> aggregationSettingsResult = SettingsHelper.GetAggregationSettings();
                if (aggregationSettingsResult.IsFailed)
                {
                    return ExitCode.AggregationSettingsFailure;
                }

                AggregationSettings aggregationSettings = aggregationSettingsResult.Value!;
                Result<IAggregator?> aggregateResult = AggregateFactory.Create(aggregationSettings);
                if (communcationResult.IsFailed)
                {
                    return ExitCode.AggregationFactoryFailure;
                }

                IAggregator aggregator = aggregateResult.Value!;

                Result<string> aggregatorResult = aggregator.Aggregate();
                if (aggregatorResult.IsFailed)
                {
                    string errors = string.Join(Environment.NewLine, aggregatorResult.Errors);
                    Result sendMessageResult = await communicator.SendMessage(errors);
                    if (sendMessageResult.IsFailed)
                    {
                        return ExitCode.CommunicationFailure;
                    }

                    return ExitCode.AggregationFailure;
                }

                string message = aggregatorResult.Value;
                await communicator.SendMessage(message);

                Logger.FunctionFinished();
                return ExitCode.Success;
            }
            catch (Exception exception)
            {
                await communicator?.SendMessage("There was an error while trying to run the script, check the logs files.");
                Logger.Error(exception.Message);
                return ExitCode.GenericError;
            }
        }
    }
}