using LauraChatManager.Handles.Other;

namespace LauraChatManager
{
    public class Loader {
        private static string directory = "Logs";
        private static string err_path = $"{directory}/errors.lauralog";
        private static string debug_path = $"{directory}/debug.lauralog";
        private static string larx_path = $"{directory}/larx.lauralog";

        public static Task CreateLogs() {
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if(!File.Exists(err_path))
                File.CreateText(err_path);
            if(!File.Exists(debug_path))
                File.CreateText(debug_path);
            if(!File.Exists(larx_path))
                File.CreateText(larx_path);
            return Task.CompletedTask;
        }

        public static Task UpdateErrors(string line) {
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if(!File.Exists(err_path))
                File.CreateText(err_path);
            using(StreamWriter stream = File.AppendText(err_path)) {
                stream.WriteLine(line);
                stream.Close();
            }
            return Task.CompletedTask;
        }
        public static Task UpdateDebugs(string line) {
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if(!File.Exists(debug_path))
                File.CreateText(debug_path);
            using(StreamWriter stream = File.AppendText(debug_path)) {
                stream.WriteLine(line);
                stream.Close();
            }
            return Task.CompletedTask;
        }
        public static Task UpdateLarx(string line) {
            if(!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            if(!File.Exists(larx_path))
                File.CreateText(larx_path);
            using(StreamWriter stream = File.AppendText(larx_path)) {
                stream.WriteLine(line);
                stream.Close();
            }
            return Task.CompletedTask;
        }

        public static async Task<Collections> GetCollections() {
            var collections = new Collections();
            Program.Chats = await ChatsDataTables.GetChats();
            collections.ActiveSessions = new();
            collections.Admins = new();
            collections.CaptchaTemp = new();
            collections.ChatsReportsStorages = new();
            collections.RsRequests = new();
            collections.TempConfigs = new();
            await Program.WriteDebbug("Chats was filled");
            if(Program.Chats.Count > 0) {
                foreach(var chat in Program.Chats) {
                    var captchas = await ChatsDataTables.GetUsersCaptcha(chat.Id);
                    collections.CaptchaTemp.Add(chat.Id, captchas);
                    collections.RsRequests = await ReportStorageReqStorage.GetReportStorageRequests();
                    if(chat.ReportStorage != 0) {
                        var reports_storage = await ChatsDataTables.GetChatsReports(chat.ReportStorage);
                        collections.ChatsReportsStorages.Add(chat.Id, reports_storage);
                    }
                }
            }
            await Program.WriteDebbug("Collections.CaptchaTemp was filled");
            await Program.WriteDebbug("Loader: Chats was loaded");
            await Program.WriteDebbug("Loading Users");
            Program.Users = await UsersDataTables.GetUsers();
            await Program.WriteDebbug("Loading users warns");
            if(Program.Users.Count > 0) {
                for (int i = 0; i < Program.Users.Count; i++) {
                    Program.Users[i].Warns = await UsersDataTables.GetUserWarns(Program.Users[i].Id);
                }
                await Program.WriteDebbug("Loading users rating controls");
                var controls = await RatingController.GetRatingControls();
                for(int i = 0; i < Program.Users.Count; i++) {
                    var userId = Program.Users[i].Id;
                    if(controls.ContainsKey(userId))
                        Program.Users[i].RatingControls = controls[userId];
                }
            }
            return collections;
        }
    }
}
