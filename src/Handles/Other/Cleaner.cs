using LauraChatManager.Types;

namespace LauraChatManager.Handles.Other {
    public class Cleaner {
        public static async Task DeleteChatData(Chat chat) {
            await Program.WriteDebbug($"Processing deletion chat's data for '{chat.Id}'");
            await ChatsDataTables.DeleteUsersCaptchaTable(chat.Id);
            await ChatsDataTables.DeleteChatConfig(chat);
            await ChatsDataTables.DeleteReportChatStorage(chat);
        }
        public static async Task DeleteUserData(User user){
            await Program.WriteDebbug($"Processing deletion user's data for '{user.Id}'");
            await UsersDataTables.DeleteUser(user.Id);
            await UsersDataTables.DeleteUserWarns(user.Id);
        }
    }
}
