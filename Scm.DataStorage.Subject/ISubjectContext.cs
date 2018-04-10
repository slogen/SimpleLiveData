using Scm.DataAccess;
using Scm.DataAccess.Qbservable;

namespace Scm.DataStorage.Subject
{
    public interface ISubjectContext : IAsyncUnitOfWork
    {
        IMeet<TEntity> Meet<TEntity>();
    }
}