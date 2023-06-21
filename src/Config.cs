namespace LauraChatManager.Configuration
{
    public class Config {
        public static readonly string Token = "bot token";

        private static string host { get; set; } = "host;
        private static string user { get; set; } = "user";
        private static string passwrd { get; set; } = "password";
        public static readonly string DB_URL = $"server={host};user={user};password={passwrd};database=";

        public static readonly string StandartDateFormat = "MM/dd/yyyy hh:mm:ss tt";

        public static readonly string ChatsData = "LauraChatsData";
        public static readonly string UsersData = "LauraUsersData";
        public static readonly string LauraData = "LauraData";
        public static readonly string LauraReportsStorages = "LauraReportsStorages";
        public static readonly string LocalNswfDetector = "http://example.com:3000";
        public static readonly long ChannelId = 0;
    }

    public class TApiConfig {
        public static readonly int ApiId = 0;
        public static readonly string ApiHash = "api hash for telegram client";
    }
    public class QiwiConfig {
        public string PublicKey = "public key";
        public string SecretKey = "secret key";
        public string Color = "theme code";
    }
}
