﻿using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scm.Sys;

namespace Scm.Web
{
    public static class HttpExtensions
    {
        public static HttpCompletionOption DefaultGetJsonAsyncCompletionOptions =
            HttpCompletionOption.ResponseHeadersRead;

        public static Encoding Encoding(this HttpResponseMessage response)
        {
            // TODO: Guess from response
            return System.Text.Encoding.UTF8;
        }

        public static HttpRequestMessage ClearAccept(this HttpRequestMessage req)
        {
            req.Headers.Accept.Clear();
            return req;
        }
        public static HttpRequestMessage Accept(this HttpRequestMessage req, MediaTypeWithQualityHeaderValue mediaType)
        {
            req.Headers.Accept.Add(mediaType);
            return req;
        }

        public static HttpRequestMessage Accept(this HttpRequestMessage req, string mediaType) =>
            req.Accept(new MediaTypeWithQualityHeaderValue(mediaType));

        public static HttpRequestMessage AcceptJson(this HttpRequestMessage req) => req.Accept("application/json");

        public static HttpRequestMessage ClearAcceptCharset(this HttpRequestMessage req)
        {
            req.Headers.AcceptCharset.Clear();
            return req;
        }
        public static HttpRequestMessage AcceptCharset(this HttpRequestMessage req, StringWithQualityHeaderValue charset)
        {
            req.Headers.AcceptCharset.Add(charset);
            return req;
        }

        public static HttpRequestMessage AcceptCharset(this HttpRequestMessage req, string charset) =>
            req.AcceptCharset(new StringWithQualityHeaderValue(charset));

        public static HttpRequestMessage AcceptCharset(this HttpRequestMessage req, Encoding charset) =>
            req.AcceptCharset(charset.WebName);

        public static HttpRequestMessage AcceptUtf8(this HttpRequestMessage req) => req.AcceptCharset(System.Text.Encoding.UTF8);

        public static async Task<TextReader> BodyText(this HttpResponseMessage resp,
            bool? detectEncodingFromByteOrderMarks = null,
            int? bufferSize = null,
            bool? leaveOpen = null)
        {
            var bodyStream = await resp.Content.ReadAsStreamAsync().ConfigureAwait(false);
            //var headers = resp.Headers;
            // TODO: Read encoding from headers
            return new StreamReader(bodyStream, resp.Encoding(),
                detectEncodingFromByteOrderMarks: false,
                bufferSize: bufferSize ?? 16 * 1024,
                leaveOpen: leaveOpen ?? true);
        }

        public static HttpRequestMessage GetRequest(this Uri uri) => new HttpRequestMessage(HttpMethod.Get, uri);

        public static string HeadersString(this HttpHeaders headers)
        {
            return string.Join("\n",
                headers.Select(h => $@"{h.Key}: {string.Join("; ", h.Value)}"));
        }

        public static async Task AssertSuccess(this HttpResponseMessage resp)
            => await HttpResponseException.AssertSuccess(resp).ConfigureAwait(false);

        public static async Task<HttpResponseMessage> AssertSuccess(this Task<HttpResponseMessage> respTask)
        {
            var resp = await respTask.ConfigureAwait(false);
            await AssertSuccess(resp).ConfigureAwait(false);
            return resp;
        }

        public static async Task<IAsyncConvertible> GetJsonAsync(this HttpClient client, Uri uri,
            JsonSerializer jsonSerializer,
            CancellationToken cancellationToken,
            HttpCompletionOption? httpCompletionOption = null)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (jsonSerializer == null)
                throw new ArgumentNullException(nameof(jsonSerializer));
            var resp = await client.SendAsync(
                    uri.GetRequest().AcceptJson().AcceptUtf8(),
                    httpCompletionOption ?? DefaultGetJsonAsyncCompletionOptions,
                    cancellationToken)
                .AssertSuccess()
                .ConfigureAwait(false);
            return await JsonHttpStreamConversion.FromResponse(jsonSerializer, resp).ConfigureAwait(false);
        }

        public static async Task<T> GetJsonAsync<T>(this HttpClient client, Uri uri,
            JsonSerializer jsonSerializer,
            CancellationToken cancellationToken,
            HttpCompletionOption? httpCompletionOption = null)
            => await (await GetJsonAsync(client, uri, jsonSerializer, cancellationToken, httpCompletionOption)
                .ConfigureAwait(false)).Convert<T>(cancellationToken).ConfigureAwait(false);

        public static async Task<T> GetJsonAsync<T>(this Task<HttpClient> clientTask, Uri uri,
            JsonSerializer jsonSerializer,
            CancellationToken cancellationToken,
            HttpCompletionOption? httpCompletionOption = null)
        {
            var client = await clientTask.ConfigureAwait(false);
            return await client.GetJsonAsync<T>(uri, jsonSerializer, cancellationToken, httpCompletionOption)
                .ConfigureAwait(false);
        }
    }
}