using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using DataSys.App.Tests.Support.App;
using FluentAssertions;
using Scm.Web;
using Xunit;

namespace DataSys.App.Tests.Test
{
    public class ODataTests : TestSourceBasedTests, IClassFixture<TestAppUnitOfWorkFactory>
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter", Justification = "Specific type required for IoC")]
        public ODataTests(TestAppUnitOfWorkFactory appUnitOfWorkFactory) : base(appUnitOfWorkFactory)
        {
        }

        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Deserialized")]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Protocol for deserialize")]
        private class Named
        {
            public string Name { get; set; }
        }

        [Fact]
        public async Task ODataNameOfInstallation()
        {
            await TestSource.Prepare(3, 3, CancellationToken).ConfigureAwait(false);
            var server = Server;
            var lst = await Client.GetJsonAsync<List<Named>>(
                // "/odata/Installation?$expand=Data&$filter=contains(name, '1')"
                // $"/odata/Installation({_app.TestSource.Installations.Query(x => x.First()).Id})" // -- BROKEN!
                new Uri(server.BaseAddress, "/odata/Installation?$select=Name"),
                JsonSerializer,
                CancellationToken
            );
            var expect = TestSource.Installations().Select(i => new {i.Name});
            lst.Should().BeEquivalentTo(expect);
        }
    }
}