using System.Globalization;
using System.Net.Mime;
using LauraChatManager.Buttons;
using LauraChatManager.Handles.Other;
using LauraChatManager.Methods;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace LauraChatManager.Handles.UpdateHandlers.Message {
    public class ChatHandler {
        private static readonly ChatPermissions mutePermissions = new() { CanSendMessages = false, CanSendAudios = false, CanSendDocuments = false, CanSendPhotos = false, CanSendVideos = false, CanSendVideoNotes = false, CanSendPolls = false, CanSendVoiceNotes = false, CanSendOtherMessages = false };
        
        public static async Task Invoke(ITelegramBotClient bot, Telegram.Bot.Types.Message msg) {
            if(msg.Chat.Type == ChatType.Channel) {
                await Program.WriteDebbug($"{msg.Chat.Id} | {msg.Text}");
                return;
            }
            var text = msg.Text;
            var photo = msg.Photo;
            var chat = msg.Chat;
            var user = msg.From;
            var newChatMembers = msg.NewChatMembers;
            var leftChatMember = msg.LeftChatMember;
            
            if(!Program.ChatsRaw.Contains(chat.Id) && chat.Type == ChatType.Supergroup) {
                Program.ChatsRaw.Add(chat.Id);
                await ChatsDataTables.InsertNewChat(chat.Id);
                await ChatsDataTables.CreateUsersCaptchaTable(chat.Id);
            }

            if(Program.UsersRaw.ContainsKey(user.Id) && Program.UsersRaw[user.Id])
                return;
            
            if(msg.Photo != null) {
                var is_prem = false;
                for(int i = 0; i < 1; i++) {
                    if(Program.Collections.PremiumChats[i].Id == chat.Id) {
                        is_prem = true;
                        break;
                    }
                }
                if(is_prem) {
                    if(!Directory.Exists("TempPhotos"))
                        Directory.CreateDirectory("TempPhotos");
                    var PhotoF = photo[3];
                    var file = await bot.GetFileAsync(PhotoF.FileId);
                    var dest = $"./TempPhotos/img{PhotoF.FileUniqueId}.jpg";
                    
                    Stream stream = File.Create(dest);
                    await bot.DownloadFileAsync(file.FilePath, stream);
                    await stream.DisposeAsync();
                    
                    var nsfwObject = await NsfwDetecter.GetNsfwScan(dest);
                    if(double.Parse(nsfwObject.Hentai.ToString("F3")) >= 0.17) {
                        var member = await bot.GetChatMemberAsync(chat.Id, user.Id);
                        var botMemb = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                        
                        var status = member.Status;
                        
                        if(status == ChatMemberStatus.Creator) {
                            status = ChatMemberStatus.Administrator;
                        }
                        
                        if(botMemb.Status != ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул хентай-фотку🤔?",
                                messageThreadId: msg.MessageThreadId,
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                        }
                        else if(status == ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул хентай-фотку, если твоя задача - защищать чат🤔?",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                        else {
                            await bot.SendTextMessageAsync(
                                chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a> получает бан за хентай-фотку💢",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.BanChatMemberAsync(
                                chat.Id,
                                user.Id);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                    }
                    else if(double.Parse(nsfwObject.Porn.ToString("F3")) >= 0.17) {
                        await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        var member = await bot.GetChatMemberAsync(chat.Id, user.Id);
                        var botMemb = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                        
                        var status = member.Status;
                        
                        if(status == ChatMemberStatus.Creator) {
                            status = ChatMemberStatus.Administrator;
                        }
                        
                        if(botMemb.Status != ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул порно-фотку🤔?",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                        }
                        else if(status == ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул порно-фотку, если твоя задача - защищать чат🤔?",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                        else {
                            await bot.SendTextMessageAsync(
                                chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a> получает бан за порно-фотку💢",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.BanChatMemberAsync(
                                chat.Id,
                                user.Id);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                    }
                    else if(double.Parse(nsfwObject.Sexy.ToString("F3")) >= 0.40) {
                        var member = await bot.GetChatMemberAsync(chat.Id, user.Id);
                        var botMemb = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);

                        var status = member.Status;

                        if(status == ChatMemberStatus.Creator) {
                            status = ChatMemberStatus.Administrator;
                        }
                        
                        if(botMemb.Status != ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул слишком сексуальную фотку🤔?",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                        }
                        else if(status == ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул слишком сексуальную фотку, если твоя задача - защищать чат🤔?",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                        else {
                            await bot.SendTextMessageAsync(
                                chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a> получает бан за слишком сексуальную фотку💢",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.BanChatMemberAsync(
                                chat.Id,
                                user.Id);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                    }
                    foreach(var fileT in Directory.GetFiles("TempPhotos")) {
                        File.Delete(fileT);
                    }
                }
            }
            if(msg.Sticker != null) {
                var is_prem = false;
                for(int i = 0; i < 1; i++) {
                    if(Program.Collections.PremiumChats[i].Id == chat.Id) {
                        is_prem = true;
                        break;
                    }
                }
                if(is_prem) {
                    if(!Directory.Exists("TempPhotos"))
                        Directory.CreateDirectory("TempPhotos");
                    var PhotoF = msg.Sticker;
                    var file = await bot.GetFileAsync(PhotoF.FileId);
                    var path = $"./TempPhotos/img{PhotoF.FileUniqueId}.jpg";
                    var dest = $"./TempPhotos/img{PhotoF.FileUniqueId}.png";
                    
                    Stream stream = File.Create(dest);
                    await bot.DownloadFileAsync(file.FilePath, stream);
                    await stream.DisposeAsync();
                    
                    using var img = await Image.LoadAsync(dest);
                    await img.SaveAsync(path, new JpegEncoder());
                    img.Dispose();
                    
                    var nsfwObject = await NsfwDetecter.GetNsfwScan(path);
                    if(double.Parse(nsfwObject.Hentai.ToString("F3")) >= 0.17) {
                        var member = await bot.GetChatMemberAsync(chat.Id, user.Id);
                        var botMemb = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                        
                        var status = member.Status;
                        
                        if(status == ChatMemberStatus.Creator) {
                            status = ChatMemberStatus.Administrator;
                        }
                        
                        if(botMemb.Status != ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул хентай-стикер?",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                        }
                        else if(status == ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул хентай-стикер, если твоя задача - защищать чат🤔?",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                        else {
                            await bot.SendTextMessageAsync(
                                chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a> получает бан за хентай-стикер💢",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.BanChatMemberAsync(
                                chat.Id,
                                user.Id);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                    }
                    else if(double.Parse(nsfwObject.Porn.ToString("F3")) >= 0.17) {
                        var member = await bot.GetChatMemberAsync(chat.Id, user.Id);
                        var botMemb = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                        
                        var status = member.Status;
                        
                        if(status == ChatMemberStatus.Creator) {
                            status = ChatMemberStatus.Administrator;
                        }
                        
                        if(botMemb.Status != ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул порно-стикер🤔?",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                        }
                        else if(status == ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул порно-стикер, если твоя задача - защищать чат🤔?",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                        else {
                            await bot.SendTextMessageAsync(
                                chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a> получает бан за порно-стикер💢",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.BanChatMemberAsync(
                                chat.Id,
                                user.Id);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                    }
                    else if(double.Parse(nsfwObject.Sexy.ToString("F3")) >= 0.40) {
                        var member = await bot.GetChatMemberAsync(chat.Id, user.Id);
                        var botMemb = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);

                        var status = member.Status;

                        if(status == ChatMemberStatus.Creator) {
                            status = ChatMemberStatus.Administrator;
                        }
                        
                        if(botMemb.Status != ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул слишком сексуальнуй стикер🤔?",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                        }
                        else if(status == ChatMemberStatus.Administrator) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a>, нафига ты кинул слишком сексуальнуй стикер, если твоя задача - защищать чат🤔?",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                        else {
                            await bot.SendTextMessageAsync(
                                chat.Id,
                                text: $"<a href=\"tg://user?id={user.Id}\">{user.FirstName}</a> получает бан за слишком сексуальный стикер💢",
                                messageThreadId: msg.MessageThreadId,
                                parseMode: ParseMode.Html,
                                disableWebPagePreview: true);
                            await bot.BanChatMemberAsync(
                                chat.Id,
                                user.Id);
                            await bot.DeleteMessageAsync(chat.Id, msg.MessageId);
                        }
                    }
                    foreach(var fileT in Directory.GetFiles("TempPhotos")) {
                        File.Delete(fileT);
                    }
                }
            }
            if(newChatMembers != null) {
                if(newChatMembers[0].Id == Program.Bot.Id) {
                    await Program.WriteDebbug($"Bot added to chat '{chat.Title}' | '{chat.Id}' | '{Program.ChannelBase[(chat.Id + 1000000000000)*-1]}' ");
                    await bot.SendTextMessageAsync(chat.Id,
                    @"<b>Благодарю Вас за добавление меня в чат😊!</b>
❕Если Вы не знаете, как со мной работать, введите команду /help",
                    parseMode: ParseMode.Html);
                }
                else{
                    var chatL = Program.Chats[Program.ChatsRaw.IndexOf(chat.Id)];
                    await bot.SendTextMessageAsync(chat.Id,
                        $"<b>👋🏻Добро пожаловать в чат \"{chat.Title}\", <a href=\"tg://user?id={newChatMembers[0].Id}\">{newChatMembers[0].FirstName}</a>!</b>",
                        parseMode: ParseMode.Html,
                        disableWebPagePreview: true);
                    if(chatL.Rules != null) {
                        var messg = await bot.SendTextMessageAsync(chat.Id,
                            "Прочтите правила чата ниже👇🏻");
                        await bot.SendTextMessageAsync(chat.Id,
                            chatL.Rules,
                            parseMode: ParseMode.Html,
                            replyToMessageId: messg.MessageId);
                    }
                }
            }
            if(leftChatMember != null) {
                if(leftChatMember.Id == Program.Bot.Id) {
                    var chatL = Program.Chats[Program.ChatsRaw.IndexOf(chat.Id)];
                    await Program.WriteDebbug($"Bot removed from chat '{chat.Title}' | '{chat.Id}' ");
                    await Cleaner.DeleteChatData(chatL);
                    Program.ChatsRaw.Remove(chat.Id);
                    Program.ChannelBase.Remove((chat.Id + 1000000000000)*-1);
                }
                else if(user.Id != Program.Bot.Id) {
                    await bot.SendTextMessageAsync(chat.Id, $"<b><a href=\"tg://user?id={leftChatMember.Id}\">{leftChatMember.FirstName}</a> покинул нас😕</b>", parseMode: ParseMode.Html, disableWebPagePreview: true);
                }
            }
            if(text != null) {
                await Program.WriteDebbug($"New message from chat '{chat.Title}' | '{chat.Id}' by user '{user.FirstName}' | '{user.Id}': {text}");
                if(text.ToLower() == "/help" || text.ToLower() == "/help@" + Program.Bot.Username.ToLower()) {
                    int? threadId = msg.MessageThreadId;
                    await bot.SendTextMessageAsync(
                        chatId: chat.Id,
                        messageThreadId: threadId,
                        text: $@"<b>⌨️Мой список команд:</b>",
                        parseMode: ParseMode.Html
                    );
                }
                
                #region Admin commands without parameters
                if(text.ToLower() == ".бан") {
                    ChatMember botAdmin = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                    ChatMember outputMember = await bot.GetChatMemberAsync(chat.Id, user.Id);
                    
                    var outputMemberStatus = outputMember.Status;
                    if(outputMember.Status == ChatMemberStatus.Creator)
                        outputMemberStatus = ChatMemberStatus.Administrator;
                    
                    if(botAdmin.Status != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "❕Я не являюсь администратором чата, поэтому я не могу совершить это действие",
                            replyToMessageId: msg.MessageId);
                    }
                    else if(outputMemberStatus != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> у вас недостаточно прав",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else {
                        if(msg.ReplyToMessage == null) {
                            await bot.SendTextMessageAsync(chatId: chat.Id,
                                text: "<b>❗️Вы не указали участника, которого вы хотели изнать!</b>\nЯ не способна банить рандомного человека🙃",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html);
                        }
                        else {
                            var replyUser = msg.ReplyToMessage.From;
                            var targetUser = await bot.GetChatMemberAsync(chat.Id, replyUser.Id);
                            
                            var targetUserAdministrator = targetUser.Status;
                            if(targetUser.Status == ChatMemberStatus.Creator)
                                targetUserAdministrator = ChatMemberStatus.Administrator;
                            
                            if(replyUser.Id == Program.Bot.Id)
                                await bot.SendTextMessageAsync(chat.Id,
                                text: "Меня назначили администратором, чтобы потом забанить себя же🤡",
                                replyToMessageId: msg.MessageId);
                            
                            else if(targetUser.Status == ChatMemberStatus.Administrator) {
                                await bot.SendTextMessageAsync(chatId: chat.Id,
                                    text: $"Тебе бы стоило решить проблему с <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> мирным способом😑.\n\nЯ не собираюсь вмешиваться в вашу войну😒",
                                    replyToMessageId: msg.MessageId,
                                    parseMode: ParseMode.Html,
                                    disableWebPagePreview: true);
                            }
                            else {
                                await bot.BanChatMemberAsync(chat.Id, replyUser.Id);
                                await bot.SendTextMessageAsync(chat.Id,
                                    text: $"💢Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был изгнан из чата!",
                                    replyToMessageId: msg.MessageId,
                                    parseMode: ParseMode.Html,
                                    disableWebPagePreview: true);
                            }
                        }
                    }
                }
                if(text.ToLower() == ".разбан") {
                    ChatMember botAdmin = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                    ChatMember outputMember = await bot.GetChatMemberAsync(chat.Id, user.Id);
                    
                    var outputMemberStatus = outputMember.Status;
                    if(outputMember.Status == ChatMemberStatus.Creator)
                        outputMemberStatus = ChatMemberStatus.Administrator;
                    
                    if(botAdmin.Status != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "❕Я не являюсь администратором чата, поэтому я не могу совершить это действие",
                            replyToMessageId: msg.MessageId);
                    }
                    else if(outputMemberStatus != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> у вас недостаточно прав",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else {
                        if(msg.ReplyToMessage == null) {
                            await bot.SendTextMessageAsync(chatId: chat.Id,
                                text: "<b>❗️Вы не указали участника, которого вы хотели вернуть!</b>\nЯ не способна вернуть рандомного человека🙃",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html);
                        }
                        else {
                            var replyUser = msg.ReplyToMessage.From;
                            var targetUser = await bot.GetChatMemberAsync(chat.Id, replyUser.Id);
                            
                            var targetUserAdministrator = targetUser.Status;
                            if(targetUser.Status == ChatMemberStatus.Creator)
                                targetUserAdministrator = ChatMemberStatus.Administrator;
                            
                            if(replyUser.Id == Program.Bot.Id)
                                await bot.SendTextMessageAsync(chat.Id,
                                text: "Зачем назначать себе разбан, если я сама же здесь администратор🤔?",
                                replyToMessageId: msg.MessageId);
                            
                            else if(targetUser.Status == ChatMemberStatus.Administrator) {
                                await bot.SendTextMessageAsync(chatId: chat.Id,
                                    text: $"<b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> тоже администратор😑.",
                                    replyToMessageId: msg.MessageId,
                                    parseMode: ParseMode.Html,
                                    disableWebPagePreview: true);
                            }
                            else {
                                await bot.UnbanChatMemberAsync(chat.Id, replyUser.Id);
                                await bot.SendTextMessageAsync(chat.Id,
                                    text: $"<b>✅Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> вынесен из черного списка!</b>\nНадеюсь он не повторит свои ошибки🙂",
                                    replyToMessageId: msg.MessageId,
                                    parseMode: ParseMode.Html,
                                    disableWebPagePreview: true);
                            }
                        }
                    }
                }
                if(text.ToLower() == ".мут") {
                    ChatMember botAdmin = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                    ChatMember outputMember = await bot.GetChatMemberAsync(chat.Id, user.Id);
                    
                    var outputMemberStatus = outputMember.Status;
                    if(outputMember.Status == ChatMemberStatus.Creator)
                        outputMemberStatus = ChatMemberStatus.Administrator;
                    
                    if(botAdmin.Status != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "❕Я не являюсь администратором чата, поэтому я не могу совершить это действие",
                            replyToMessageId: msg.MessageId);
                    }
                    else if(outputMemberStatus != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> у вас недостаточно прав",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else if(chat.Type != ChatType.Supergroup) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> чат не является супергруппой",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else {
                        if(msg.ReplyToMessage == null) {
                            await bot.SendTextMessageAsync(chatId: chat.Id,
                                text: "<b>❗️Вы не указали участника, которого вы хотели заглушить!</b>\nЯ не способна глушить рандомного человека🙃",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html);
                        }
                        else {
                            var replyUser = msg.ReplyToMessage.From;
                            var targetUser = await bot.GetChatMemberAsync(chat.Id, replyUser.Id);
                            
                            var targetUserAdministrator = targetUser.Status;
                            if(targetUser.Status == ChatMemberStatus.Creator)
                                targetUserAdministrator = ChatMemberStatus.Administrator;
                            
                            if(replyUser.Id == Program.Bot.Id)
                                await bot.SendTextMessageAsync(chat.Id,
                                text: "Если я заглушу себя, то как пользователь бота будет потом использовать меня🤔?",
                                replyToMessageId: msg.MessageId);
                            
                            else if(targetUser.Status == ChatMemberStatus.Administrator) {
                                await bot.SendTextMessageAsync(chatId: chat.Id,
                                    text: $"<b>Заглушить <a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> я не могу, поскольку он администратор😑.\n\nСвои отношения обсуждайте в личках😠!",
                                    replyToMessageId: msg.MessageId,
                                    parseMode: ParseMode.Html,
                                    disableWebPagePreview: true);
                            }
                            else {
                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddMinutes(15), permissions: mutePermissions);
                                await bot.SendTextMessageAsync(chat.Id,
                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на 15 минут</b>\nЗа это время он должен успокоиться😇",
                                    replyToMessageId: msg.MessageId,
                                    parseMode: ParseMode.Html,
                                    disableWebPagePreview: true);
                            }
                        }
                    }
                }
                if(text.ToLower() == ".размут") {
                    ChatMember botAdmin = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                    ChatMember outputMember = await bot.GetChatMemberAsync(chat.Id, user.Id);
                    
                    var outputMemberStatus = outputMember.Status;
                    if(outputMember.Status == ChatMemberStatus.Creator)
                        outputMemberStatus = ChatMemberStatus.Administrator;
                    
                    if(botAdmin.Status != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "❕Я не являюсь администратором чата, поэтому я не могу совершить это действие",
                            replyToMessageId: msg.MessageId);
                    }
                    else if(outputMemberStatus != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> у вас недостаточно прав",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else if(chat.Type != ChatType.Supergroup) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> чат не является супергруппой",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else {
                        if(msg.ReplyToMessage == null) {
                            await bot.SendTextMessageAsync(chatId: chat.Id,
                                text: "<b>❗️Вы не указали участника, которого вы хотели вернуть в общение!</b>\nЯ не способна снимать мут с рандомного человека🙃",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html);
                        }
                        else {
                            var replyUser = msg.ReplyToMessage.From;
                            var targetUser = await bot.GetChatMemberAsync(chat.Id, replyUser.Id);
                            
                            var targetUserAdministrator = targetUser.Status;
                            if(targetUser.Status == ChatMemberStatus.Creator)
                                targetUserAdministrator = ChatMemberStatus.Administrator;
                            
                            if(replyUser.Id == Program.Bot.Id)
                                await bot.SendTextMessageAsync(chat.Id,
                                text: "Зачем мне снимать с себя мут, если я и так говорю свободно🤔?",
                                replyToMessageId: msg.MessageId);
                            
                            else if(targetUser.Status == ChatMemberStatus.Administrator) {
                                await bot.SendTextMessageAsync(chatId: chat.Id,
                                    text: $"<b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> является администратором чата😑.",
                                    replyToMessageId: msg.MessageId,
                                    parseMode: ParseMode.Html,
                                    disableWebPagePreview: true);
                            }
                            else {
                                try {
                                    await bot.PromoteChatMemberAsync(chat.Id, replyUser.Id);
                                }
                                catch(ApiRequestException exc) {
                                    if(exc.Message.Contains("bots can't add new chat members"))
                                        await bot.SendTextMessageAsync(chat.Id,
                                            text: $"<b>✅Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> теперь может свободно общаться</b>\nНадеюсь ссориться с кем-то он будет в личке🙂",
                                            replyToMessageId: msg.MessageId,
                                            parseMode: ParseMode.Html,
                                            disableWebPagePreview: true);
                                }
                            }
                        }
                    }
                }
                if(text.ToLower() == "/chatid") {
                    ChatMember botAdmin = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                    ChatMember outputMember = await bot.GetChatMemberAsync(chat.Id, user.Id);

                    var outputMemberStatus = outputMember.Status;
                    if(outputMember.Status == ChatMemberStatus.Creator)
                        outputMemberStatus = ChatMemberStatus.Administrator;
                    
                    if(botAdmin.Status != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "❕Я не являюсь администратором чата, поэтому я не могу совершить это действие",
                            replyToMessageId: msg.MessageId);
                    }
                    else if(outputMemberStatus != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> у вас недостаточно прав",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else {
                        await bot.SendTextMessageAsync(chat.Id,
                        text: $"<b>🆔ID данного чата: </b> <code>{chat.Id}</code>",
                        replyToMessageId: msg.MessageId,
                        parseMode: ParseMode.Html);
                    }
                }
                #endregion

                #region Admin commands with parameters
                if(text.ToLower().StartsWith(".бан @")) {
                    ChatMember botAdmin = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                    ChatMember outputMember = await bot.GetChatMemberAsync(chat.Id, user.Id);

                    var outputMemberStatus = outputMember.Status;
                    if(outputMember.Status == ChatMemberStatus.Creator)
                        outputMemberStatus = ChatMemberStatus.Administrator;

                    if(botAdmin.Status != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "❕Я не являюсь администратором чата, поэтому я не могу совершить это действие",
                            replyToMessageId: msg.MessageId);
                    }
                    else if(outputMemberStatus != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> у вас недостаточно прав",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else {
                        var userTemp = await TelegramApiMethods.GetUserByUsernameAsync(text.ToLower().Split(".бан @")[1].Split(" ")[0]);
                        if(userTemp == null) {
                            await bot.SendTextMessageAsync(chatId: chat.Id,
                                text: "<b>❗️Данный пользователь не найден по юзернейму!</b>",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html);
                        }
                        else {
                            try {
                                var targetUser = await bot.GetChatMemberAsync(chat.Id, userTemp.ID);

                                var targetUserAdministrator = targetUser.Status;
                                if(targetUser.Status == ChatMemberStatus.Creator)
                                    targetUserAdministrator = ChatMemberStatus.Administrator;
                                
                                if(userTemp.ID == Program.Bot.Id)
                                    await bot.SendTextMessageAsync(chat.Id,
                                    text: "Меня назначили администратором, чтобы потом забанить себя же🤡",
                                    replyToMessageId: msg.MessageId);
                                
                                else if(targetUser.Status == ChatMemberStatus.Administrator) {
                                    await bot.SendTextMessageAsync(chatId: chat.Id,
                                        text: $"Тебе бы стоило решить проблему с <b><a href=\"tg://user?id={userTemp.ID}\">{userTemp.first_name}</a></b> мирным способом😑.\n\nЯ не собираюсь вмешиваться в вашу войну😒",
                                        replyToMessageId: msg.MessageId,
                                        parseMode: ParseMode.Html,
                                        disableWebPagePreview: true);
                                }
                                else {
                                    try {
                                        await bot.BanChatMemberAsync(chat.Id, userTemp.ID);
                                        await bot.SendTextMessageAsync(chat.Id,
                                            text: $"💢Участник чата <b><a href=\"tg://user?id={userTemp.ID}\">{userTemp.first_name}</a></b> был изгнан из чата!",
                                            replyToMessageId: msg.MessageId,
                                            parseMode: ParseMode.Html,
                                            disableWebPagePreview: true);
                                    }
                                    catch(Exception exc) {
                                        await bot.SendTextMessageAsync(chat.Id,
                                            text: $"⚠️Возникла непредвиденная ошибка: <code>{exc.Message}</code>",
                                            replyToMessageId: msg.MessageId,
                                            parseMode: ParseMode.Html);
                                        await Program.WriteError($"TelegramBotApi in '.бан @' | ban by username: {exc.Message}");
                                    }
                                }
                            }
                            catch(Exception exc) {
                                await bot.SendTextMessageAsync(chat.Id,
                                    text: $"⚠️Возникла непредвиденная ошибка: <code>{exc.Message}</code>\nВозможно данный пользователь не был участником чата",
                                    replyToMessageId: msg.MessageId,
                                    parseMode: ParseMode.Html);
                                await Program.WriteError($"TelegramBotApi in '.бан @' | ban by username: {exc.Message}");
                            }
                        }
                    }
                }
                if(text.ToLower().StartsWith(".разбан @")) {
                    ChatMember botAdmin = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                    ChatMember outputMember = await bot.GetChatMemberAsync(chat.Id, user.Id);

                    var outputMemberStatus = outputMember.Status;
                    if(outputMember.Status == ChatMemberStatus.Creator)
                        outputMemberStatus = ChatMemberStatus.Administrator;

                    if(botAdmin.Status != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "❕Я не являюсь администратором чата, поэтому я не могу совершить это действие",
                            replyToMessageId: msg.MessageId);
                    }
                    else if(outputMemberStatus != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> у вас недостаточно прав",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else {
                        var userTemp = await TelegramApiMethods.GetUserByUsernameAsync(text.ToLower().Split(".разбан @")[1].Split(" ")[0]);
                        if(userTemp == null) {
                            await bot.SendTextMessageAsync(chatId: chat.Id,
                                text: "<b>❗️Данный пользователь не найден по юзернейму!</b>",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html);
                        }
                        else {
                            try {
                                var targetUser = await bot.GetChatMemberAsync(chat.Id, userTemp.ID);
                                var targetUserAdministrator = targetUser.Status;
                                if(targetUser.Status == ChatMemberStatus.Creator)
                                    targetUserAdministrator = ChatMemberStatus.Administrator;

                                if(userTemp.ID == Program.Bot.Id)
                                    await bot.SendTextMessageAsync(chat.Id,
                                    text: "Зачем назначать себе разбан, если я сама же здесь администратор🤔?",
                                    replyToMessageId: msg.MessageId);

                                else if(targetUser.Status == ChatMemberStatus.Administrator) {
                                    await bot.SendTextMessageAsync(chatId: chat.Id,
                                        text: $"<b><a href=\"tg://user?id={userTemp.ID}\">{userTemp.first_name}</a></b> тоже администратор😑.",
                                        replyToMessageId: msg.MessageId,
                                        parseMode: ParseMode.Html,
                                        disableWebPagePreview: true);
                                }
                                else {
                                    try {
                                        await bot.UnbanChatMemberAsync(chat.Id, userTemp.ID);
                                        await bot.SendTextMessageAsync(chat.Id,
                                            text: $"<b>✅Участник чата <b><a href=\"tg://user?id={userTemp.ID}\">{userTemp.first_name}</a></b> вынесен из черного списка!</b>\nНадеюсь он не повторит свои ошибки🙂",
                                            replyToMessageId: msg.MessageId,
                                            parseMode: ParseMode.Html,
                                            disableWebPagePreview: true);
                                    }
                                    catch(Exception exc) {
                                        await bot.SendTextMessageAsync(chat.Id,
                                            text: $"⚠️Возникла непредвиденная ошибка: <code>{exc.Message}</code>\nВозможно данный пользователь не был участником чата",
                                            replyToMessageId: msg.MessageId,
                                            parseMode: ParseMode.Html);
                                        await Program.WriteError($"TelegramBotApi in '.разбан @' | ban by username: {exc.Message}");
                                    }
                                }
                            }
                            catch(Exception exc) {
                                await Program.WriteError($"TelegramBotApi in '.разбан @' | ban by username: {exc.Message}");
                                await bot.SendTextMessageAsync(chat.Id,
                                    text: $"⚠️Возникла непредвиденная ошибка: <code>{exc.Message}</code>\nВозможно данный пользователь не был участником чата",
                                    replyToMessageId: msg.MessageId,
                                    parseMode: ParseMode.Html);
                            }
                        }
                    }
                }
                if(text.ToLower().StartsWith(".размут @")) {
                    ChatMember botAdmin = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                    ChatMember outputMember = await bot.GetChatMemberAsync(chat.Id, user.Id);

                    var outputMemberStatus = outputMember.Status;
                    if(outputMember.Status == ChatMemberStatus.Creator)
                        outputMemberStatus = ChatMemberStatus.Administrator;

                    if(botAdmin.Status != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "❕Я не являюсь администратором чата, поэтому я не могу совершить это действие",
                            replyToMessageId: msg.MessageId);
                    }
                    else if(outputMemberStatus != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> у вас недостаточно прав",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else if(chat.Type != ChatType.Supergroup) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> чат не является супергруппой",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else {
                        var userTemp = await TelegramApiMethods.GetUserByUsernameAsync(text.ToLower().Split(".размут @")[1].Split(" ")[0]);
                        if(userTemp == null) {
                            await bot.SendTextMessageAsync(chatId: chat.Id,
                                text: "<b>❗️Данный пользователь не найден по юзернейму!</b>",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html);
                        }
                        else {
                            try {
                                var targetUser = await bot.GetChatMemberAsync(chat.Id, userTemp.ID);
                                var targetUserAdministrator = targetUser.Status;
                                if(targetUser.Status == ChatMemberStatus.Creator)
                                    targetUserAdministrator = ChatMemberStatus.Administrator;

                                if(userTemp.ID == Program.Bot.Id)
                                    await bot.SendTextMessageAsync(chat.Id,
                                    text: "Зачем мне снимать с себя мут, если я и так говорю свободно🤔?",
                                    replyToMessageId: msg.MessageId);

                                else if(targetUser.Status == ChatMemberStatus.Administrator) {
                                    await bot.SendTextMessageAsync(chatId: chat.Id,
                                        text: $"<b><a href=\"tg://user?id={userTemp.ID}\">{userTemp.first_name}</a></b> тоже администратор😑.",
                                        replyToMessageId: msg.MessageId,
                                        parseMode: ParseMode.Html,
                                        disableWebPagePreview: true);
                                }
                                else {
                                    try {
                                        await bot.PromoteChatMemberAsync(chat.Id, userTemp.ID);
                                    }
                                    catch(Exception exc) {
                                        if(exc.Message.Contains("bots can't add new chat members"))
                                            await bot.SendTextMessageAsync(chat.Id,
                                                text: $"<b>✅Участник чата <b><a href=\"tg://user?id={userTemp.ID}\">{userTemp.first_name}</a></b> теперь может свободно общаться</b>\nНадеюсь ссориться с кем-то он будет в личке🙂",
                                                replyToMessageId: msg.MessageId,
                                                parseMode: ParseMode.Html,
                                                disableWebPagePreview: true);
                                        else {
                                            await Program.WriteError($"TelegramBotApi in '.размут @' | ban by username: {exc.Message}");
                                            await bot.SendTextMessageAsync(chat.Id,
                                                text: $"⚠️Возникла непредвиденная ошибка: <code>{exc.Message}</code>\nВозможно данный пользователь не был участником чата",
                                                replyToMessageId: msg.MessageId,
                                                parseMode: ParseMode.Html);                                       
                                        }
                                    }
                                }
                            }
                            catch(Exception exc) {
                                await Program.WriteError($"TelegramBotApi in '.размут @' | ban by username: {exc.Message}");
                                await bot.SendTextMessageAsync(chat.Id,
                                    text: $"⚠️Возникла непредвиденная ошибка: <code>{exc.Message}</code>\nВозможно данный пользователь не был участником чата",
                                    replyToMessageId: msg.MessageId,
                                    parseMode: ParseMode.Html);    
                            }
                        }
                    }
                }
                if(text.ToLower().StartsWith(".мут ")) {
                    ChatMember botAdmin = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                    ChatMember outputMember = await bot.GetChatMemberAsync(chat.Id, user.Id);

                    var outputMemberStatus = outputMember.Status;
                    if(outputMember.Status == ChatMemberStatus.Creator)
                        outputMemberStatus = ChatMemberStatus.Administrator;
                    
                    if(botAdmin.Status != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "❕Я не являюсь администратором чата, поэтому я не могу совершить это действие",
                            replyToMessageId: msg.MessageId);
                    }
                    else if(outputMemberStatus != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> у вас недостаточно прав",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else if(chat.Type != ChatType.Supergroup) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> чат не является супергруппой",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else {
                        if(msg.ReplyToMessage == null) {
                            await bot.SendTextMessageAsync(chatId: chat.Id,
                                text: "<b>❗️Вы не указали участника, которого вы хотели заглушить!</b>\nЯ не способна глушить рандомного человека🙃",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html);
                        }
                        else {
                            var replyUser = msg.ReplyToMessage.From;
                            var targetUser = await bot.GetChatMemberAsync(chat.Id, replyUser.Id);
                    
                            var targetUserAdministrator = targetUser.Status;
                            if(targetUser.Status == ChatMemberStatus.Creator)
                                targetUserAdministrator = ChatMemberStatus.Administrator;
                            
                            if(replyUser.Id == Program.Bot.Id)
                                await bot.SendTextMessageAsync(chat.Id,
                                text: "Если я заглушу себя, то как пользователь бота будет потом использовать меня🤔?",
                                replyToMessageId: msg.MessageId);
                            
                            else if(targetUser.Status == ChatMemberStatus.Administrator) {
                                await bot.SendTextMessageAsync(chatId: chat.Id,
                                    text: $"<b>Заглушить <a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> я не могу, поскольку он администратор😑.\n\nСвои отношения обсуждайте в личках😠!",
                                    replyToMessageId: msg.MessageId,
                                    parseMode: ParseMode.Html,
                                    disableWebPagePreview: true);
                            }
                            else {
                                var args = text.ToLower().Split(' ');
                                if(args.Length < 3) {
                                    await bot.SendTextMessageAsync(chat.Id,
                                        text: $"<b>❕Недостаточно аргументов, чтобы я могла зашлушить его</b>",
                                        replyToMessageId: msg.MessageId,
                                        parseMode: ParseMode.Html,
                                        disableWebPagePreview: true);
                                }
                                else {
                                    int times = 0;
                                    if(!int.TryParse(args[1], out times)) {
                                        await bot.SendTextMessageAsync(chat.Id,
                                            text: $"<b>❗️Вы не указали длительность глушения</b>",
                                            replyToMessageId: msg.MessageId,
                                            parseMode: ParseMode.Html,
                                            disableWebPagePreview: true);
                                    }
                                    else {
                                        switch (args[2]) {
                                            case "минут":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddMinutes(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} минут</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                            case "минуты":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddMinutes(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} минуты</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                            case "минуту":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddMinutes(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} минуту</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                                
                                            case "час":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddHours(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} час</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                            case "часа":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddHours(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} часа</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                            case "часов":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddHours(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} часов</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                            
                                            case "день":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddDays(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} день</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                            case "дня":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddDays(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} дня</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                            case "дней":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddDays(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} дней</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                                
                                            case "месяц":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddMonths(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} месяц</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                            case "месяца":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddMonths(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} месяца</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                            case "месяцев":
                                                await bot.RestrictChatMemberAsync(chat.Id, replyUser.Id, untilDate: DateTime.UtcNow.AddMonths(times), permissions: mutePermissions);
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>🔇Участник чата <b><a href=\"tg://user?id={replyUser.Id}\">{replyUser.FirstName}</a></b> был заглушен на {times} месяцев</b>\nЗа это время он должен успокоиться😇",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                            
                                            default:
                                                await bot.SendTextMessageAsync(chat.Id,
                                                    text: $"<b>\"{args[2]}\" не является временем❌</b>",
                                                    replyToMessageId: msg.MessageId,
                                                    parseMode: ParseMode.Html,
                                                    disableWebPagePreview: true);
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if(text.ToLower().StartsWith(".привязать репорт-хранилище ")) {
                    ChatMember botAdmin = await bot.GetChatMemberAsync(chat.Id, Program.Bot.Id);
                    ChatMember outputMember = await bot.GetChatMemberAsync(chat.Id, user.Id);

                    var outputMemberStatus = outputMember.Status;
                    if(outputMember.Status == ChatMemberStatus.Creator)
                        outputMemberStatus = ChatMemberStatus.Administrator;
                    
                    if(botAdmin.Status != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "❕Я не являюсь администратором чата, поэтому я не могу совершить это действие",
                            replyToMessageId: msg.MessageId);
                    }
                    else if(outputMemberStatus != ChatMemberStatus.Administrator) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> у вас недостаточно прав",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else if(chat.Type != ChatType.Supergroup) {
                        await bot.SendTextMessageAsync(chatId: chat.Id,
                            text: "<b>🚫Команда отклонена:</b> чат не является супергруппой",
                            replyToMessageId: msg.MessageId,
                            parseMode: ParseMode.Html);
                    }
                    else {
                        var parserOperation = long.TryParse(text.ToLower().Split(".привязать репорт-хранилище ")[1], out var chatId);
                        var config = new LauraChatManager.Types.Chat();
                        foreach(var chatConfig in Program.Chats) {
                            if(chatConfig.Id == chat.Id) {
                                config = chatConfig;
                                break;
                            }
                        }
                        if(!parserOperation) {
                            await bot.SendTextMessageAsync(chat.Id,
                                text: "<b>💢Неверно указан ID-чата</b>",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html);
                        }
                        else if(config.ReportStorage != 0) {
                            var chatR = await bot.GetChatAsync(config.ReportStorage);
                            await bot.SendTextMessageAsync(chat.Id,
                                text: $"💢Ваш чат уже использует репорт-хранилище <b>\"{chatR.Title}\"</b> | <code>{chatR.Id}</code>!\nПопробуйте этот чат отвязать от репорт-хранилища или привяжите репорт-хранилище в другом чате",
                                replyToMessageId: msg.MessageId,
                                parseMode: ParseMode.Html);
                        }
                        else {
                            try {
                                var chatR = await bot.GetChatAsync(config.ReportStorage);
                                if(chatR.Type != ChatType.Supergroup) {
                                    var message = "<b>⛔️Этот чат не может быть использован в качестве репорт-хранилища!</b>\n";
                                    switch(chatR.Type) {
                                        case ChatType.Group:
                                            message += "<i>Данный чат не является супергруппой, сделайте данный чат супергруппой во избежании проблем</i>";
                                            break;
                                        case ChatType.Channel:
                                            message += "<i>Канал не может быть использован в качестве репорт-хранилища</i>";
                                            break;
                                        default:
                                            message += "<i>Вы указали ID пользователя, а не чата</i>";
                                            break;
                                    }
                                    if(chatR.IsForum.HasValue && chatR.IsForum.Value) 
                                        message = "<b>⛔️Этот чат не может быть использован в качестве репорт-хранилища!</b>\n<i>Данный чат является форумом</i>";
                                    await bot.SendTextMessageAsync(chat.Id,
                                        text: message,
                                        parseMode: ParseMode.Html,
                                        replyToMessageId: msg.MessageId);
                                }
                                else {
                                    var end = DateTime.UtcNow.AddMinutes(5);
                                    await ReportStorageReqStorage.CreateReportStorageRequest(new() {TargetChat = chat.Id, OutputChat = chatR.Id, Expire = end});
                                    await bot.SendTextMessageAsync(chatId, 
                                        $"🔰Чат \"{chat.Title}\" | <code>{chat.Id}</code> кинул запрос на привязку данного чата в качестве репорт-хранилища.\nВы принимаете данный запрос?\n\nЗапрос истечет {end.ToString(Configuration.Config.StandartDateFormat, CultureInfo.InvariantCulture)} по времени UTC",
                                        parseMode: ParseMode.Html,
                                        replyMarkup: ReportStorageButtons.GetRequestButton());
                                    await Task.Delay(350);
                                    await bot.SendTextMessageAsync(chat.Id,
                                        text: "✅Запрос был успешно отравлен!",
                                        replyToMessageId: msg.MessageId);
                                }
                            }
                            catch(Exception) {

                            }
                        }
                    }
                }
                #endregion
                
                if(text.ToLower() == ".состояние") {
                    TimeSpan time = Program.NextLarxUpdate - DateTime.UtcNow;
                    await bot.SendTextMessageAsync(
                        chatId: chat.Id,
                        text:
@$"<b>Моё состояние🌸:</b>
- Пинг: <code>{Program.Ping} мс</code>
- Пинг базы: <code>{Program.DbPing} мс</code>
- RAM: <code>{Program.RamUsage} МБ</code>

<b><i>Следующее обновление данных: </i></b>
<code>{time.Hours} часов {time.Minutes} минут {time.Seconds} секунд</code>",
                        replyToMessageId: msg.MessageId,
                        parseMode: ParseMode.Html);
                }
            }
        }
    }
}
