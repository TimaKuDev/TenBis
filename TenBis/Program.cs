using NLog;
using TenBis.Classes.Aggregation;
using TenBis.Factories;
using TenBis.Interfaces;
using TenBis.SettingsFolder;
using TenBis.SettingsFolder.Models;

internal class Program
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private static void Main(string[] args)
    {
        //Tima hide windowH
        int exitCode = RunScript();
        Environment.Exit(exitCode);
    }

    private static int RunScript()
    {
        string? message = null;
        ICommunication communcation = null;
        try
        {
            BrowserSettingsModel? browserSettingsModel = SettingsHelper.GetBrowserSettins();
            NotifySettingsModel? notifySettingsModel = SettingsHelper.GetNotifySettins();
            communcation = NotifierFactory.CreateNotifier(notifySettingsModel);
            bool? runScript = communcation?.ValidateRunningScript();
            if (!runScript.HasValue || !runScript.Value)
            {
                return 1;
            }

            return SeleniumTenBisAggregation.Aggrgate(browserSettingsModel, out message);
        }
        catch (Exception exception)
        {
            _logger.Error(exception.Message);
            return 1;
        }
        finally
        {
            communcation?.Notify(message);
        }
    }
}