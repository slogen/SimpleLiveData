using System;
using System.Threading.Tasks;
using DataSys.Protocol;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Scm.Sys;
using Scm.Web;
using Xunit;

namespace SimpleLiveData.Tests.Test
{
    public class PlainTests : TestSourceBasedTests
    {
        [Fact]
        public async Task GetById()
        {
            TestSource.Prepare(3, 3);
            var i1 = await TestSource.Installations.Query(ins => ins.FirstOrDefaultAsync());
            var task = Client.GetJsonAsync(
                new Uri(Server.BaseAddress, $"/api/Installation/{i1.Id}"),
                JsonSerializer
            );
            var result = await task.ConfigureAwait(false);
            var got = await result.Convert<Installation>(CancellationToken).ConfigureAwait(false);
            got.Should().BeEquivalentTo(new {i1.Id, i1.Name, i1.InstallationPeriod.From, i1.InstallationPeriod.To});
        }
    }
}