using System.Reactive.Subjects;
using Scm.DataAccess.Qbservable;

namespace Scm.DataStorage.Subject
{
    public static class SubjectMeetExtensions
    {
        public static IMeet<TEntity> ToMeet<TEntity>(this ISubject<TEntity> subject)
            => new SubjectMeet<TEntity>(subject);
    }
}
