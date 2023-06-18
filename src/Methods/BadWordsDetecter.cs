namespace LauraChatManager.Methods;

public class BadWordsDetecter {
        private static readonly string[] data =
        {
            "блять", "сука", "блядина", "пидор", "пидорас", "сучка", "шлюха", "шалава", "вагина", "залупа", "хуй", "нахуй", "еблан", "ебланчик",
            "хуесос", "чурка", "чурки", "суки", "пидоры", "пидорасы", "блядины", "шлюхи", "шалавы", "хуи", "ебланы", "ебланчик", "ебанавты", "ебанафты",
            "ебанафт", "хуесосы", "безмамные", "безмамный", "безмамного", "пидора", "блядину", "блядинам", "шлюхе", "пиздец", "пизда", "пизды", "хуеглот",
            "бля", "ебать", "ебанутся", "ебанашка", "ебу", "дохуя", "дохуище", "долбоёб", "долбоеб", "долбоёбы", "долбоебы", "долбоёбка", "долбоебка", "долбоёбки", "долбоебки",
            "ебанутый", "хуйня", "хуйню", "поебота", "ахуел", "ахуели"
        };

        public static bool CheckBadWords(string message) {
            var detect = false;
            var words = message.ToLower().Split(' ');
            for(int i = 0; i < words.Length; i++) {
                for(int j = 0; j < data.Length; j++) {
                    if(words[i] == data[j]) {
                        detect = true;
                        break;
                    }
                    if(words[i].StartsWith(data[j])) {
                        detect = true;
                        break;
                    }
                }
                if(detect == true)
                    break;
            }
            return detect;
        }
}
