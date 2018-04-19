using System;
using FluentAssertions;
using Xunit;

namespace Scm.Linq.Tests
{
    public class ToConcurrentDictionaryTests
    {
        [Fact]
        public void ToConcurrentDictionaryNotThrowWhenPassingConcurrencyLevel()
        {
            new[] {0}.Invoking(xs => xs.ToConcurrentDictionary(x => x, x => x, concurrencyLevel: 1))
                .Should().NotThrow();
        }

        [Fact]
        public void ToConcurrentDictionaryShouldUseComparer()
        {
            new[] {"a", "A"}.Invoking(items => items.ToConcurrentDictionary(x => x, x => x,
                    StringComparer.InvariantCultureIgnoreCase))
                .Should().Throw<ArgumentException>().Where(ex => ex.Message.Contains("duplicate"));
        }
    }
}