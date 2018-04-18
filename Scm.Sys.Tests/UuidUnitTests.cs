using System;
using FluentAssertions;
using Xunit;

namespace Scm.Sys.Tests
{
    public class UuidUnitTests
    {
        [Fact]
        public void DnsNamespaceShouldBeCorrect()
        {
            Uuid.Dns.Namespace("www.example.org").Should().Be(Guid.Parse("74738ff5-5367-5958-9aee-98fffdcd1876"));
        }
    }
}
