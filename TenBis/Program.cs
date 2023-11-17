using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;
using NLog;
using TenBis.Factories;
using TenBis.Interfaces;
using TenBis.SettingsFolder;

internal class Program
{
    private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();
    private static void Main(string[] args)
    {
        //Tima hide windowH
        int exitCode = aggregate10BisMoneyToPoints();
        Environment.Exit(exitCode);
    }

    private static int aggregate10BisMoneyToPoints()
    {
        string currentBalanceAmount = null;
        bool? isSuccessfullyAggregation = false;
        IBrowser browser = null;
        SettingsModel settingsModel = null;
        try
        {
            settingsModel = Settings.GetSettins();
            browser = BrowserFactory.CreateBrowser(settingsModel.BrowserType, settingsModel.UserProfilePath);
            browser?.StartTenBisWebsite();
            browser?.ValidateUserLoggedIn();
            isSuccessfullyAggregation = browser?.TryAggregateMoneyToPoints();
            currentBalanceAmount = browser.GetCurrentPointsAmount();
            
            return 0;
        }
        catch (Exception exception)
        {
            _logger.Error(exception.Message);
            return 1;
        }
        finally
        {
            browser?.Dispose();
            INotifier notify = NotifierFactory.CreateNotifier(settingsModel.NotifyType, settingsModel.NotifyTo, currentBalanceAmount, isSuccessfullyAggregation: isSuccessfullyAggregation);
            notify?.Notify();
        }
    }
}