using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Scm.Web
{
    public class JsonHttpStreamConversion : JsonStreamConversion
    {
        public JsonHttpStreamConversion(JsonSerializer deserializer, Stream stream, Encoding encoding) : base(
            deserializer, stream, encoding)
        {
        }

        public static async Task<JsonHttpStreamConversion> FromResponse(JsonSerializer deserializer,
            HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"HTTP Error: {response.StatusCode}\n{await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
            var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            return new JsonHttpStreamConversion(deserializer, stream, response.Encoding());
        }
    }
}