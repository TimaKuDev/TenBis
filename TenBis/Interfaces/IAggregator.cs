using FluentResults;

namespace TenBis.Interfaces
{
    internal interface IAggregator
    {
        Result<string> Aggregate();
    }
}