using FluentAssertions;
using Xunit;

namespace Scm.Linq.Tests
{
    public class FTests
    {
        [Fact]
        void FExprRunsCorrectly()
        {
            F.Expr(() => 1).Compile()().Should().Be(1);
            F.Expr((int x) => -x).Compile()(2).Should().Be(-2);
            F.Expr((int x, long y) => x + y).Compile()(int.MaxValue, 1000).Should().Be(int.MaxValue + 1000L);
            F.Expr((int x, long y, double z) => x + y + z).Compile()(int.MaxValue, 1000, 2.25).Should()
                .Be(int.MaxValue + 1000L + 2.25);
        }

        [Fact]
        void FFuncRunsCorrectly()
        {
            F.Func(() => 1)().Should().Be(1);
            F.Func((int x) => -x)(2).Should().Be(-2);
            F.Func((int x, long y) => x + y)(int.MaxValue, 1000).Should().Be(int.MaxValue + 1000L);
            F.Func((int x, long y, double z) => x + y + z)(int.MaxValue, 1000, 2.25).Should()
                .Be(int.MaxValue + 1000L + 2.25);
        }
    }
}