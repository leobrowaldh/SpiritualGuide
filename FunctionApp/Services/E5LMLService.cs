using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;

namespace FunctionApp.Services
{
    public class E5LMLService : IE5LMLService
    {
        private readonly HttpClient _httpClient;

        public E5LMLService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<float[]> EmbedAsync(string input)
        {
            var response = await _httpClient.PostAsJsonAsync("/embed", new { input });
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var emb = doc.RootElement.GetProperty("embedding");
            var arr = new float[emb.GetArrayLength()];
            int i = 0;
            foreach (var v in emb.EnumerateArray())
                arr[i++] = v.GetSingle();
            return arr;
        }
    }
}
