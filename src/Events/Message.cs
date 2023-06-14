using LauraChatManager.Handles.UpdateHandlers.Message;
using LauraChatManager.Types;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace LauraChatManager.Events {
    public class Message {
        public static List<FbaPosts> News = new();
        private static ITelegramBotClient Bot;
        public static async Task InvokeMessage(ITelegramBotClient bot, Telegram.Bot.Types.Message msg) {
            Bot = bot;
            if(msg.Chat.Id == msg.From.Id)
                await UserHandler.Invoke(bot, msg);
            if(msg.Chat.Id != msg.From.Id)
                await ChatHandler.Invoke(bot, msg);
            if(msg.Chat.Id == Configuration.Config.ChannelId)
                switch(msg.Type){
                    case MessageType.Text:
                        News.Add(new() {Text = msg.Text, PostID = msg.MessageId});
                        break;
                    case MessageType.Photo:
                        News.Add(new() {Text = msg.Text, PostID = msg.MessageId, Photo = msg.Photo[0].FileId});
                        break;
                    case MessageType.Video:
                        News.Add(new() {Text = msg.Text, PostID = msg.MessageId, Video = msg.Video.FileId});
                        break;
                    case MessageType.Document:
                        News.Add(new() {Text = msg.Text, PostID = msg.MessageId, File = msg.Document.FileId});
                        break;
                    case MessageType.Audio:
                        News.Add(new() {Text = msg.Text, PostID = msg.MessageId, File = msg.Audio.FileId});
                        break;
                }
        }
        public static async void NewsSender() {
            if(News.Count > 0) {
                var posts = News;
                for(int i = 0; i < posts.Count; i++) {
                    var news = posts[i];
                    News.Remove(news);
                    for(int j = 0; j < Program.Users.Count; j++){
                        if(Program.Users[j].IsReceiving)
                        {
                            if(news.Photo != null)
                                await Bot.SendPhotoAsync(Program.Chats[j].Id, new InputFileId(news.Photo), caption:  $"<b>Говорит <a href=\"t.me/FBA_Studio\">FBA Studio</a>:</b>\n{news.Text}", parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl($"t.me/FBA_Studio/{news.PostID}", "Перейти к посту")));
                            else if(news.Video != null)
                                await Bot.SendVideoAsync(Program.Chats[j].Id, new InputFileId(news.Video), caption: $"<b>Говорит <a href=\"t.me/FBA_Studio\">FBA Studio</a>:</b>\n{news.Text}", parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl($"t.me/FBA_Studio/{news.PostID}", "Перейти к посту")));
                            else if(news.File != null)
                                await Bot.SendDocumentAsync(Program.Chats[j].Id, new InputFileId(news.File), caption: $"<b>Говорит <a href=\"t.me/FBA_Studio\">FBA Studio</a>:</b>\n{news.Text}", parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl($"t.me/FBA_Studio/{news.PostID}", "Перейти к посту")));
                            else if(news.Audio != null)
                                await Bot.SendAudioAsync(Program.Chats[j].Id, new InputFileId(news.Audio), caption: $"<b>Говорит <a href=\"t.me/FBA_Studio\">FBA Studio</a>:</b>\n{news.Text}", parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl($"t.me/FBA_Studio/{news.PostID}", "Перейти к посту")));
                            else
                                await Bot.SendTextMessageAsync(Program.Users[j].Id, $"<b>Говорит <a href=\"t.me/FBA_Studio\">FBA Studio</a>:</b>\n{news.Text}", parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl($"t.me/FBA_Studio/{news.PostID}", "Перейти к посту")), disableWebPagePreview: true);
                        }
                    }
                    for(int j = 0; j < Program.Chats.Count; j++){
                        if(Program.Chats[j].IsReceive) {
                            if(news.Photo != null)
                                await Bot.SendPhotoAsync(Program.Chats[j].Id, new InputFileId(news.Photo), caption:  $"<b>Говорит <a href=\"t.me/FBA_Studio\">FBA Studio</a>:</b>\n{news.Text}", parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl($"t.me/FBA_Studio/{news.PostID}", "Перейти к посту")));
                            else if(news.Video != null)
                                await Bot.SendVideoAsync(Program.Chats[j].Id, new InputFileId(news.Video), caption: $"<b>Говорит <a href=\"t.me/FBA_Studio\">FBA Studio</a>:</b>\n{news.Text}", parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl($"t.me/FBA_Studio/{news.PostID}", "Перейти к посту")));
                            else if(news.File != null)
                                await Bot.SendDocumentAsync(Program.Chats[j].Id, new InputFileId(news.File), caption: $"<b>Говорит <a href=\"t.me/FBA_Studio\">FBA Studio</a>:</b>\n{news.Text}", parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl($"t.me/FBA_Studio/{news.PostID}", "Перейти к посту")));
                            else if(news.Audio != null)
                                await Bot.SendAudioAsync(Program.Chats[j].Id, new InputFileId(news.Audio), caption: $"<b>Говорит <a href=\"t.me/FBA_Studio\">FBA Studio</a>:</b>\n{news.Text}", parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl($"t.me/FBA_Studio/{news.PostID}", "Перейти к посту")));
                            else
                                await Bot.SendTextMessageAsync(Program.Chats[j].Id, $"<b>Говорит <a href=\"t.me/FBA_Studio\">FBA Studio</a>:</b>\n{news.Text}", parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl($"t.me/FBA_Studio/{news.PostID}", "Перейти к посту")), disableWebPagePreview: true);
                        }
                    }
                }
            }
        }
    }
}
