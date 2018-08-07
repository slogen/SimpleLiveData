using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Scm.Sys.Tests
{
    public class DynamicArrayTests
    {
        [Fact]
        public void DynamicArray_AsArray_ThrowsOnNullElementType()
        {
            new int[0].Invoking(x => x.AsArray(null))
                .Should().Throw<ArgumentNullException>().Which.Message.Should().Contain("elementType");
        }
        [Fact]
        public void DynamicArray_AsArray_ThrowsOnNullEnumerable()
        {
            default(IEnumerable).Invoking(x => x.AsArray(typeof(int)))
                .Should().Throw<ArgumentNullException>().Which.Message.Should().Contain("enumerable");
        }
        [Fact]
        public void DynamicArray_AsArray_ThrowsOnNullCollection()
        {
            default(int[]).Invoking(x => x.AsArray(typeof(int)))
                .Should().Throw<ArgumentNullException>().Which.Message.Should().Contain("collection");
        }

        [Fact]
        public void DynamicArray_AsArray_OfExistingTypesReturnsSameObject()
        {
            var l = Enumerable.Range(0, 2).ToArray();
            l.AsArray(typeof(int)).Should().BeSameAs(l);
        }
        [Fact]
        public void DynamicArray_AsArray_OfOtherTypesReturnsNewObject()
        {
            var l = Enumerable.Range(0, 2).ToList();
            var r = l.AsArray(typeof(long));
            r.Should().BeEquivalentTo(new []{ 0L, 1L });
            r.Should().NotBeSameAs(l);
        }
        [Fact]
        public void DynamicArray_AsArray_OfCollectionShouldUseCopyTo()
        {
            var l = A.Fake<ICollection>();
            A.CallTo(() => l.Count).Returns(2);
            A.CallTo(() => l.CopyTo(A<Array>.Ignored, 0)).DoesNothing();
            l.AsArray(typeof(int)).Should().NotBeSameAs(l);
            A.CallTo(() => l.Count).MustHaveHappenedOnceExactly();
            A.CallTo(() => l.CopyTo(A<Array>.Ignored, 0)).MustHaveHappenedOnceExactly();
        }
        [Fact]
        public void DynamicArray_AsArray_OfCollectionConverts()
        {
            var l = new HashSet<int>(Enumerable.Range(0, 11));
            var r = l.AsArray(typeof(int));
            r.Should().BeEquivalentTo(l);
        }
    }
}
