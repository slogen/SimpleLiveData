using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleLiveData.Tests
{
    public static class TestExtensions
    {
        public static async Task<string> TestGetAsync(this HttpClient client, string uri)
        {
            var resp = await client.GetAsync(uri).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"HTTP Error: {resp.StatusCode}\n{await ExtractExplanation(resp)}");
            }

            return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }

        private static async Task<string> ExtractExplanation(HttpResponseMessage resp)
        {
            return await resp.Content.ReadAsStringAsync();
        }
    }
}