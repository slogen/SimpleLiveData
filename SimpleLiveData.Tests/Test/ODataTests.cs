using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Scm.Sys;
using Scm.Web;
using Xunit;

namespace SimpleLiveData.Tests.Test
{
    public class ODataTests : TestSourceBasedTests
    {
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Deserialized")]
        [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Protocol for deserialize")]
        private class Named
        {
            public string Name { get; set; }
        }

        [Fact]
        public async Task GetApplied()
        {
            TestSource.Prepare(3, 3);
            var task = Client.GetJsonAsync(
                // "/odata/Installation?$expand=Data&$filter=contains(name, '1')"
                // $"/odata/Installation({_app.TestSource.Installations.Query(x => x.First()).Id})" // -- BROKEN!
                new Uri(Server.BaseAddress, "/odata/Installation?$select=Name"),
                JsonSerializer
            );
            var result = await task.ConfigureAwait(false);
            var lst = await result.Convert<List<Named>>(CancellationToken).ConfigureAwait(false);
            lst.Should()
                .BeEquivalentTo(
                    TestSource.Installations.Query(installations => installations.Select(i => new {i.Name})));
        }
    }
}