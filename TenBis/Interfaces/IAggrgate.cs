namespace TenBis.Interfaces
{
    internal interface IAggrgate
    {
        internal bool TryAggrgate(out string? message);
    }
}