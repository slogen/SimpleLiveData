using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DataSys.App.Presentation.Mvc;
using DataSys.App.Tests.Support.App;
using DataSys.Protocol;
using FluentAssertions;
using Parquet;
using Scm.Web;
using Scm.Presentation.Mvc.Parquet;
using Scm.Sys;
using Xunit;

namespace DataSys.App.Tests.Test
{
    public class PlainTests : TestSourceBasedTests, IClassFixture<TestAppUnitOfWorkFactory>
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Specific type required for IoC")]
        public PlainTests(TestAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }

        [Fact]
        public async Task GetTurbinesByIdReturnsExpectedJson()
        {
            await TestSource.Prepare(3, 3, CancellationToken).ConfigureAwait(false);
            var server = Server;
            var client = await Client.ConfigureAwait(false);
            var i1 = await TestSource.ObserveInstallations(ins => ins.ToObservable().FirstOrDefaultAsync());
            var got = await client.GetJsonAsync<Installation>(
                new Uri(server.BaseAddress, $"{InstallationXController.RoutePrefix}/{i1.Id}"),
                JsonSerializer,
                CancellationToken
            );
            got.Should().BeEquivalentTo(new {i1.Id, i1.Name, i1.InstallationPeriod.From, i1.InstallationPeriod.To});
        }
    }

    public class ParquetIntegrationTests : TestSourceBasedTests, IClassFixture<TestAppUnitOfWorkFactory>
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Specific type required for IoC")]
        public ParquetIntegrationTests(TestAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }
        public async Task<IList<T>> SendAsyncParquet<T>(HttpRequestMessage req)
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

        public class SimpleStructure
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
            await TestSource.Prepare(3, 3, CancellationToken).ConfigureAwait(false);
            var server = Server;
            var got = await SendAsyncParquet<Installation>(
                new Uri(server.BaseAddress,
                    $"{InstallationXController.RoutePrefix}/{InstallationXController.AllRoute}")
                    .GetRequest())
                .ConfigureAwait(false);
            got.Should().BeEquivalentTo(TestSource.Installations(),
                cfg => cfg.ExcludingMissingMembers()
                    // Currently Guid is not serialized :(
                    .Excluding(x => x.Id));
        }

    }
}