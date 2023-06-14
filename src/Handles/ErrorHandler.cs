using Telegram.Bot;

namespace LauraChatManager.Handles
{
    public class Error
    {
        public static async Task Handler(ITelegramBotClient bot, Exception exc, CancellationToken cts)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{DateTime.UtcNow} UTC: Exception occured: {exc.Message}");
            Console.ForegroundColor = ConsoleColor.White;
            
            Program.HasException = true;
            bot.StartReceiving(LauraChatManager.Handles.Updates.Receiver, Handler, Program.Options);
        }
    }
}
