using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Scm.Linq
{
    public static class ExpressionCompositionExtensions
    {
        /// <summary>
        /// Compose <paramref name="f"/> *after* <paramref name="g"/> in the mathematical sense: (f.After(g))(x) == f(g(x))
        /// </summary>
        public static Expression<Func<TIn1, TOut2>> After<TIn1, TOut1, TIn2, TOut2>(
            this Expression<Func<TIn2, TOut2>> f, Expression<Func<TIn1, TOut1>> g)
            where TOut1 : TIn2
            => Expression.Lambda<Func<TIn1, TOut2>>(f.Body.Replace(f.Parameters.Single(), g.Body),
                g.Parameters.Single());

        /// <summary>
        /// Compose <paramref name="f"/> *before* <paramref name="g"/> in the mathematical sense: (f.Before(g))(x) == g(f(x))
        /// </summary>
        public static Expression<Func<TIn1, TOut2>> Before<TIn1, TOut1, TIn2, TOut2>(
            this Expression<Func<TIn1, TOut1>> f, Expression<Func<TIn2, TOut2>> g)
            where TOut1 : TIn2
            => g.After(f);

        private static Expression<Func<TIn, bool>> BinaryDistributiveOperation<TIn>(
            Func<Expression, Expression, Expression> binaryOp,
            Expression<Func<TIn, bool>> f1,
            Expression<Func<TIn, bool>> f2)
            => Expression.Lambda<Func<TIn, bool>>(
                binaryOp(f1.Body, f2.Body.Replace(f2.Parameters.Single(), f1.Parameters.Single())),
                f1.Parameters.Single());

        private static Expression<Func<TIn, bool>> UnaryDistributiveOperation<TIn>(
            Func<Expression, Expression> unaryOp,
            Expression<Func<TIn, bool>> f)
            => Expression.Lambda<Func<TIn, bool>>(unaryOp(f.Body), f.Parameters);

        public static Expression<Func<TIn, bool>> AndAlso<TIn>(
            this Expression<Func<TIn, bool>> f1,
            Expression<Func<TIn, bool>> f2)
            => BinaryDistributiveOperation(Expression.AndAlso, f1, f2);

        public static Expression<Func<TIn, bool>> OrElse<TIn>(this Expression<Func<TIn, bool>> f1,
            Expression<Func<TIn, bool>> f2)
            => BinaryDistributiveOperation(Expression.OrElse, f1, f2);

        public static Expression<Func<TIn, bool>> Not<TIn>(this Expression<Func<TIn, bool>> f)
            => UnaryDistributiveOperation(Expression.Not, f);

        public static Expression<Func<TArg1, Func<TArg2, T>>> Curry<TArg1, TArg2, T>(
            this Expression<Func<TArg1, TArg2, T>> f)
            => Expression.Lambda<Func<TArg1, Func<TArg2, T>>>(
                Expression.Lambda<Func<TArg2, T>>(f.Body, f.Parameters.Skip(1).Single()),
                f.Parameters.First());

        public static Expression<Func<TIn, bool>> All<TIn>(this IEnumerable<Expression<Func<TIn, bool>>> source,
            bool ifEmpty = true)
            => source.Aggregate(default(Expression<Func<TIn, bool>>), (acc, next) => acc?.AndAlso(next) ?? next) ??
               (_ => ifEmpty);

        public static Expression<Func<TIn, bool>> Any<TIn>(this IEnumerable<Expression<Func<TIn, bool>>> source,
            bool ifEmpty = false)
            => source.Aggregate(default(Expression<Func<TIn, bool>>), (acc, next) => acc?.OrElse(next) ?? next) ??
               (_ => ifEmpty);
    }
}