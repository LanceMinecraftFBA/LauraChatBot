using System.Data;
using System.Globalization;
using LauraChatManager.Configuration;
using LauraChatManager.Types;
using MySql.Data.MySqlClient;

namespace LauraChatManager.Handles.Other {
    public class UsersDataTables {
        private static string _valuesWarns = @"(
id INT PRIMARY KEY AUTO_INCREMENT,
chatId VARCHAR(255) NOT NULL,
reason VARCHAR(200) NOT NULL,
date VARCHAR(255) NOT NULL,
expire VARCHAR(255) NOT NULL)";
        private static string _default = "DEFAULT";

        public static async Task<List<User>> GetUsers() {
            List<User> list = new();
            using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)) {
                try {
                    await mySql.OpenAsync();
                    var query = $"SELECT * FROM users";
                    using(MySqlCommand myCmd = new(query, mySql)) {
                        using(MySqlDataReader myReader = (MySqlDataReader)await myCmd.ExecuteReaderAsync()) {
                            if(!myReader.HasRows) {
                                await mySql.CloseAsync();
                                await Program.WriteDebbug("Users table is empty");
                            }
                            else {
                                while(await myReader.ReadAsync()) {
                                    var temp = new User(){Id = long.Parse(myReader[1].ToString()), IsBlocked = (myReader[3].ToString() != "0"), IsRaider = (myReader[5].ToString() != "0"), IsFbaEnemy = (myReader[6].ToString() != "0"), Rating = long.Parse(myReader[7].ToString()), IsReceiving = (myReader[8].ToString() != "0")};
                                    if(myReader[2].ToString() != "None")
                                        temp.Nickname = myReader[2].ToString();
                                    if(myReader[3].ToString() != "0")
                                        temp.IsBlocked = true;
                                    if(myReader[4].ToString() != "None")
                                        temp.BlockEnd = DateTime.ParseExact(myReader[4].ToString(), Config.StandartDateFormat, CultureInfo.InvariantCulture);
                                    list.Add(temp);
                                }
                                await mySql.CloseAsync();
                                await Program.WriteDebbug("Users list was loaded");
                            }
                        }
                    }
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
            return list;
        }
        public static async Task InsertUser(long userId) {
            using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)) {
                try {
                    await mySql.OpenAsync();
                    var query = @$"INSERT INTO users(id, userId, nickname, is_blocked, block_end, is_raider, is_fbaenemy, rating, isReceive) VALUES(
0, {userId}, {_default}, {_default}, {_default}, {_default}, {_default}, {_default}, {_default})";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    Program.Users.Add(new() {Id = userId});
                    await Program.WriteDebbug($"New user '{userId}'");
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }
        public static async Task UpdateUser(User _new) {
            var old = new User();
            foreach (var oldest in Program.Users)
                if(oldest.Id == _new.Id) {
                    old = oldest;
                    break;
                }
            using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)) {
                try {
                    await mySql.OpenAsync();
                    var diff = DifferenceDetecter.GetDifferenceForUser(_new, old);
                    var query = "UPDATE users " + diff.Args + "WHERE userId = " + _new.Id;
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    for(int i = 0; i < Program.Users.Count; i++)
                        if(Program.Users[i].Id == _new.Id) {
                            Program.Users[i] = _new;
                            break;
                        }
                    await Program.WriteDebbug($"User '{_new.Id}' has {diff.ArgsCount} changes");
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }
        public static async Task DeleteUser(long userId) {
            using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)) {
                try {
                    await mySql.OpenAsync();
                    var query = $"DELETE FROM users WHERE user_id = {userId}";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                    for(int i = 0; i < Program.Users.Count; i++)
                        if(Program.Users[i].Id == userId)
                            Program.Users.Remove(Program.Users[i]);
                    await Program.WriteDebbug($"User '{userId}' was removed");
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }


        public static async Task<WarnUser> GetUserWarns(long userId) {
            WarnUser user = new();
            user.Warns = new();
            using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)) {
                try {
                    await mySql.OpenAsync();
                    var query = $"SELECT * FROM {userId}_warns";
                    using(MySqlCommand myCmd = new(query, mySql)) {
                        using(MySqlDataReader myReader = (MySqlDataReader)await myCmd.ExecuteReaderAsync()) {
                            if(!myReader.HasRows) {
                                await Program.WriteDebbug("User Warns is empty");
                                await mySql.CloseAsync();
                            }
                            else {
                                user.UserId = userId;
                                while(await myReader.ReadAsync()) {
                                    var id = int.Parse(myReader[0].ToString());
                                    var chatId = long.Parse(myReader[1].ToString());
                                    var reason = myReader[2].ToString();
                                    var date = DateTime.ParseExact(myReader[3].ToString(), Config.StandartDateFormat, CultureInfo.InvariantCulture);
                                    var expire = DateTime.ParseExact(myReader[4].ToString(), Config.StandartDateFormat, CultureInfo.InvariantCulture);

                                    if(user.Warns.ContainsKey(chatId))
                                        user.Warns[chatId].Add(new() {Id = id, Reason = reason, Date = date, Expire = expire});
                                    else
                                    {
                                        user.Warns.Add(chatId, new List<Warn>());
                                        user.Warns[chatId].Add(new() {Id = id, Reason = reason, Date = date, Expire = expire});
                                    }
                                }
                                await mySql.CloseAsync();
                                await Program.WriteDebbug($"Warns selected for User '{userId}'");
                            }
                        }
                    }
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
            return user;
        }
        public static async Task CreateUserWarns(long userId) {
            using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)){
                try{
                    await mySql.OpenAsync();
                    var query = $"CREATE TABLE {userId}_warns{_valuesWarns}";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }
        public static async Task DeleteUserWarns(long userId){
            using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)){
                try{
                    await mySql.OpenAsync();
                    var query = $"DROP TABLE {userId}_warns";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }
        
        public static async Task InsertNewWarn(long userId, long chatId, string reason) {
            using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)) {
                try {
                    await mySql.OpenAsync();
                    var query = $"INSERT INTO {userId}_warns(id, chatId, reason, date, expire) VALUES(0, {chatId}, '{reason}', '{DateTime.UtcNow.ToString(Config.StandartDateFormat)}', '{DateTime.UtcNow.AddDays(3).ToString(Config.StandartDateFormat)}')";
                    using(MySqlCommand myCmd = new(query, mySql))
                        await myCmd.ExecuteNonQueryAsync();
                    await mySql.CloseAsync();
                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }
        public static async Task DeleteUserWarn(int warnId, long userId, long chatId) {
            using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)) {
                try {

                }
                catch(MySqlException exc) {
                    await Program.WriteError("MySql Exception: " + exc.Message);
                }
                finally {
                    if(mySql.State == ConnectionState.Open)
                        await mySql.CloseAsync();
                }
            }
        }
    }
}
