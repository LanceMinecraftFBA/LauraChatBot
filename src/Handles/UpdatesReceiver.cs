using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LauraChatManager.Handles {
    public class Updates {
        public static async Task Receiver(ITelegramBotClient bot, Update update, CancellationToken ct) {
            if(Program.HasException) {
                Program.HasException = false;
                return;
            }
            switch (update.Type) {
                case UpdateType.Message:
                    await LauraChatManager.Events.Message.InvokeMessage(bot, update.Message);
                    break;
                case UpdateType.ChannelPost:
                    await LauraChatManager.Events.Message.InvokeMessage(bot, update.ChannelPost);
                    break;
                case UpdateType.CallbackQuery:
                    await LauraChatManager.Events.CallbackQuery.InvokeCallback(bot, update.CallbackQuery);
                    break;
            }
        }
    }
}
