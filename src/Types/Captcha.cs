using LauraChatManager.Types.Enums;

namespace LauraChatManager.Types {
    public class Captcha {
        public long UserId;
        public DateTime Expire;
        public int Attemps;
        public CaptchaType Type;
        public string? Solution;
    }
}
