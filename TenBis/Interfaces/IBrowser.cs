namespace TenBis.Interfaces
{
    internal interface IBrowser: IDisposable
    {
        public void StartTenBisWebsite();
        public void IsUserLoggedInValidation();
        public void AggregateMoneyToPoints();
        public string GetMessage();
    }
}
