using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DataSys.App.Presentation.Mvc;
using DataSys.Protocol;
using FluentAssertions;
using Parquet;
using Scm.Presentation.Mvc.Parquet;
using Scm.Sys;
using Scm.Web;
using Xunit;

namespace DataSys.App.Tests.Test
{
    [Collection(nameof(StandardClientServer33ReadOnly))]
    public class ParquetIntegrationTests : InClientServerStateContextTest<StandardClientServer33ReadOnly>
    {
        protected async Task<IList<T>> SendAsyncParquet<T>(HttpRequestMessage req)
            where T: new()
        {
            var reply = await (await Client.ConfigureAwait(false))
                .SendAsync(req.ClearAccept().Accept(DefaultParquetOutputFormatter.SupportedMediaTypeList.First()),
                    CancellationToken).ConfigureAwait(false);
            reply.EnsureSuccessStatusCode();
            return await reply.Content.ReadAsStreamAsync().AsSeekableAsync(CancellationToken)
                .ParquetDeserialize<T>(CancellationToken)
                .ConfigureAwait(false);
        }

        protected class SimpleStructure
        {
            public int Id { get; set; }

            public int? NullableId { get; set; }

            public string Name { get; set; }

            public DateTimeOffset Date { get; set; }
        }

        [Fact]
        public void SimpleParquetSerializationWorks()
        {
            var t = DateTimeOffset.UtcNow.TruncateTo(TimeSpan.FromSeconds(1));
            var src = Enumerable.Range(0, 2).Select(x =>
                    new Installation(Guid.NewGuid(), $"I{x}", 
                        t,
                        t.Add(TimeSpan.FromHours(1))))
                .ToArray();
            using (var ms = new MemoryStream())
            {
                ParquetConvert.Serialize(src, ms);
                ms.Seek(0, SeekOrigin.Begin);
                var got = ParquetConvert.Deserialize<Installation>(ms);
                got.Should().BeEquivalentTo(src, cfg => cfg.WithStrictOrdering()
                    // Currently Guid is not serialized :(
                    .Excluding(x => x.Id));
            }
        }

        [Fact]
        public async Task GetTurbinesByIdReturnsExpectedParquet()
        {
            await Prepared.ConfigureAwait(false);
            var server = Server;
            var got = await SendAsyncParquet<Installation>(
                    new Uri(server.BaseAddress,
                            $"{InstallationXController.RoutePrefix}/{InstallationXController.AllRoute}")
                        .GetRequest())
                .ConfigureAwait(false);
            got.Should().BeEquivalentTo(TestSource.Installations,
                cfg => cfg.ExcludingMissingMembers()
                    // Currently Guid is not serialized :(
                    .Excluding(x => x.Id));
        }

        public ParquetIntegrationTests(StandardClientServer33ReadOnly context) : base(context)
        {
        }
    }
}