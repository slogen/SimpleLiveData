using FakeItEasy;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace Scm.Sys.Tests
{
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "These tests execise exactly that")]
    public class DynamicEnumerableTests
    {
        [Fact]
        public void DynamicEnumerable_Elementtype_ThrowsOnNullElementType()
        {
            default(IEnumerable).Invoking(x => x.ElementType())
                .Should().Throw<ArgumentNullException>().Which.Message.Should().Contain("enumerable");
        }
        [Fact]
        public void DynamicEnumerable_Elementtype_ThrowsOnMissingIEnumerableTImplementation()
        {
            var e = A.Fake<IEnumerable>();
            e.Invoking(x => x.ElementType())
                .Should().Throw<DynamicEnumerable.InconclusiveElementTypeException>()
                .Which.Should().BeEquivalentTo(new
                {
                    SourceObject = e,
                    CandidateTypes = new Type[0]
                });
        }


        [SuppressMessage("ReSharper", "PossibleInterfaceMemberAmbiguity", Justification = "Tests for exactly that")]
        public interface IMultipleIEnumerableImplementations: IEnumerable<int>, IEnumerable<float> { }
        [Fact]
        public void DynamicEnumerable_Elementtype_ThrowsOnMultipleIEnumerableTImplementation()
        {
            var e = A.Fake<IMultipleIEnumerableImplementations>();
            e.Invoking(x => x.ElementType())
                .Should().Throw<DynamicEnumerable.InconclusiveElementTypeException>()
                .Which.Should().BeEquivalentTo(new
                {
                    SourceObject = e,
                    CandidateTypes = typeof(IMultipleIEnumerableImplementations).GetInterfaces().Where(x => x != typeof(IEnumerable))
                });
        }
        [Fact]
        public void DynamicEnumerable_Elementtype_ExtractsElementType()
        {
            A.Fake<IEnumerable<float>>().ElementType().Should().Be(typeof(float));
        }

    }
}
