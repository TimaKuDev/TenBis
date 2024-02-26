using NLog;
using TenBis.Classes;
using TenBis.Classes.Aggregation;
using TenBis.Factories;
using TenBis.Interfaces;
using TenBis.SettingsFolder;
using TenBis.SettingsFolder.Models;

internal class Program
{
    
    private static void Main(string[] args)
    {
        //Tima hide windowH
        int exitCode = TenBisScript.RunScript();
        Console.Read();
        Environment.Exit(exitCode);
    }
}