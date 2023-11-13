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
            using StreamReader reader = new StreamReader("ChromeUserPath.txt");
            string chromeUserPath = reader.ReadLine();
            tenBisWebsite = new TenBisWebsite(chromeUserPath);
            tenBisWebsite.StartTenBisWebsite();
            tenBisWebsite.ValidateUserLoggedIn();
            tenBisWebsite.AggregateMoneyToPoints();
        }
        catch(Exception exception)
        {

        }
        finally
        {
            tenBisWebsite?.Dispose();
        }
    }
}