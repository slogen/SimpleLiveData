using System;
using Scm.DataAccess;

namespace Scm.DataStorage.Subject
{
    public static class SinkExtensions
    {
        public static SubjectSink<TContext> Sink<TContext>(this TContext context)
            where TContext : SubjectContext => new SubjectSink<TContext>(context);

        public class SubjectSink<TContext> where TContext : SubjectContext
        {
            public SubjectSink(TContext context)
            {
                Context = context;
            }

            public TContext Context { get; }

            public ISink<TEntity> Of<TEntity>() where TEntity : class
                => new SubjectSink<TEntity, TContext>(Context);

            public ISink Of(Type entityType) =>
                (ISink) Activator.CreateInstance(typeof(SubjectSink<,>).MakeGenericType(entityType, typeof(TContext)),
                    Context);
        }
    }
}