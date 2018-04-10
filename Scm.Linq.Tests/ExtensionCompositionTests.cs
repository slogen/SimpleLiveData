using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace Scm.Linq.Tests
{
    public class ExtensionCompositionTests
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Just throw")]
        private static bool DoThrow(int x) => throw new InvalidOperationException();

        [Fact]
        public void CompositionAndCorrectlyEvaluated()
        {
            const int i = 2 * 3;
            F.Expr((int x) => x % 2 == 0)
                .AndAlso(x => x % 3 == 0)
                .Compile()(i).Should().Be(true);
            F.Expr((int x) => x % 4 == 0)
                .AndAlso(x => x % 3 == 0)
                .Compile()(i).Should().Be(false);
            F.Expr((int x) => x % 3 == 0)
                .AndAlso(x => x % 4 == 0)
                .Compile()(i).Should().Be(false);
        }

        [Fact]
        public void CompositionAndDoesShortcutEvaluation()
        {
            F.Expr((int x) => x % 2 == 0)
                .AndAlso(x => DoThrow(x))
                .Compile()(1).Should().Be(false);
        }


        [Fact]
        public void CompositionOrCorrectlyEvaluated()
        {
            const int i = 2 * 3;
            F.Expr((int x) => x % 2 == 0)
                .OrElse(x => x % 3 == 0)
                .Compile()(i).Should().Be(true);
            F.Expr((int x) => x % 4 == 0)
                .OrElse(x => x % 3 == 0)
                .Compile()(i).Should().Be(true);
            F.Expr((int x) => x % 3 == 0)
                .OrElse(x => x % 4 == 0)
                .Compile()(i).Should().Be(true);
            F.Expr((int x) => x % 5 == 0)
                .OrElse(x => x % 4 == 0)
                .Compile()(i).Should().Be(false);
        }

        [Fact]
        public void CompositionOrDoesShortcutEvaluation()
        {
            F.Expr((int x) => x % 2 == 0)
                .OrElse(x => DoThrow(x))
                .Compile()(0).Should().Be(true);
        }

        [Fact]
        [SuppressMessage("ReSharper", "ArgumentsStyleLiteral", Justification = "Used to state explicit test")]
        [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue", Justification = "Used to state explicit test")]
        public void EmptyCompositionShouldEvaluateCorrectly()
        {
            var none = Enumerable.Empty<Expression<Func<int, bool>>>().ToList();
            none.All(ifEmpty: true).Compile()(1).Should().Be(true);
            none.All(ifEmpty: false).Compile()(1).Should().Be(false);
            none.All().Compile()(1).Should().Be(true);
            none.Any(ifEmpty: true).Compile()(1).Should().Be(true);
            none.Any(ifEmpty: false).Compile()(1).Should().Be(false);
            none.Any().Compile()(1).Should().Be(false);
        }

        [Fact]
        public void FAfterGAppliesGFirst()
        {
            F.Expr((int i) => $"f({i})")
                .After((int i) => i + 1)
                .Compile()(0)
                .Should()
                .Be("f(1)");
        }

        [Fact]
        public void SequenceCompositionAllDoesShortut()
        {
            new[] {F.Expr((int x) => x % 2 == 0), x => DoThrow(x)}
                .All().Compile()(1).Should().BeFalse();
        }

        [Fact]
        public void SequenceCompositionAnyDoesShortut()
        {
            new[] {F.Expr((int x) => x % 2 == 0), x => DoThrow(x)}
                .Any().Compile()(0).Should().BeTrue();
        }

        [Fact]
        [SuppressMessage("ReSharper", "ArgumentsStyleLiteral", Justification = "Used to state explicit test")]
        [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue", Justification = "Used to state explicit test")]
        public void SequenceCompositionShouldEvaluateCorrectly()
        {
            var predicates = new[]
            {
                new[] {F.Expr((int x) => x % 2 == 0), F.Expr((int x) => x % 3 == 0), F.Expr((int x) => x % 5 == 0)},
                new[] {F.Expr((int x) => x % 2 == 0), F.Expr((int x) => x % 3 == 0), F.Expr((int x) => x % 4 == 0)},
                new[] {F.Expr((int x) => x % 4 == 0), F.Expr((int x) => x % 8 == 0), F.Expr((int x) => x % 9 == 0)}
            };
            const int v = 2 * 3 * 5;
            var expectAll = new[] {true, false, false};
            var expectAny = new[] {true, true, false};
            foreach (var ifEmpty in new[] {true, false})
                for (var i = 2; i < predicates.Length; ++i)
                {
                    predicates[i].All(ifEmpty).Compile()(v)
                        .Should().Be(expectAll[i], "All([{0}] {1}, ifEmpty: {2}) should be {3}", i, predicates[i],
                            ifEmpty, expectAll[i]);
                    var pi = predicates[i].Any(ifEmpty);
                    pi.Compile()(v)
                        .Should().Be(expectAny[i], "Any([{0}] {1}, ifEmpty: {2}) should be {3}", i, predicates[i],
                            ifEmpty, expectAny[i]);
                }
        }
    }
}