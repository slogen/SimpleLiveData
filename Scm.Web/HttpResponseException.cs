using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Scm.Web
{
    public class HttpResponseException : HttpRequestException
    {
        public HttpResponseException(HttpResponseMessage response, string message, Exception inner) : base(message,
            inner)
        {
            Response = response;
        }

        public HttpResponseException(HttpResponseMessage response, Exception inner)
            : this(response, Format(response), inner)
        {
        }

        public HttpResponseException(HttpResponseMessage response)
            : this(response, null)
        {
        }

        public HttpResponseMessage Response { get; }

        protected static string Format(HttpResponseMessage response, string content = null)
            => $"HTTP Error: {response.StatusCode}\n{response.Headers.HeadersString()}\n{content ?? ""}";

        public static async Task AssertSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new HttpResponseException(response, Format(response, content), null);
            }
        }
    }
}