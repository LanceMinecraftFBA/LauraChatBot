using LauraChatManager.Types;
using LauraChatManager.Types.Games;

namespace LauraChatManager {
    public class Collections {
        public Dictionary<long, List<Captcha>> CaptchaTemp;
        public Dictionary<long, List<Report>> ChatsReportsStorages;
        public Dictionary<long, RandomCountSession> ActiveSessions;
        public List<Chat> TempConfigs;
        public List<Admin> Admins;
    }
}

