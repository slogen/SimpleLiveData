using System;
using Scm.DataStorage.Subject;

namespace Scm.DataAccess.Rx
{
    public static class MeetExtensions
    {
        public static SubjectMeet<TContext> Meet<TContext>(this TContext context)
            where TContext : SubjectContext => new SubjectMeet<TContext>(context);

        public class SubjectMeet<TContext> where TContext : SubjectContext
        {
            public SubjectMeet(TContext context)
            {
                Context = context;
            }

            public TContext Context { get; }

            public IMeet<TEntity> Of<TEntity>() where TEntity : class
                => new SubjectMeet<TEntity, TContext>(Context);

            public IMeet Of(Type entityType) =>
                (IMeet) Activator.CreateInstance(typeof(SubjectMeet<,>).MakeGenericType(entityType, typeof(TContext)),
                    Context);
        }
    }
}