namespace LauraChatManager.Types {
    public class User {
        public string? Nickname;
        public long Id;
        public bool IsBlocked;
        public DateTime? BlockEnd;
        public bool IsRaider;
        public bool IsFbaEnemy;
        public bool IsReceiving;
        public long Rating;
        public WarnUser Warns;

        public User() {
            IsBlocked = false;
            IsRaider = false;
            IsFbaEnemy = false;
            Rating = 0;
        }
    }
}
