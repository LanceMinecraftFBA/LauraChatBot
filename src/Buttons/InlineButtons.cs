using Telegram.Bot.Types.ReplyMarkups;
using LauraChatManager.Types;
using LauraChatManager.Types.Enums;

namespace LauraChatManager.Buttons;

public class ReportStorageButtons {
    public InlineKeyboardMarkup GetRequestButton() {
        InlineKeyboardMarkup markup = new(new[] {
            new[] {
                InlineKeyboardButton.WithCallbackData("✅Принять", "accept_request"),
                InlineKeyboardButton.WithCallbackData("❌Отклонить", "reject_request")
            }
        });
        return markup;
    }
}

public class CaptchaInlineButtons {
    
}

public class MenuInlineButtons {
    public InlineKeyboardMarkup GetMainMenuMarkup() {
        InlineKeyboardMarkup markup = new(new[] {
                new[] {
                    InlineKeyboardButton.WithCallbackData("Варны📨", "warns"),
                    InlineKeyboardButton.WithCallbackData("Антирейд🛡️", "antiraid") 
                },
                new[] {
                    InlineKeyboardButton.WithCallbackData("Кастомизация🎭", "customization")
                },
                new[] {
                    InlineKeyboardButton.WithCallbackData("🌟Премиум-план", "premium-plan")     
                }
        });
        return markup;
    }
    public InlineKeyboardMarkup GetWarnsMenuMarkup(Chat config)
    {
        var pun = "мут🔇";
        switch (config.Punish) {
            case PunishType.Ban:
                pun = "бан💢";
                break;
            default:
                break;
        }
        InlineKeyboardMarkup markup = new(new[] {
            new[] {
                InlineKeyboardButton.WithCallbackData($"Макс. варнов: {config.MaxWarns}", "change-max-warns")
            },
            new[] {
                InlineKeyboardButton.WithCallbackData($"Наказание: {pun}", "change-pun") 
            },
            new[] {
                InlineKeyboardButton.WithCallbackData("Назад🔙", "back"), 
            }
        });
        return markup;
    }
}
