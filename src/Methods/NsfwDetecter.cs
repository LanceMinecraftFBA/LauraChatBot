using System.Net;
using Newtonsoft.Json;
using LauraChatManager.Types;
using LauraChatManager.Configuration;

namespace LauraChatManager.Methods;

public class NsfwDetecter {
    public static readonly string header = "multipart/form-data";

    public static async Task<NsfwObject> GetNsfwScan(string path) {
        try {
            HttpClientHandler handler = new();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            using (HttpClient httpClient = new HttpClient(handler))
            {
                using (MultipartFormDataContent formData = new MultipartFormDataContent())
                {
                    FileStream imageStream = File.OpenRead(path);
                    formData.Add(new StreamContent(imageStream), "image", "image.jpg");

                    HttpResponseMessage response = await httpClient.PostAsync(Config.LocalNswfDetector + "/classify", formData);
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
