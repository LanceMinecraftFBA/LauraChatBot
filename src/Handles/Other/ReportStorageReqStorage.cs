using LauraChatManager.Types;
using LauraChatManager.Configuration;

using MySql.Data.MySqlClient;
using System.Data;
using System.Globalization;

namespace LauraChatManager.Handles.Other;

public class ReportStorageReqStorage {
    public static async Task<List<ReportStorageRequest>> GetReportStorageRequests() {
        List<ReportStorageRequest> temp = new();
        using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
            try {
                await mySql.OpenAsync();
                var query = $"SELECT * FROM report_storage_request";
                using(MySqlCommand myCmd = new(query, mySql)) {
                    using(MySqlDataReader myReader = (MySqlDataReader) await myCmd.ExecuteReaderAsync()) {
                        if(myReader.HasRows) {
                            while(await myReader.ReadAsync()) {
                                temp.Add(new() {TargetChat = long.Parse(myReader[1].ToString()), OutputChat = long.Parse(myReader[2].ToString()), Expire = DateTime.ParseExact(myReader[3].ToString(), Config.StandartDateFormat, CultureInfo.InvariantCulture)});
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
            return temp;
        }
    }
    public static async Task DeleteReportStorageRequest(ReportStorageRequest form) {
        using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
            try {
                await mySql.OpenAsync();
                var query = $"DELETE FROM report_storage_request WHERE outputChat = {form.OutputChat} AND targetChat = {form.TargetChat}";
                using(MySqlCommand myCmd = new(query, mySql))
                    await myCmd.ExecuteNonQueryAsync();
                for(int i = 0; i < Program.Collections.RsRequests.Count; i++) {
                    if(Program.Collections.RsRequests[i].OutputChat == form.OutputChat) {
                        Program.Collections.RsRequests.RemoveAt(i);
                        break;
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
    }
    public static async Task CreateReportStorageRequest(ReportStorageRequest form) {
        using(MySqlConnection mySql = new(Config.DB_URL + Config.ChatsData)) {
            try {
                await mySql.OpenAsync();
                var query = $"INSERT INTO report_storage_request(id, targetChat, outputChat, expire) VALUES(0, {form.TargetChat}, {form.OutputChat}, '{form.Expire.ToString(Config.StandartDateFormat)}')";
                using(MySqlCommand myCmd = new(query, mySql))
                    await myCmd.ExecuteNonQueryAsync();
                await mySql.CloseAsync();
                Program.Collections.RsRequests.Add(form);
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