namespace TenBis.Interfaces
{
    internal interface IBrowser : IDisposable
    {
        public void StartTenBisWebsite();
        public void ValidateUserLoggedIn();
        public bool TryAggregateMoneyToPoints();
        string GetCurrentPointsAmount();
    }
}
