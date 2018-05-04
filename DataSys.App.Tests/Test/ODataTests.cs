using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DataSys.App.Tests.Support;
using FluentAssertions;
using Scm.Sys;
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
            var task = Client.GetJsonAsync(
                // "/odata/Installation?$expand=Data&$filter=contains(name, '1')"
                // $"/odata/Installation({_app.TestSource.Installations.Query(x => x.First()).Id})" // -- BROKEN!
                new Uri(Server.BaseAddress, "/odata/Installation?$select=Name"),
                JsonSerializer
            );
            var result = await task.ConfigureAwait(false);
            var lst = await result.Convert<List<Named>>(CancellationToken).ConfigureAwait(false);
            var expect = await TestSource.ObserveInstallations(insts => insts.Select(i => new {i.Name})).ToList();
            lst.Should().BeEquivalentTo(expect);
        }
    }
}