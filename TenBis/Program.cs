using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TenBis.Classes;
using TenBis.Enum;
using TenBis.SettingsFolder.Models;

internal static class Program
{
    public static async Task Main(string[] args)
     {

        //TelegramBotClient  m_TelegramBotClient = new TelegramBotClient("7192574119:AAGyMShywFK2tZXVe0A3dATY3zXuWOD-eBc");
        //InlineKeyboardMarkup reply = new(new[]
        //  {
        //        new[]
        //        {
        //            InlineKeyboardButton.WithCallbackData(text : "Yes", bool.TrueString),
        //            InlineKeyboardButton.WithCallbackData(text : "No", bool.FalseString)
        //        }
        //    });

        //await m_TelegramBotClient.SendMessage(1089050732, text: "Would you like to aggregate your money to points?", replyMarkup: reply);


        ExitCode exitCode = await TenBisScript.Execute();
        Environment.Exit((int)exitCode);
    }
}