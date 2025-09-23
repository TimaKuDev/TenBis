using FluentResults;

namespace TenBis.Interfaces
{
    internal interface IAggregator
    {
        Task<Result<string>> Aggregate();
    }
}