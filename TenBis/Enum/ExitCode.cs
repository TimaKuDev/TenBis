namespace TenBis.Enum
{
    internal enum ExitCode
    {
        Success = 0,
        GenericError = 1,
        AggregationSettingsFailure = 2,
        AggregationFailure = 3,
        AggregationFactoryFailure = 4,
        CommunicationSettingsFailure = 5,
        CommunicationFailure = 6,
        CommunicationFactoryFailure = 7,
        Timeout = 8,
        UserCancelled = 9
    }
}
