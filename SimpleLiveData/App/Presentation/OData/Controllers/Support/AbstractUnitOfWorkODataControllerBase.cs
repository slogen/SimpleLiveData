﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNetCore.Mvc;
using Scm.DataAccess;
using Scm.DataAccess.Queryable;
using Scm.Presentation.OData;

namespace SimpleLiveData.App.Presentation.OData
{
    public abstract class AbstractUnitOfWorkODataControllerBase<TUnitOfWork, TEntity> 
        : AbstractUnitOfWorkODataControllerBase<TUnitOfWork, IQueryableSource<TEntity>, TEntity>
        where TUnitOfWork: class, IAsyncUnitOfWork
    {
    }

    public abstract class AbstractUnitOfWorkODataControllerBase<TUnitOfWork, TSource, TEntity> : ControllerBase
        where TUnitOfWork: class, IAsyncUnitOfWork
        where TSource: class, IQueryableSource<TEntity>
    {
        private TUnitOfWork _unitOfWork;
        protected TUnitOfWork UnitOfWork => _unitOfWork ?? (_unitOfWork = GetUnitOfWork());
        protected abstract TUnitOfWork GetUnitOfWork();
        private TSource _source;
        protected TSource Source => _source ?? (_source = GetSource());
        protected abstract TSource GetSource();

        protected virtual IODataOptions ODataOptions => null;

        private abstract class AbstractQueryableByQbservable<T>: IQueryable<T>
        {
            private class QP: IQueryProvider {
                public IQueryable CreateQuery(Expression expression)
                {
                    throw new NotImplementedException();
                }

                public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
                {
                    throw new NotImplementedException();
                }

                public object Execute(Expression expression)
                {
                    throw new NotImplementedException();
                }

                public TResult Execute<TResult>(Expression expression)
                {
                    throw new NotImplementedException();
                }
            }
            public abstract IQbservable<T> Source { get; }
            public virtual IQueryProvider Provider { get; } = new QP();
            protected AbstractQueryableByQbservable() { }
            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public virtual IEnumerator GetEnumerator() => throw new NotImplementedException();
            public Type ElementType => Source.ElementType;
            public Expression Expression => Source.Expression;
        }

        private class QueryableByQbservable<T> : AbstractQueryableByQbservable<T>
        {
            public QueryableByQbservable(IQbservable<T> source)
            {
                Source = source;
            }

            public override IQbservable<T> Source { get; }
        }

        private abstract class AbstractQbservableByQueryable : IQbservable
        {
            public abstract IQueryable Source { get; }
            public virtual IQbservableProvider Provider => throw new NotImplementedException();
            protected AbstractQbservableByQueryable() { }
            public virtual IEnumerator GetEnumerator() => throw new NotImplementedException();
            public Type ElementType => Source.ElementType;
            public Expression Expression => Source.Expression;
        }
        private class QbservableByQueryable : AbstractQbservableByQueryable
        {
            public QbservableByQueryable(IQueryable source, IQbservableProvider provider)
            {
                Source = source;
                Provider = provider;
            }

            public override IQueryable Source { get; }
            public override IQbservableProvider Provider { get; }
        }

        public virtual IQueryable Get(ODataQueryOptions<TEntity> queryOptions)
        {
            var ao = ODataOptions.ApplyOptions();
            var x = Source.Query(qin =>
            {
                var q1 = qin;
                var q2 = queryOptions.ApplyTo(q1, ao.QuerySettings(), ao.IgnoredQueryOptions(ordered: false));
                return q2;
            });
            return x;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork?.Dispose();
            }
        }
        protected void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~AbstractUnitOfWorkODataControllerBase() { Dispose(false); }
    }
}