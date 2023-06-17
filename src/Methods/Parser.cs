using LauraChatManager.Types;

namespace LauraChatManager.Methods {
    public class Parser {
        public static Gmt ParseGmt(string gmt) {
            Gmt val = new();
            var args = gmt.Split(":");
            if(args[0].StartsWith("+")) {
                val.Hours = int.Parse(args[0].Split("+")[1]);
                val.Minutes = int.Parse(args[1]);
            }
            else {
                val.Hours = -int.Parse(args[0].Split("-")[1]);
                val.Minutes = -int.Parse(args[1]);
            }
            return val;
        }
        public static string ParseText(string inp, Dictionary<string, string> args){
            var result = inp;
            for(int i = 0; i < args.Count; i++)
                if(inp.Contains(args.ElementAt(i).Key))
                    inp.Replace(args.ElementAt(i).Key, args.ElementAt(i).Value);
            return result;
        }
        public static Time ParseTime(string inp) {
            var time = inp.Split(":");
            return new() { Hour = int.Parse(time[0]), Minute = int.Parse(time[1]) };
        }
    }
}