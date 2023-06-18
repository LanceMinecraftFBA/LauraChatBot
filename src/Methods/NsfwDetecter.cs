using System.Net;
using Newtonsoft.Json;
using LauraChatManager.Types;
using LauraChatManager.Configuration;

namespace LauraChatManager.Methods;

public class NsfwDetecter {
    public static readonly string header = "multipart/form-data";

    public static async Task<NsfwObject> GetNsfwScan(string path) {
        try {
            using (HttpClient httpClient = new HttpClient())
            {
                using (MultipartFormDataContent formData = new MultipartFormDataContent())
                {
                    FileStream imageStream = File.OpenRead(path);
                    formData.Add(new StreamContent(imageStream), "image", path);

                    HttpResponseMessage response = await httpClient.PostAsync(Config.LocalNswfDetector, formData);
                    string responseText = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<NsfwObject>(responseText);
                }
            }
        }
        catch {
            return new();
        }
    }
}
