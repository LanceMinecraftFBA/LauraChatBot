using Telegram.Bot.Types.ReplyMarkups;

namespace LauraChatManager.Buttons;

public class Buttons {
    public static ReplyKeyboardMarkup GetUserMenu() {
        var buttons = new ReplyKeyboardMarkup(new[] {
                new KeyboardButton[] {
                    new KeyboardButton("О боте🧸"),
                    KeyboardButton.WithRequestChat("Выбрать чат⚙️", new() {BotIsMember = true, BotAdministratorRights = new() {CanDeleteMessages = true, CanRestrictMembers = true, CanManageChat = true}, UserAdministratorRights = new() {CanChangeInfo = true, CanDeleteMessages = true, CanInviteUsers = true, CanRestrictMembers = true, CanPinMessages = true, CanManageChat = true}, ChatIsChannel = false})
                }
            });
        buttons.ResizeKeyboard = true;
        return buttons;
    }
}
