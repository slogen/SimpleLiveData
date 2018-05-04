using System;
using System.Reactive.Linq;

namespace Scm.DataAccess
{
    public static class SinkExtensions
    {
        public static IObservable<TEntity> Add<TEntity>(this ISink<TEntity> sink, IObservable<TEntity> entities)
            where TEntity : class
            => sink.Change(entities.Select(e => new {e, c = EntityChange.Add}).GroupBy(x => x.c, x => x.e))
                .Select(x => x.Entity);
    }
}