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
using LauraChatManager.Types.Enums;
using LauraChatManager.Methods;

using WTelegram;
using TL;

using Error = LauraChatManager.Handles.Error;
using User = Telegram.Bot.Types.User;
using LauraChatManager.Handles.Other;

namespace LauraChatManager
{
    class Program
    {
        private static readonly TelegramBotClient bot = new(Config.Token);
        public static readonly Client Client = new(TApiConfig.ApiId, TApiConfig.ApiHash);

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
        public static Dictionary<long, bool> UsersRaw = new();
        public static List<long> ChatsRaw = new();

        public static Process Process;

        static async Task Main() {
            await Client.LoginBotIfNeeded(Config.Token);
            Client.CollectAccessHash = true;
            Helpers.Log = SkipApiLog;
            Client.OnUpdate += AccessHashReceiver;
            Console.Clear();
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
                    if(Users.Count > 0) {
                        for(int i = 0; i < Users.Count; i++) {
                            if(Users[i].Warns.Warns.Count > 0) {
                                for(int j = 0; j < Users[i].Warns.Warns.Count; j++) {
                                    for(int k = 0; k < Users[i].Warns.Warns.ElementAt(j).Value.Count; k++) {
                                        if(Users[i].Warns.Warns.ElementAt(j).Value[k].Expire <= DateTime.UtcNow) {
                                            await UsersDataTables.DeleteUserWarn(Users[i].Warns.Warns.ElementAt(j).Value[k].Id, Users[i].Id, Users[i].Warns.Warns.ElementAt(j).Key);
                                        }
                                    }
                                }
                            }
                            if(Users[i].BlockEnd.HasValue && Users[i].BlockEnd <= DateTime.UtcNow) {
                                LauraChatManager.Types.User new_user = Users[i];
                                new_user.BlockEnd = null;
                                new_user.IsBlocked = false;
                                await UsersDataTables.UpdateUser(new_user);
                            }
                            if(Users[i].RatingControls.Count > 0) {
                                for(int j = 0; i < Users[j].RatingControls.Count; j++) {
                                    if(Users[i].RatingControls[j].Expire <= DateTime.UtcNow) {
                                        Users[i].RatingControls.RemoveAt(j);
                                    }
                                }
                            }
                        }
                    }

                    if(Chats.Count > 0) {
                        for(int i = 0; i < Chats.Count; i++) {
                            if(Chats[i].Gmt != null) { 
                                switch(Chats[i].ChatState) {
                                    case ChatState.State:
                                    {
                                        var gmt = Parser.ParseGmt(Chats[i].Gmt);
                                        var time = Parser.ParseTime(Chats[i].Night);
                                        if(DateTime.UtcNow.AddHours(gmt.Hours).AddMinutes(gmt.Minutes).Hour >= time.Hour && DateTime.UtcNow.AddHours(gmt.Hours).AddMinutes(gmt.Minutes).Minute >= time.Minute) {
                                            LauraChatManager.Types.Chat new_chat = Chats[i];
                                            new_chat.ChatState = ChatState.Night;
                                            await ChatsDataTables.UpdateChatConfig(new_chat);
                                            await bot.SendTextMessageAsync(new_chat.Id, "test");
                                            await bot.SetChatPermissionsAsync(new_chat.Id, new() { CanSendMessages = false });
                                        }
                                        break;
                                    }
                                    case ChatState.Night:
                                    {
                                        var gmt = Parser.ParseGmt(Chats[i].Gmt);
                                        var time = Parser.ParseTime(Chats[i].State);
                                        if(DateTime.UtcNow.AddHours(gmt.Hours).AddMinutes(gmt.Minutes).Hour >= time.Hour && DateTime.UtcNow.AddHours(gmt.Hours).AddMinutes(gmt.Minutes).Minute >= time.Minute) {
                                            LauraChatManager.Types.Chat new_chat = Chats[i];
                                            new_chat.ChatState = ChatState.State;
                                            await ChatsDataTables.UpdateChatConfig(new_chat);
                                            await bot.SendTextMessageAsync(new_chat.Id, "test");
                                            await bot.SetChatPermissionsAsync(new_chat.Id, new() { CanSendMessages = true, CanSendOtherMessages = true, CanSendAudios = true, CanSendDocuments = true, CanSendPhotos = true, CanSendPolls = true, CanSendVideoNotes = true, CanSendVideos = true, CanSendVoiceNotes = true });
                                        }
                                        break;
                                    }
                                }
                            }
                            if(Chats[i].StExpire <= DateTime.UtcNow) {
                                var new_chat = Chats[i];
                                new_chat.StateOwner = 0;
                                new_chat.SettingsState = null;
                                for(int j = 0; j < Collections.TempConfigs.Count; j++) {
                                    if(Collections.TempConfigs[j].Id == new_chat.Id) {
                                        Collections.TempConfigs.RemoveAt(j);
                                        break;
                                    }
                                }
                                await ChatsDataTables.UpdateChatConfig(new_chat);
                            }
                        }
                    }
                    
                    if(Collections.CaptchaTemp.Count > 0) {
                        for(int i = 0; i < Collections.CaptchaTemp.Count; i++) {
                            for(int j = 0; j < Collections.CaptchaTemp.ElementAt(i).Value.Count; j++) {
                                if(Collections.CaptchaTemp.ElementAt(i).Value[j].Expire <= DateTime.UtcNow) {
                                    var chat = new Types.Chat();
                                    for(int k = 0; k < Program.Chats.Count; i++) {
                                        if(Program.Chats[i].Id == Program.ChatsRaw[i]) {
                                            chat = Program.Chats[i];
                                            break;
                                        }
                                    }
                                    await ChatsDataTables.DeleteUserCaptchaInTable(chat, Collections.CaptchaTemp.ElementAt(i).Value[j].UserId);
                                }
                            }
                        }
                    }
                    
                    if(Collections.RsRequests.Count > 0) {
                        for(int i = 0; i < 0; i++) {
                            if(Collections.RsRequests[i].Expire <= DateTime.UtcNow) {
                                await ReportStorageReqStorage.DeleteReportStorageRequest(Collections.RsRequests[i]);
                            }
                        }
                    }
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
        
        static async void SkipApiLog(int arg1, string arg2) {}
        
        static async Task AccessHashReceiver(IObject arg) {
            if (arg is not UpdatesBase updates) return;
            foreach(var update in updates.Chats) {
                var ach_hash = Client.GetAccessHashFor<Channel>(update.Value.ID);
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
