using FluentResults;
using TenBis.Interfaces;
using TenBis.Logging;
using TenBis.SettingsFolder.Models;

namespace TenBis.Classes.Aggregators
{
    internal class ApiTenBisAggregator : IAggregator
    {
        private readonly ApiSettings? m_Api;

        public ApiTenBisAggregator(ApiSettings? api)
        {
            Logger.FunctionStarted();
            m_Api = api;
            Logger.FunctionFinished();
        }


        Result<string> IAggregator.Aggregate()
        {
            Logger.FunctionStarted();
            Logger.FunctionFinished();
            throw new NotImplementedException();
        }
    }
}
