using MySql.Data.MySqlClient;

using LauraChatManager.Types;
using LauraChatManager.Configuration;
using System.Globalization;
using System.Data;

namespace LauraChatManager.Handles.Other;

public class RatingController {
    public static async Task<Dictionary<long, List<RatingControl>>> GetRatingControls(){
        Dictionary<long, List<RatingControl>> temp = new();
        using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)) {
            try {
                await mySql.OpenAsync();
                var query = $"SELECT * FROM rating_control";
                using(MySqlCommand myCmd = new(query, mySql)) {
                    using(MySqlDataReader myReader = (MySqlDataReader) await myCmd.ExecuteReaderAsync()) {
                        if(myReader.HasRows) {
                            while(await myReader.ReadAsync()) {
                                var userId = long.Parse(myReader[1].ToString());
                                var targetUserId = long.Parse(myReader[2].ToString());
                                var count = int.Parse(myReader[3].ToString());
                                var expire = DateTime.ParseExact(myReader[4].ToString(), Config.StandartDateFormat, CultureInfo.InvariantCulture);
                                if(temp.ContainsKey(userId)) {
                                    temp[userId].Add(new() {
                                        TargetUser = targetUserId,
                                        Count = count,
                                        Expire = expire
                                    });
                                }
                                else {
                                    List<RatingControl> temp2 = new();
                                    temp2.Add(new() {
                                        TargetUser = targetUserId,
                                        Count = count,
                                        Expire = expire
                                    });
                                    temp.Add(userId, temp2);
                                }
                            }
                        }
                    }
                }
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
        return temp;
    }
    public static async Task CreateRatingControl(long userId, long targetUserId) {
        using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)) {
            try {
                await mySql.OpenAsync();
                var expire = DateTime.UtcNow.AddDays(3);
                var query = $"INSERT INTO rating_control(id, userId, targetUserId, count, expire) VALUES(0, {userId}, {targetUserId}, 1, '{expire.ToString(Config.StandartDateFormat)}')";
                using(MySqlCommand myCmd = new(query, mySql))
                    await myCmd.ExecuteNonQueryAsync();
                await mySql.CloseAsync();
                for(int i = 0; i < Program.Users.Count; i++)
                    if(Program.Users[i].Id == userId)
                        Program.Users[i].RatingControls.Add(new() {
                            TargetUser = targetUserId,
                            Count = 1,
                            Expire = expire
                        });
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
    public static async Task DeleteRatingControl(long userId, long targetUserId) {
        using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)) {
            try {
                await mySql.OpenAsync();
                var query = $"DELETE FROM rating_control WHERE targetUserId = {targetUserId} AND userId = {userId}";
                using(MySqlCommand myCmd = new(query, mySql))
                    await myCmd.ExecuteNonQueryAsync();
                await mySql.CloseAsync();
                for(int i = 0; i < Program.Users.Count; i++)
                    if(Program.Users[i].Id == userId) {
                        for(int j = 0; j < Program.Users[i].RatingControls.Count; i++)
                            if(Program.Users[i].RatingControls[j].TargetUser == targetUserId) {
                                    Program.Users[i].RatingControls.RemoveAt(j);
                                    break;
                                }
                        break;
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
    }
    public static async Task UpdateRatingControl(long userId, long targetUserId) {
        using(MySqlConnection mySql = new(Config.DB_URL + Config.UsersData)) {
            try {
                var new_count = 0;
                for(int i = 0; i < Program.Users.Count; i++) {
                    if(Program.Users[i].Id == userId) {
                        for(int j = 0; j < Program.Users[i].RatingControls.Count; i++) {
                            if(Program.Users[i].RatingControls[j].TargetUser == targetUserId) {
                                new_count = Program.Users[i].RatingControls[j].Count + 1;
                                Program.Users[i].RatingControls[j].Count = new_count;
                                break;
                            }
                        }
                        break;
                    }
                }
                await mySql.OpenAsync();
                var date = DateTime.UtcNow.AddDays(3);
                var query = $"UPDATE rating_control SET count = {new_count} WHERE userId = {userId} AND targetUserId = {targetUserId}";
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
}