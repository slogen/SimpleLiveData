using System;
using System.Net.Http;
using System.Threading.Tasks;
using DataSys.App.Tests.Support.App.Source;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace DataSys.App.Tests.Test
{
    /// <inheritdoc />
    /// <summary>
    /// Captures a context with a (lazy) running server, client and TestSource
    /// </summary>
    public interface IClientServerStateContext: IDisposable
    {
        Task<HttpClient> Client { get; }
        TestServer Server { get; }
        JsonSerializer JsonSerializer { get; }
        TestSource TestSource { get; }
        Task Prepared { get; }
    }
}