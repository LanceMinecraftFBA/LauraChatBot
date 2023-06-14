namespace LauraChatManager.Types {
    public class Report {
        public long TargetUserId;
        public long OutputUserId;
        public int MesssageId;
        public string? Reason;
        public DateTime Stamp;
        public bool IsAdmin;
    }
}
