using System.Globalization;
using LauraChatManager.Configuration;
using LauraChatManager.Types;

namespace LauraChatManager.Handles.Other {
    public class DifferenceDetecter {
        public string Args;
        public int ArgsCount;

        public static DifferenceDetecter GetDifferenceForChatConfig(Chat _new, Chat old) {
            var args = "SET";
            var count = 0;
            if(_new.AsActive != old.AsActive)
                switch (_new.AsActive)
                {
                    case false:
                        args += $" as_active = {0}";
                        count += 1;
                        break;
                    case true:
                        args += $" as_active = {1}";
                        count += 1;
                        break;
                }
            if(_new.DetectUrl != old.DetectUrl) {
                if(args != "SET")
                    args += ",";
                switch (_new.DetectUrl)
                {
                    case false:
                        args += $" detect_url = {0}";
                        break;
                    case true:
                        args += $" detect_url = {1}";
                        break;
                }
                count += 1;
            }
            if(_new.IsReceive != old.IsReceive) {
                if(args != "SET")
                    args += ",";
                switch (_new.IsReceive)
                {
                    case false:
                        args += $" receive_news = {0}";
                        break;
                    case true:
                        args += $" receive_news = {1}";
                        break;
                }
                count += 1;
            }
            if(_new.NoBadWordsActive != old.NoBadWordsActive) {
                if(args != "SET")
                    args += ",";
                switch (_new.DetectUrl)
                {
                    case false:
                        args += $" nbw_active = {0}";
                        break;
                    case true:
                        args += $" nbw_active = {1}";
                        break;
                }
                count += 1;
            }
            if(_new.Gmt != old.Gmt) {
                if(args != "SET")
                    args += ",";
                if(_new.Gmt != null)
                    args += $" gmt = '{_new.Gmt}'";
                else
                    args += $" gmt = DEFAULT";
                count += 1;
            }
            if(_new.Night != old.Night) {
                if(args != "SET")
                    args += ",";
                if(_new.Night != null)
                    args += $" night = '{_new.Night}'";
                else
                    args += $" night = DEFAULT";
                count += 1;
            }
            if(_new.Rules != old.Rules) {
                if(args != "SET")
                    args += ",";
                if(_new.Rules != null)
                    args += $" rules = '{_new.Rules}'";
                else
                    args += $" rules = DEFAULT";
                count += 1;
            }
            if(_new.State != old.State) {
                if(args != "SET")
                    args += ",";
                if(_new.State != null)
                    args += $" state = '{_new.State}'";
                else
                    args += $" state = DEFAULT";
                count += 1;
            }
            if(_new.ChSticker != old.ChSticker) {
                if(args != "SET")
                    args += ",";
                if(_new.ChSticker != null)
                    args += $" ch_sticker = '{_new.ChSticker}'";
                else
                    args += $" ch_sticker = DEFAULT";
                count += 1;
            }
            if(_new.CustomHello != old.CustomHello) {
                if(args != "SET")
                    args += ",";
                if(_new.CustomHello != null)
                    args += $" custom_hello = '{_new.CustomHello}'";
                else
                    args += $" custom_hello = DEFAULT";
                count += 1;
            }
            if(_new.Punish != old.Punish) {
                if(args != "SET")
                    args += ",";
                args += $" custom_hello = '{_new.Punish.ToString()}'";
                count += 1;
            }
            if(_new.Minutes != old.Minutes) {
                if(args != "SET")
                    args += ",";
                args += $" cp_minutes = {_new.Minutes}";
                count += 1;
            }
            if(_new.Antispam != old.Antispam) {
                if(args != "SET")
                    args += ",";
                args += $" antispam = {_new.Antispam}";
                count += 1;
            }
            if(_new.MaxWarns != old.MaxWarns) {
                if(args != "SET")
                    args += ",";
                args += $" max_warns = {_new.MaxWarns}";
                count += 1;
            }
            if(_new.StExpire != old.StExpire) {
                if(args != "SET")
                    args += ",";
                if(_new.StExpire != null)
                    args += $" st_expire = '{_new.StExpire.Value.ToString(Config.StandartDateFormat, CultureInfo.InvariantCulture)}'";
                else
                    args += $" st_expire = DEFAULT";
                count += 1;
            }
            if(_new.ChatState != old.ChatState) {
                if(args != "SET")
                    args += ",";
                args += $" chat_state = {_new.ChatState.ToString()}";
                count += 1;
            }
            if(_new.StateOwner != old.StateOwner) {
                if(args != "SET")
                    args += ",";
                if(_new.StateOwner != 0)
                    args += $" state_owner = {_new.StateOwner}";
                else
                    args += $" state_owner = DEFAULT";
                count += 1;
            }
            if(_new.CaptchaButton != old.CaptchaButton) {
                if(args != "SET")
                    args += ",";
                args += $" captcha_button = '{_new.CaptchaButton.ToString()}'";
                count += 1;
            }
            if(_new.ReportStorage != old.ReportStorage) {
                if(args != "SET")
                    args += ",";
                args += $" report_storage = {_new.ReportStorage}";
                count += 1;
            }
            if(_new.SettingsState != old.SettingsState) {
                if(args != "SET")
                    args += ",";
                if(_new.SettingsState != null)
                    args += $" settings_state = '{_new.SettingsState}'";
                else
                    args += $" csettings_state = DEFAULT";
                count += 1;
            }
            if(_new.MaxCaptchaAttemps != old.MaxCaptchaAttemps) {
                if(args != "SET")
                    args += ",";
                args += $" cp_ma = {_new.MaxCaptchaAttemps}";
                count += 1;
            }

            if(_new.NotifComment != old.NotifComment) {
                if(args != "SET")
                    args += ",";
                args += $" not_comment = '{_new.NotifComment}'";
                count += 1;
            }
            return new() { Args = args, ArgsCount = count };
        }
        public static DifferenceDetecter GetDifferenceForUser(User _new, User old) {
            var args = "SET";
            var count = 0;
            if(_new.Rating != old.Rating) {
                args += $" rating = {_new.Rating}";
                count += 1;
            }
            if(_new.BlockEnd != old.BlockEnd) {
                if(args != "SET")
                    args += ",";
                if(_new.BlockEnd != null)
                    args += $" block_end = {_new.BlockEnd.Value.ToString(Config.StandartDateFormat, CultureInfo.InvariantCulture)}";
                else
                    args += $" block_end = DEFAULT";
                count += 1;
            }
            if(_new.IsBlocked != old.IsBlocked) {
                if(args != "SET")
                    args += ",";
                switch (_new.IsBlocked)
                {
                    case true:
                        args += $" is_blocked = 1";
                        break;
                    case false:
                        args += $" is_blocked = 0";
                        break;
                }
                count += 1;
            }
            if(_new.IsRaider != old.IsRaider) {
                if(args != "SET")
                    args += ",";
                switch (_new.IsRaider)
                {
                    case true:
                        args += $" is_raider = 1";
                        break;
                    case false:
                        args += $" is_raider = 0";
                        break;
                }
                count += 1;
            }
            if(_new.IsFbaEnemy != old.IsFbaEnemy) {
                if(args != "SET")
                    args += ",";
                switch (_new.IsFbaEnemy)
                {
                    case true:
                        args += $" is_fbaenemy = 1";
                        break;
                    case false:
                        args += $" is_fbaenemy = 0";
                        break;
                }
                count += 1;
            }
            if(_new.Nickname != old.Nickname)
            {
                if(args != "SET")
                    args += ",";
                if(_new.Nickname != null)
                    args += $" nickname = '{_new.Nickname}'";
                else
                    args += " nickname = DEFAULT";
                count += 1;
            }
            if(_new.IsReceiving != old.IsReceiving) {
                if(args != "SET")
                    args += ",";
                switch (_new.IsReceiving)
                {
                    case true:
                        args += $" isReceive = 1";
                        break;
                    case false:
                        args += $" isReceive = 0";
                        break;
                }
                count += 1;
            }
            return new() { Args = args, ArgsCount = count };
        }
    }
}
