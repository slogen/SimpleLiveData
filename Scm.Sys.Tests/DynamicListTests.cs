using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Scm.Sys.Tests
{
    public class DynamicListTests
    {
        public static IEnumerable<object[]> InitialCounts =>
            new[] { default(int?), 0, 1, 2 }.Select(x => new object[] { x });

        [Theory]
        [MemberData(nameof(InitialCounts))]
        public void DynamicList_Create_ThrowsOnNullElementType(int? initialCapacity)
        {
            default(Type).Invoking(x => DynamicList.Create(x, initialCapacity))
                .Should().Throw<ArgumentNullException>().Which.Message.Should().Contain("elementType");
        }
        [Theory]
        [MemberData(nameof(InitialCounts))]
        public void DynamicList_AsList_ThrowsOnNullEnumerable(int? initialCapacity)
        {
            default(int[]).Invoking(x => x.AsList(typeof(int), initialCapacity))
                .Should().Throw<ArgumentNullException>().Which.Message.Should().Contain("enumerable");
        }
        [Theory]
        [MemberData(nameof(InitialCounts))]
        public void DynamicList_AsList_ThrowsOnNullElementType(int? initialCapacity)
        {
            new int[0].Invoking(x => x.AsList(null, initialCapacity))
                .Should().Throw<ArgumentNullException>().Which.Message.Should().Contain("elementType");
        }

        [Theory]
        [MemberData(nameof(InitialCounts))]
        public void DynamicList_AsList_OfExistingTypesReturnsSameObject(int? initialCapacity)
        {
            var l = Enumerable.Range(0, 2).ToList();
            l.AsList(typeof(int), initialCapacity).Should().BeSameAs(l);
        }
        [Theory]
        [MemberData(nameof(InitialCounts))]
        public void DynamicList_AsList_OfOtherTypesReturnsNewObject(int? initialCapacity)
        {
            var l = Enumerable.Range(0, 2).ToList();
            var r = l.AsList(typeof(long), initialCapacity);
            r.Should().BeEquivalentTo(new []{ 0L, 1L });
            r.Should().NotBeSameAs(l);
        }
        [Theory]
        [MemberData(nameof(InitialCounts))]
        public void DynamicList_AsList_OfCollectionShouldUseCopyTo(int? initialCapacity)
        {
            var l = A.Fake<ICollection>();
            A.CallTo(() => l.Count).Returns(2);
            A.CallTo(() => l.CopyTo(A<Array>.Ignored, 0)).DoesNothing();
            l.AsList(typeof(int), initialCapacity).Should().NotBeSameAs(l);
            A.CallTo(() => l.Count).MustHaveHappenedOnceExactly();
            A.CallTo(() => l.CopyTo(A<Array>.Ignored, 0)).MustHaveHappenedOnceExactly();
        }
        [Theory]
        [MemberData(nameof(InitialCounts))]
        public void DynamicList_AsList_OfCollectionConverts(int? initialCapacity)
        {
            var l = new HashSet<int>(Enumerable.Range(0, 11));
            var r = l.AsList(typeof(int), initialCapacity);
            r.Should().BeEquivalentTo(l);
        }
    }
}
