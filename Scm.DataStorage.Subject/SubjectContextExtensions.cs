using System;
using Scm.DataAccess;

namespace Scm.DataStorage.Subject
{
    public static class SubjectContextExtensions
    {
        public static ISink Sink<TContext>(this TContext context, Type entityType)
            where TContext : SubjectContext
            => context.Sink().Of(entityType);
    }
}