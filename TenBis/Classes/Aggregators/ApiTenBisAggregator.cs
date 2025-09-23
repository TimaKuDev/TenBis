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
            if (api == null)
            {
                throw new ArgumentNullException(nameof(api), "API settings cannot be null.");
            }
            if (string.IsNullOrWhiteSpace(api.BaseUrl))
            {
                throw new ArgumentException("BaseUrl cannot be null or empty.", nameof(api.BaseUrl));
            }
            if (string.IsNullOrWhiteSpace(api.Cookie))
            {
                throw new ArgumentException("Cookie cannot be null or empty.", nameof(api.Cookie));
            }
            if (!api.TimeoutSeconds.HasValue)
            {
                throw new ArgumentException("TimeoutSeconds must have a value.", nameof(api.TimeoutSeconds));
            }

            m_Api = api;
            Logger.FunctionFinished();
        }


        async Task<Result<string>> IAggregator.Aggregate()
        {
            Logger.FunctionStarted();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Cookie", m_Api!.Cookie);
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await client.GetAsync($"{m_Api.BaseUrl}NextApi/GetUser");
            var content = await response.Content.ReadAsStringAsync();
            Logger.FunctionFinished();
            return Result.Ok();
        }
    }
}
