using NLog;
using TenBis.Classes;

internal class Program
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private static void Main(string[] args)
    {
        int exitCode = aggregate10BisMoneyToPoints();
        Environment.Exit(exitCode);
    }

    private static int aggregate10BisMoneyToPoints()
    {
        TenBisWebsite tenBisWebsite = null;
        try
        {
            tenBisWebsite = new TenBisWebsite(UserPath.GetChromePath());
            tenBisWebsite.StartTenBisWebsite();
            tenBisWebsite.ValidateUserLoggedIn();
            tenBisWebsite.AggregateMoneyToPoints();
            return 0;
        }
        catch(Exception exception)
        {
            _logger.Error(exception.Message);
            return 1;
        }
        finally
        {
            tenBisWebsite?.Dispose();
        }
    }
}