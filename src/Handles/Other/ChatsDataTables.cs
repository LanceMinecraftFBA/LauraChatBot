using System.Data;
using System.Globalization;
using LauraChatManager.Configuration;
using LauraChatManager.Types;
using LauraChatManager.Types.Enums;
using MySql.Data.MySqlClient;

namespace LauraChatManager.Handles.Other {
    public class ChatsDataTables {
        private static string _values_captcha = @"(
id INT PRIMARY KEY AUTO_INCREMENT,
userId VARCHAR(255) NOT NULL,
expire VARCHAR(255) NOT NULL,
attemps INT NOT NULL,
type VARCHAR(255) NOT NULL,
solution VARCHAR(255) NOT NULL
)";
        private static string _values_reports = @"(
id INT PRIMARY KEY AUTO_INCREMENT,
target_user_id VARCHAR(255) NOT NULL,
output_user_id VARCHAR(255) NOT NULL,
message_id VARCHAR(255) NOT NULL,
reason VARCHAR(200) DEFAULT('None') NOT NULL,
stamp VARCHAR(255) NOT NULL,
is_admin TINYINT NOT NULL
)";
        private static string _default = "DEFAULT";

        public static async Task<List<Captcha>?> GetUsersCaptcha(long chatId) {
            var list = new List<Captcha>();
            using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
                try {
                    await mySql.OpenAsync();
                    var query = $"SELECT * FROM {chatId*-1}_captchas";
                    using(MySqlCommand myCmd = new(query, mySql)) {
                        using(MySqlDataReader myReader = (MySqlDataReader)await myCmd.ExecuteReaderAsync()) {
                            if(!myReader.HasRows)
                            {
                                await mySql.CloseAsync();
                                list = null;
                                await Program.WriteDebbug("List<Captcha> is null");
                            }
                            else
                            {
                                while(myReader.Read()) {
                                    var temp = new Captcha()
                                    {
                                        UserId = long.Parse(myReader[1].ToString()),
                                        Expire = DateTime.ParseExact(myReader[2].ToString(), Config.StandartDateFormat, CultureInfo.InvariantCulture),
                                        Attemps = int.Parse(myReader[3].ToString()),
                                        Type = (CaptchaType)Enum.Parse(typeof(CaptchaType), myReader[4].ToString())
                                    };

                                    if(myReader[5].ToString() != "None")
                                        temp.Solution = myReader[5].ToString();
                                    list.Add(temp);
                                }
                                await mySql.CloseAsync();
                                await Program.WriteDebbug("List<Captcha> was returned");
                            }
                        }
                    }
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally{
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
            return list;
        }
        public static async Task<bool> CreateUsersCaptchaTable(long chatId) {
            var oper = false;
            using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
                try {
                    await mySql.OpenAsync();
                    var query = $"CREATE TABLE {chatId*-1}_captchas{_values_captcha}";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    Program.Collections.CaptchaTemp.Add(chatId, new());
                    oper = true;
                    await Program.WriteDebbug($"Table {chatId}_captchas was created");
                }
                catch(MySqlException exc) {
                    oper = false;
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
            return oper;
        }
        public static async Task<bool> InsertUserCaptchaInTable(Chat chat, long userId) {
            var oper = false;
            using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
                var sol_val = "None";
                if(chat.CaptchaButton == CaptchaType.Math) {
                    sol_val = "None";
                }
                try {
                    await mySql.OpenAsync();
                    var expire = DateTime.UtcNow.AddMinutes(chat.Minutes);
                    var query = $"INSERT INTO {chat.Id*-1}_captchas(id, userId, expire, attemps, type, solution) VALUES(0, {userId}, '{expire.ToString(Config.StandartDateFormat, CultureInfo.InvariantCulture)}', {chat.MaxCaptchaAttemps}, '{chat.CaptchaButton.ToString()}', '{sol_val}')";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    Program.Collections.CaptchaTemp[chat.Id].Add(new() {UserId = userId, Attemps = chat.MaxCaptchaAttemps, Expire = expire, Type = chat.CaptchaButton });
                    await Program.WriteDebbug($"Table {chat.Id}_captchas has insertion for {userId}");
                    oper = true;
                }
                catch(MySqlException exc) {
                    oper = false;
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }

            }
            return oper;
        }
        public static async Task<bool> UpdateUserAttempsCaptchaInTable(Chat chat, long userId, int attemps) {
            var oper = false;
            using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
                var sol_val = "None";
                if(chat.CaptchaButton == CaptchaType.Math) {
                    sol_val = "None";
                }
                try {
                    await mySql.OpenAsync();
                    var query = $"UPDATE {chat.Id*-1}_captchas SET attemps = {attemps} WHERE userId = {userId}";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    for(int i = 0; i < Program.Collections.CaptchaTemp[chat.Id].Count; i++)
                        if(Program.Collections.CaptchaTemp[chat.Id][i].UserId == userId) {
                            Program.Collections.CaptchaTemp[chat.Id][i].Attemps = attemps;
                            break;
                        }
                    await Program.WriteDebbug($"Table {chat.Id}_captchas has update for {userId}");
                    oper = true;
                }
                catch(MySqlException exc) {
                    oper = false;
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }

            }
            return oper;
        }
        public static async Task<bool> DeleteUserCaptchaInTable(Chat chat, long userId) {
            var oper = false;
            using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
                var sol_val = "None";
                if(chat.CaptchaButton == CaptchaType.Math) {
                    sol_val = "None";
                }
                try {
                    await mySql.OpenAsync();
                    var query = $"DELETE FROM {chat.Id*-1}_captchas WHERE userId = {userId}";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    await Program.WriteDebbug($"Table {chat.Id}_captchas has deletion for {userId}");
                    for(int i = 0; i < Program.Collections.CaptchaTemp[chat.Id].Count; i++)
                        if(Program.Collections.CaptchaTemp[chat.Id][i].UserId == userId) {
                            Program.Collections.CaptchaTemp[chat.Id].Remove(Program.Collections.CaptchaTemp[chat.Id][i]);
                            break;
                        }
                    oper = true;
                }
                catch(MySqlException exc) {
                    oper = false;
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }

            }
            return oper;
        }
        public static async Task<bool> DeleteUsersCaptchaTable(long chatId) {
            var oper = false;
            using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
                try {
                    await mySql.OpenAsync();
                    var query = $"DROP TABLE {chatId*-1}_captchas";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    oper = true;
                    await Loader.UpdateDebugs($"Table {chatId}_captchas was deleted");
                    Program.Collections.CaptchaTemp.Remove(chatId);
                }
                catch(MySqlException exc) {
                    oper = false;
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
            return oper;
        }

        public static async Task<List<Chat>?> GetChats() {
            List<Chat> list = new();
            using(MySqlConnection mySql = new($"{Config.DB_URL}{Config.ChatsData}")) {
                try {
                    await mySql.OpenAsync();
                    var query = "SELECT * FROM chats_config";
                    using(MySqlCommand myCmd = new(query, mySql)) {
                        using(MySqlDataReader myReader = (MySqlDataReader)await myCmd.ExecuteReaderAsync()) {
                            if(!myReader.HasRows) {
                                await mySql.CloseAsync();
                                await Program.WriteDebbug("Chats' Configs is empty");
                            }
                            else {
                                while(await myReader.ReadAsync()) {
                                    Chat chat = new() {
                                        Id = long.Parse(myReader[1].ToString()),
                                        MaxWarns = int.Parse(myReader[2].ToString()),
                                        Antispam = int.Parse(myReader[3].ToString()),
                                        AsActive = (myReader[4].ToString() != "0"),
                                        DetectUrl = (myReader[5].ToString() != "0"),
                                        ReportStorage = long.Parse(myReader[6].ToString()),
                                        CaptchaButton = (CaptchaType)Enum.Parse(typeof(CaptchaType), myReader[7].ToString()),
                                        NoBadWordsActive = (myReader[8].ToString() != "0"),
                                        Punish = (PunishType)Enum.Parse(typeof(PunishType), myReader[12].ToString()),
                                        MaxCaptchaAttemps = int.Parse(myReader[13].ToString()),
                                        Minutes = int.Parse(myReader[14].ToString()),
                                        StateOwner = long.Parse(myReader[18].ToString()),
                                        IsReceive = (myReader[20].ToString() != "0"),
                                        ChatState = (ChatState)Enum.Parse(typeof(ChatState), myReader[21].ToString())
                                    };

                                    if(myReader[9].ToString() != "None")
                                        chat.Rules = myReader[9].ToString();
                                    if(myReader[10].ToString() != "None")
                                        chat.CustomHello = myReader[10].ToString();
                                    if(myReader[11].ToString() != "None")
                                        chat.ChSticker = myReader[11].ToString();
                                    if(myReader[15].ToString() != "None")
                                        chat.Night = myReader[15].ToString();
                                    if(myReader[16].ToString() != "None")
                                        chat.State = myReader[16].ToString();
                                    if(myReader[17].ToString() != "None")
                                        chat.SettingsState = (SettingsState)Enum.Parse(typeof(SettingsState), myReader[17].ToString());
                                    if(myReader[19].ToString() != "None")
                                        chat.Gmt = myReader[19].ToString();
                                    if(myReader[22].ToString() != "None")
                                        chat.StExpire = DateTime.ParseExact(myReader[22].ToString(), Config.StandartDateFormat, CultureInfo.InvariantCulture);
                                    if(myReader[23].ToString() != "None")
                                        chat.NotifComment = myReader[23].ToString();

                                    list.Add(chat);
                                }
                                await mySql.CloseAsync();
                                await Program.WriteDebbug("Chats' Configs was collected");
                            }
                        }
                    }
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
            return list;
        }
        public static async Task InsertNewChat(long chatId) {
            using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
                try {
                    await mySql.OpenAsync();
                    var query = $@"INSERT INTO chats_config(
id, chatId, max_warns, antispam, as_active, detect_url, report_storage, captcha_button, nbw_active,
rules, custom_hello, ch_sticker, punish, cp_ma, cp_minutes, night, state, settings_state,
state_owner, gmt, receive_news, chat_state, st_expire, not_comment) VALUES(
0, {chatId}, {_default}, {_default}, {_default}, {_default}, {_default}, {_default}, {_default},
{_default}, {_default}, {_default}, {_default}, {_default}, {_default}, {_default}, {_default},
{_default}, {_default}, {_default}, {_default}, {_default}, {_default}, {_default})";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    Program.Chats.Add(new() { Id = chatId });
                    await Program.WriteDebbug($"New chat '{chatId}' is registered");
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }
        public static async Task<int> UpdateChatConfig(Chat chat) {
            Chat temp = new();
            for(int i = 0; i < Program.Chats.Count; i++)
                if(Program.Chats[i].Id == chat.Id)
                    temp = Program.Chats[i];
            var diff = DifferenceDetecter.GetDifferenceForChatConfig(chat, temp);
            using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
                try {
                    await mySql.OpenAsync();
                    var query = $"UPDATE chats_config {diff.Args} WHERE chatId = {chat.Id}";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    for(int i = 0; i < Program.Chats.Count; i++)
                        if(Program.Chats[i].Id == chat.Id) {
                            Program.Chats[i] = temp;
                            break;
                        }
                    await Program.WriteDebbug($"Chat '{chat.Id}' has {diff.ArgsCount} changes");

                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
            return diff.ArgsCount;
        }
        public static async Task DeleteChatConfig(Chat chat) {
            Program.Chats.Remove(chat);
            using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
                try {
                    await mySql.OpenAsync();
                    var query = $"DELETE FROM chats_config WHERE chatId = {chat.Id}";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    await Program.WriteDebbug($"Table chats_config has deletion for ID'{chat.Id}'");
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }

        public static async Task<List<Report>> GetChatsReports(long chat_storage) {
            List<Report> list = new();
            using(MySqlConnection mySql = new(Config.DB_URL + Config.LauraReportsStorages)) {
                try
                {
                    await mySql.OpenAsync();
                    var query = $"SELECT * FROM {chat_storage*-1}_storage";
                    using(MySqlCommand myCmd = new(query, mySql))
                        using(MySqlDataReader myReader = (MySqlDataReader)await myCmd.ExecuteReaderAsync())
                            if(myReader.HasRows != false)
                                while(myReader.Read()) {
                                    Report temp = new() { TargetUserId = long.Parse(myReader[1].ToString()),
                                        OutputUserId = long.Parse(myReader[2].ToString()),
                                        MesssageId = int.Parse(myReader[3].ToString()),
                                        Stamp = DateTime.ParseExact(myReader[5].ToString(), Config.StandartDateFormat, CultureInfo.InvariantCulture),
                                        IsAdmin = (myReader[6].ToString() != "0")};
                                    if(myReader[4].ToString() != "None")
                                        temp.Reason = myReader[4].ToString();
                                    list.Add(temp);
                                }
                    await mySql.CloseAsync();
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
            return list;
        }
        public static async Task CreateChatReportStorage(long chat_storage) {
            using(MySqlConnection mySql = new(Config.DB_URL + Config.LauraReportsStorages)) {
                try
                {
                    await mySql.OpenAsync();
                    var query = $"CREATE TABLE {chat_storage*-1}_storage{_values_reports}";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }
        public static async Task DeleteReportChatStorage(Chat chat) {
            using(MySqlConnection mySql = new(Config.DB_URL + Config.LauraReportsStorages)) {
                try {
                    await mySql.OpenAsync();
                    var query = $"DROP TABLE {chat.ReportStorage*-1}_storage";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    Program.Collections.ChatsReportsStorages.Remove(chat.Id);
                    await Program.WriteDebbug($"Reports Storage '{chat.ReportStorage}' was deleted");
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }
        public static async Task InsertReport(Report form, Chat chat) {
            using(MySqlConnection mySql = new(Config.DB_URL + Config.LauraReportsStorages)) {
                try {
                    await mySql.OpenAsync();
                    var reason = _default;
                    var isAdmin = 0;
                    if(form.IsAdmin)
                        isAdmin = 1;
                    if(form.Reason != null)
                        reason = $"'{form.Reason}'";
                    var query = @$"INSERT INTO {chat.ReportStorage*-1}_storage(id, target_user_id, output_user_id, message_id, reason, stamp, is_admin) VALUES(
0, {form.TargetUserId}, {form.OutputUserId}, {form.MesssageId}, {reason}, '{DateTime.UtcNow.ToString(Config.StandartDateFormat, CultureInfo.InvariantCulture)}', {isAdmin})";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    Program.Collections.ChatsReportsStorages[chat.Id].Add(form);
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }
        public static async Task DeleteReportFromStorage(Report form, long report_storage) {
            using(MySqlConnection mySql = new(Config.DB_URL + Config.LauraReportsStorages)) {
                try {
                    await mySql.OpenAsync();
                    var query = $"DELETE FROM {report_storage*-1}_storage WHERE message_id = {form.MesssageId}";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    await Program.WriteDebbug($"Report '{form.MesssageId}' deleted from storage {report_storage}");
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }
    }
}
