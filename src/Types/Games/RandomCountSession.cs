using LauraChatManager.Types.Games.Enum;

namespace LauraChatManager.Types.Games {
    public class RandomCountSession {
        public long SessionId;
        public bool IsChatSession;
        public List<long> Members;
        public int SelectedCount;
        public RcDifficulty Difficulty;
        public Dictionary<long, int> Counts;
    }
}