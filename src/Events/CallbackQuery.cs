using LauraChatManager.Handles.UpdateHandlers.Callback;
using Telegram.Bot;

namespace LauraChatManager.Events {
    public class CallbackQuery {
        public static async Task InvokeCallback(ITelegramBotClient bot, Telegram.Bot.Types.CallbackQuery call) {
            if(call.Message.Chat.Id == call.From.Id)
                await UserHandler.Invoke(bot, call);
            else
                await ChatHandler.Invoke(bot, call);
        }
    }
}
