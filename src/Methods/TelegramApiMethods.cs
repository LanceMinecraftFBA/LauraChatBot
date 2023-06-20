using Telegram.Bot.Types.Enums;
using TL;

namespace LauraChatManager.Methods;

public class TelegramApiMethods {
        public static async Task<User?> GetRandomMemberAsync(long chatId, ChatType chatType, string? username = null)
        {
            await Program.Client.LoginBotIfNeeded(Configuration.Config.Token);
            var chatFull = new Messages_ChatFull();
            Contacts_ResolvedPeer peer = new Contacts_ResolvedPeer();
            if (chatType == ChatType.Supergroup)
            {
                chatId = Convert.ToInt64(chatId.ToString().Split("-100")[1]);
                Random random = new Random();
                var acs_h = Program.ChannelBase[chatId];
                var chan = new InputChannel(chatId, acs_h);
                var channel = await Program.Client.Channels_GetParticipants(chan, new ChannelParticipantsRecent());
                var resul = random.Next(channel.users.Count);
                var users = new List<TL.User>();
                for (int i = 0; i < channel.participants.Length; i++)
                {
                    for (int j = 0; j < channel.users.Count; j++)
                    {
                        if (channel.participants[i].UserID == channel.users[channel.users.ElementAt(j).Key].ID)
                            users.Add(channel.users[channel.users.ElementAt(j).Key]);
                    }
                }
                var user = channel.users[channel.users.ElementAt(resul).Key];
                return user;
            }
            else if (chatType == ChatType.Group)
            {
                Random random = new Random();
                chatFull = await Program.Client.Messages_GetFullChat(chatId);
                var resul = random.Next(chatFull.users.Count);
                var user = chatFull.users[chatFull.users.ElementAt(resul).Key];
                return user;
            }
            else
            {
                return null;
            }
        }
        public static async Task<List<TL.User>> GetDeletedMembersAsync(long chatId, ChatType chatType)
        {
            try
            {
                await Program.Client.LoginBotIfNeeded(Configuration.Config.Token);
                var chatFull = new Messages_ChatFull();
                Contacts_ResolvedPeer peer = new Contacts_ResolvedPeer();
                if (chatType == ChatType.Supergroup)
                {
                    chatId = Convert.ToInt64(chatId.ToString().Split("-100")[1]);
                    Random random = new Random();
                    var acs_h = Program.ChannelBase[chatId];
                    var chan = new InputChannel(chatId, acs_h);
                    var channel = await Program.Client.Channels_GetParticipants(chan, new ChannelParticipantsRecent());
                    var list = channel.participants;
                    var users = new List<TL.User>();
                    var deleted = new List<TL.User>();
                    for (int i = 0; i < list.Length; i++)
                    {
                        for(int j = 0; j < channel.users.Count; j++)
                        {
                            if (list[i].UserID == channel.users[channel.users.ElementAt(j).Key].ID)
                                users.Add(channel.users[channel.users.ElementAt(j).Key]);
                        }
                    }
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].flags.HasFlag(TL.User.Flags.deleted))
                        {
                            deleted.Add(users[i]);
                        }
                    }
                    return deleted;
                }
                else if (chatType == ChatType.Group)
                {
                    Random random = new Random();
                    chatFull = await Program.Client.Messages_GetFullChat(chatId);
                    var resul = random.Next(chatFull.users.Count);
                    var list = new List<TL.User>();
                    for (int i = 0; i < chatFull.users.Count; i++)
                    {
                        if (chatFull.users[chatFull.users.ElementAt(i).Key].flags.HasFlag(TL.User.Flags.deleted))
                        {
                            list.Add(chatFull.users[chatFull.users.ElementAt(i).Key]);
                        }
                    }
                    return list;
                }
                else
                {
                    return new();
                }
            }
            catch(Exception exc)
            {
                await Program.WriteError("Error Occured in GetDeletedMembersAsync(): " + exc.Message);
                return new();
            }
        }
        public static async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                await Program.Client.LoginBotIfNeeded(Configuration.Config.Token);
                var user = await Program.Client.Contacts_ResolveUsername(username);
                return user.User;
            }
            catch(Exception exc)
            {
                await Program.WriteError("Error Occured in GetUserByUsernameAsync(): " + exc.Message);
                return null;
            }
        }
}
