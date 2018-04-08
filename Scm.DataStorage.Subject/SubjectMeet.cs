using System.Reactive.Subjects;

namespace Scm.DataStorage.Subject
{
    public class SubjectMeet<TEntity>: AbstractSubjectMeet<TEntity>
    {
        protected override ISubject<TEntity> Subject { get; }
        public SubjectMeet(ISubject<TEntity> subject) { Subject = subject; }
    }
}
