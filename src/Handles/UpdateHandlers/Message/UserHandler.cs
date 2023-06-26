using Telegram.Bot;
using Telegram.Bot.Types.Enums;

using LauraChatManager.Types;
using LauraChatManager.Handles.Other;
using LauraChatManager.Buttons;

namespace LauraChatManager.Handles.UpdateHandlers.Message {
    public class UserHandler {
        public static async Task Invoke(ITelegramBotClient bot, Telegram.Bot.Types.Message msg) {
            var text = msg.Text;
            var userId = msg.From.Id;
            var fullName = msg.From.FirstName;
            var sharedChat = msg.ChatShared;

            if(!Program.UsersRaw.ContainsKey(userId)) {
                Program.UsersRaw.Add(userId, false);
                await UsersDataTables.InsertUser(userId);
                await UsersDataTables.CreateUserWarns(userId);
            }

            if(Program.UsersRaw[userId]) {
                return;
            
            }

            if(sharedChat != null) {
                var chat = await bot.GetChatAsync(sharedChat.ChatId);
                if(!Program.ChatsRaw.Contains(sharedChat.ChatId)) {
                    await bot.SendTextMessageAsync(userId, "Благодарю Вас за то, что Вы теперь используете меня😌");
                }
                else {
                    await bot.SendTextMessageAsync(userId, $"⚙️Загружаю параметры чата \"{chat.Title}\"");
                    var chatConfig = new Chat();
                    foreach(var chatC in Program.Chats) {
                        if(chatC.Id == chat.Id) {
                            chatConfig = chatC;
                            chatConfig.SettingsState = Types.Enums.SettingsState.Main;
                            break;
                        }
                    }
                    await ChatsDataTables.UpdateChatConfig(chatConfig);
                }
            }

            if(text != null) {
                await Program.WriteDebbug($"New message text by user '{fullName}' | '{userId}': {text}");
                if(text.ToLower() == "/start")
                    await bot.SendTextMessageAsync(userId, $"Привет, <a href=\"tg://user?id={userId}\">{fullName}</a>! Я рада познакомиться с вами. Я Лаура, ваш чат-менеджер. Моя задача - поддерживать порядок в чате и помогать участникам. Кроме того, я могу сыграть с вами в игры! 😊\n\nЕсли вы хотите узнать больше о моих возможностях, просто введите команду /help, и я расскажу вам подробнее.", parseMode: ParseMode.Html, disableWebPagePreview: true, replyMarkup: Buttons.Buttons.GetUserMenu());
                if(text.ToLower() == "/help")
                    await bot.SendTextMessageAsync(
                        userId,
$@"<b>⌨️Мой список команд:</b>

<b><i>Общие команды</i></b>
- /help - получить справочный материал для работы с ботом
- .состояние - получить состояние бота",
                        parseMode: ParseMode.Html
                    );
                if(text.ToLower() == ".состояние") {
                    TimeSpan time = Program.NextLarxUpdate - DateTime.UtcNow;
                    await bot.SendTextMessageAsync(
                        chatId: userId,
                        text:
                        @$"<b>Моё состояние🌸:</b>
- Пинг: <code>{Program.Ping} мс</code>
- Пинг базы: <code>{Program.DbPing} мс</code>
- RAM: <code>{Program.RamUsage} МБ</code>

<b><i>Следующее обновление данных: </i></b>
<code>{time.Hours} часов {time.Minutes} минут {time.Seconds} секунд</code>",
                        parseMode: ParseMode.Html);
                }
            }
        }
    }
}
