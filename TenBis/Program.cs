using TenBis.Classes;
using TenBis.Enum;

internal static class Program
{
    public static async Task Main(string[] args)
    {

        ExitCode exitCode = await TenBisScript.Execute();
        Environment.Exit((int)exitCode);
    }
}