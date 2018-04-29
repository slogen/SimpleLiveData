using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;
using Scm.DataAccess;

namespace Scm.DataStorage.Subject
{
    public class SubjectContext
    {
        public SubjectContext(ConcurrentDictionary<Type, object> subjects = null)
        {
            Subjects = subjects ?? new ConcurrentDictionary<Type, object>();
        }

        private ConcurrentDictionary<Type, object> Subjects { get; }

        public virtual ISubject<IChange<TEntity>> Subject<TEntity>()
            => (ISubject<IChange<TEntity>>) Subject(typeof(TEntity));

        protected virtual object CreateSubject(Type entityType)
        {
            return Activator.CreateInstance(
                typeof(Subject<>).MakeGenericType(typeof(IChange<>).MakeGenericType(entityType)));
        }

        public object Subject(Type entityType)
            => Subjects.GetOrAdd(entityType, CreateSubject);
    }
}