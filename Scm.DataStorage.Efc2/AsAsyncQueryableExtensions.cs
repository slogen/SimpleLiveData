using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Scm.DataStorage.Efc2
{
    public static class AsAsyncQueryableExtensions
    {
        public static IQueryable<TElement> AsAsyncQueryable<TElement>(this IEnumerable<TElement> source,
            TaskFactory taskFactory = null)
        {
            var q = source.AsQueryable();
            return q.Provider is IAsyncQueryProvider ? q : new AsyncQueryable<TElement>(q, taskFactory);
        }

        private sealed class AsyncQueryProvider<T> : IAsyncQueryProvider
        {
            internal AsyncQueryProvider(IQueryProvider inner, TaskFactory taskFactory = null)
            {
                Inner = inner;
                TaskFactory = taskFactory ?? Task.Factory;
            }

            private TaskFactory TaskFactory { get; }
            private IQueryProvider Inner { get; }

            public IQueryable CreateQuery(Expression expression) => new AsyncQueryable<T>(expression, TaskFactory);

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression) =>
                new AsyncQueryable<TElement>(expression, TaskFactory);

            public object Execute(Expression expression) => Inner.Execute(expression);

            public TResult Execute<TResult>(Expression expression) => Inner.Execute<TResult>(expression);

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
                => new AsyncQueryable<TResult>(expression, TaskFactory);

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
                => TaskFactory.StartNew(() => Execute<TResult>(expression), cancellationToken);
        }

        private sealed class AsyncQueryable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public AsyncQueryable(IEnumerable<T> enumerable, TaskFactory taskFactory)
                : base(enumerable)
            {
                TaskFactory = taskFactory;
            }

            public AsyncQueryable(Expression expression, TaskFactory taskFactory)
                : base(expression)
            {
                TaskFactory = taskFactory;
            }

            private TaskFactory TaskFactory { get; }

            public IAsyncEnumerator<T> GetEnumerator() =>
                new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator(), TaskFactory);

            IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);
        }

        private struct AsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private IEnumerator<T> Inner { get; }
            private TaskFactory TaskFactory { get; }

            public AsyncEnumerator(IEnumerator<T> inner, TaskFactory taskFactory)
            {
                Inner = inner;
                TaskFactory = taskFactory;
            }

            public void Dispose()
            {
                Inner.Dispose();
            }

            public Task<bool> MoveNext(CancellationToken cancellationToken)
                => TaskFactory.StartNew(Inner.MoveNext, cancellationToken);

            public T Current => Inner.Current;
        }
    }
}