namespace LauraChatManager.Types {
    public class WarnUser {
        public long UserId;
        public Dictionary<long, List<Warn>> Warns;
    }
    public class Warn {
        public int Id;
        public string Reason;
        public DateTime Date;
        public DateTime Expire;
    }
}
