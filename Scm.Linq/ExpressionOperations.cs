using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Scm.Linq
{
    public static class ExpressionOperations
    {
        /// <summary>
        /// Replace all occurences of <paramref name="from"/> with <paramref name="to"/> in <paramref name="body"/>
        /// </summary>
        public static Expression Replace(this Expression body, Expression from, Expression to)
            => new ExpressionReplacer(from, to).Visit(body);

        /// <summary>
        /// Replace all occurences of <paramref name="fromFunc"/>(<paramref name="source"/>[i]) with <paramref name="toFunc"/>(<paramref name="source"/>[i]) in <paramref name="body"/>
        /// </summary>
        public static Expression Replace<TSource>(this Expression body, IEnumerable<TSource> source,
            Func<TSource, Expression> fromFunc, Func<TSource, Expression> toFunc)
            => source.Aggregate(
                body,
                (acc, next) => acc.Replace(fromFunc(next), toFunc(next)));

        public static Expression BetaReduce(this LambdaExpression lambda, params Expression[] arguments)
            => lambda.BetaReduce((IEnumerable<Expression>) arguments);

        public static Expression BetaReduce(this LambdaExpression lambda, IEnumerable<Expression> arguments)
            => lambda.Body.Replace(
                lambda.Parameters.Zip(arguments, (p, a) => new {p, a}),
                x => x.p,
                x => x.a);

        public static Expression BetaReduce<TIn, TOut>(this Expression<Func<TIn, TOut>> f, Expression arg)
        {
            if (!typeof(TIn).IsAssignableFrom(arg.Type))
                throw new InvalidCastException($"Unable to assign {typeof(TIn)} from {arg.Type} ");
            return ((LambdaExpression) f).BetaReduce(arg);
        }

        public static Expression BetaReduceRecursive(this Expression expresion)
            => new BetaReduceRecursiveVisitor().Visit(expresion);

        private sealed class ExpressionReplacer : ExpressionVisitor
        {
            private readonly Expression _from;
            private readonly Expression _to;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public ExpressionReplacer(Expression from, Expression to)
            {
                _from = from;
                _to = to;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Expression Visit(Expression node)
            {
                return ReferenceEquals(node, _from) ? _to : base.Visit(node);
            }
        }

        private class BetaReduceRecursiveVisitor : ExpressionVisitor
        {
            protected override Expression VisitInvocation(InvocationExpression node)
                => ((LambdaExpression) node.Expression).BetaReduce(
                        node.Arguments.Select(BetaReduceRecursive))
                    .BetaReduceRecursive();
        }
    }
}