using System;
using System.Diagnostics;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

using Config = LauraChatManager.Configuration.Config;
using TApiConfig = LauraChatManager.Configuration.TApiConfig;
using LauraChatManager.Types;

using WTelegram;
using TL;

using Error = LauraChatManager.Handles.Error;
using User = Telegram.Bot.Types.User;


namespace LauraChatManager
{
    class Program
    {
        private static readonly TelegramBotClient bot = new(Config.Token);
        private static readonly Client client = new(TApiConfig.ApiId, TApiConfig.ApiHash);

        public static bool HasException = false;
        public static readonly ReceiverOptions Options = new() { AllowedUpdates = new UpdateType[] { UpdateType.Message, UpdateType.ChannelPost, UpdateType.CallbackQuery }};
        public static User Bot;
        public static Dictionary<long, long> ChannelBase = new();
        public static DateTime Started = DateTime.UtcNow;
        public static Collections Collections;

        public static long Ping;
        public static long DbPing;
        public static long RamUsage;

        public static DateTime NextLarxUpdate;

        public static List<LauraChatManager.Types.User> Users = new();
        public static List<LauraChatManager.Types.Chat> Chats = new();

        public static Process Process;

        static async Task Main() {
            client.OnUpdate += AccessHashReceiver;
            Console.WriteLine("Creating logs...");
            await Loader.CreateLogs();
            await WriteDebbug("Loading Collections for boost from Databases...");
            Collections = await Loader.GetCollections();
            bot.StartReceiving(LauraChatManager.Handles.Updates.Receiver, Error.Handler, Options);
            Bot = await bot.GetMeAsync();
            Started = DateTime.UtcNow;
            Console.WriteLine(@$"Bot started in {Started.ToString(Config.StandartDateFormat, CultureInfo.InvariantCulture)} UTC

Bot info:
Id - {Bot.Id}
Name - {Bot.FirstName}
Username - {Bot.Username}");
            Process = Process.GetCurrentProcess();
            RamUsage = Process.WorkingSet64/1024/1024;
            await WriteLarx($"RAM Usage: {RamUsage}");
            Ping = new System.Net.NetworkInformation.Ping().Send("api.telegram.org").RoundtripTime;
            await WriteLarx($"Ping: {Ping}");
            DbPing = new System.Net.NetworkInformation.Ping().Send("45.89.52.134").RoundtripTime;
            await WriteLarx($"Ping DataBase: {DbPing}");

            while(true) {
                NextLarxUpdate = DateTime.UtcNow.AddHours(1);
                while((NextLarxUpdate - DateTime.UtcNow).TotalMinutes > 0) {
                    
                }
                Process = Process.GetCurrentProcess();
                RamUsage = Process.WorkingSet64/1024/1024;
                await WriteLarx($"RAM Usage: {RamUsage}");
                Ping = new System.Net.NetworkInformation.Ping().Send("api.telegram.org").RoundtripTime;
                await WriteLarx($"Ping: {Ping}");
                DbPing = new System.Net.NetworkInformation.Ping().Send("45.89.52.134").RoundtripTime;
                await WriteLarx($"Ping DataBase: {DbPing}");
            }
        }

        static async Task AccessHashReceiver(IObject arg) {
            if (arg is not UpdatesBase updates) return;
            foreach(var update in updates.Chats) {
                var ach_hash = client.GetAccessHashFor<Channel>(update.Value.ID);
                if(!ChannelBase.ContainsKey(update.Value.ID) && ach_hash != 0) {
                    ChannelBase.Add(update.Value.ID, ach_hash);
                    await WriteDebbug($"Collected Access Hash '{ach_hash}' for Supergroup ID{update.Value.ID}(Telegram API Format)");
                }
            }
        }

        public static async Task WriteError(string text) {
            var log = DateTime.UtcNow.ToString(Config.StandartDateFormat, CultureInfo.InvariantCulture) + " [ERROR]: " + text;
            await Loader.UpdateErrors(log);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static async Task WriteLarx(string text) {
            var log = DateTime.UtcNow.ToString(Config.StandartDateFormat, CultureInfo.InvariantCulture) + " [LARX]: " + text;
            await Loader.UpdateLarx(log);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static async Task WriteDebbug(string text) {
            var log = DateTime.UtcNow.ToString(Config.StandartDateFormat, CultureInfo.InvariantCulture) + " [DEBUG]: " + text;
            await Loader.UpdateDebugs(log);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(log);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
