using System;
using System.Linq.Expressions;
using FluentAssertions;
using Xunit;

namespace Scm.Linq.Tests
{
    public class ExpressionOperationsTests
    {
        [Fact]
        public void BetaReduceRecusiveTransforms()
        {
            var f = F.Expr<int, Func<int, int, int>>(x => (y, z) => x + x + y + z);
            var e = Expression.Invoke(f,
                Expression.Add(Expression.Constant(1, typeof(int)), Expression.Constant(2, typeof(int))));
            var r = e.BetaReduceRecursive();
            r.Should().BeEquivalentTo(
                F.Expr((int y, int z) => 1 + 2 + 1 + 2 + y + z));
        }
    }
}