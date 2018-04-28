using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
            Uuid.Dns.Namespace("www.example.org", version: 3).Should()
                .Be(Guid.Parse("74738ff5-5367-5958-9aee-98fffdcd1876"));
        }

        [Fact]
        public void InvalidVersionThrows()
        {
            "foo".Invoking(name => Uuid.Dns.Namespace(name, version: 4))
                .Should().Throw<NotSupportedException>().Where(ex => ex.Message.Contains("Version"));
        }

        [Fact]
        public void NullBytesThrows()
        {
            default(byte[]).Invoking(bytes => Uuid.Dns.Namespace(bytes, 0, 0, 3, MD5.Create()))
                .Should().Throw<ArgumentException>().Where(ex => ex.Message.Contains("bytes"));
        }

        [Fact]
        public void ShortBytesThrows()
        {
            new byte[16].Invoking(bytes => Uuid.Dns.Namespace(bytes, 1, 16, 3, MD5.Create()))
                .Should().Throw<ArgumentException>().Where(ex => ex.Message.Contains("bytes"));
        }

        [Fact]
        public void UuidDefaultEncodingIsUsed()
        {
            const string nonAscii = "é";
            if (Uuid.DefaultEncoding.EncodingName != Encoding.UTF8.EncodingName)
                throw new InvalidOperationException("Test case only supports the UTF8 as default");
            Uuid.DefaultEncoding = Encoding.UTF8;
            Uuid.Dns.Namespace(nonAscii, encoding: Encoding.UTF32)
                .Should().NotBe(Uuid.Dns.Namespace(nonAscii));
        }

        [Fact]
        public void UuidDefaultProduceVariousItems()
        {
            Enumerable.Range(0, 1000)
                .Select(_ => Uuid.Default())
                .Should().OnlyHaveUniqueItems()
                .And.NotContain(Guid.Empty);
        }
    }
}