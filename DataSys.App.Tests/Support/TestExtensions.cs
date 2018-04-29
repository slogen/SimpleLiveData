using System.Net.Http;
using System.Threading.Tasks;

namespace DataSys.App.Tests.Support
{
    public static class TestExtensions
    {
        public static async Task<string> TestGetAsync(this HttpClient client, string uri)
        {
            var resp = await client.GetAsync(uri).ConfigureAwait(false);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"HTTP Error: {resp.StatusCode}\n{await resp.Content.ReadAsStringAsync()}");

            return await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}