using System;
using System.Reactive.Linq;

namespace Scm.DataAccess
{
    public static class SinkExtensions
    {
        public static IObservable<long> Add<TEntity>(this ISink<TEntity> sink, IObservable<TEntity> entities)
            => sink.Change(entities.Select(e => new {e, c = EntityChange.Add}).GroupBy(x => x.c, x => x.e));
    }
}