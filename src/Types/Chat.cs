using LauraChatManager.Types.Enums;

namespace LauraChatManager.Types {
    public class Chat {
        public long Id;
        public int MaxWarns;
        public int Antispam;
        public bool AsActive;
        public bool DetectUrl;
        public long ReportStorage;
        public CaptchaType CaptchaButton;
        public bool NoBadWordsActive;
        public string? Rules;
        public string? CustomHello;
        public string? ChSticker;
        public PunishType Punish;
        public int MaxCaptchaAttemps;
        public int Minutes;
        public string? Night;
        public string? State;
        public SettingsState? SettingsState;
        public long StateOwner;
        public string? Gmt;
        public bool IsReceive;
        public ChatState ChatState;
        public DateTime? StExpire;
        public string? NotifComment;

        public Chat() {
            MaxWarns = 4;
            Antispam = 4;
            AsActive = false;
            DetectUrl = false;
            ReportStorage = 0;
            CaptchaButton = CaptchaType.None;
            NoBadWordsActive = false;
            Punish = PunishType.Mute;
            MaxCaptchaAttemps = 3;
            Minutes = 5;
            StateOwner = 0;
            IsReceive = true;
            ChatState = ChatState.State;
        }
    }
}
