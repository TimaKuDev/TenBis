using NLog;
using TenBis.Classes;

internal static class Program
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    public static void Main(string[] args)
    {
        _logger.Info($"Starting {Helper.GetCurrentMethod()}");
        TenBisScript.RunScript();
        Environment.Exit(1);
    }
}