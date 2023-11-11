using TenBis.Classes;

internal class Program
{
    private static void Main(string[] args)
    {
        aggregate10BisMoneyToPoints();
        Environment.Exit(0);
    }

    private static void aggregate10BisMoneyToPoints()
    {
        TenBisWebsite tenBisWebsite = null;

        try
        {
            tenBisWebsite = new TenBisWebsite();
            tenBisWebsite.StartTenBisWebsite();
            tenBisWebsite.ValidateUserLoggedIn();
            tenBisWebsite.AggregateMoneyToPoints();
        }
        finally
        {
            tenBisWebsite?.Dispose();
        }
    }
}