using System.Reactive.Subjects;

namespace Scm.DataAccess.Qbservable
{
    public class InstantMeet<TEntity> : AbstractInstantMeet<TEntity>
    {
        public InstantMeet(IMeet<TEntity> inner, ISubject<TEntity> instantSubject = null)
        {
            Inner = inner;
            InstantSubject = instantSubject ?? new Subject<TEntity>();
        }

        protected override IMeet<TEntity> Inner { get; }
        protected override ISubject<TEntity> InstantSubject { get; }
    }
}