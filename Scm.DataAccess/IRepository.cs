using System.Linq;

namespace Scm.DataAccess
{
    public interface IRepository
    {
        // TODO: add non-generic stuff?
    }

    public interface IRepository<TEntity> : ISink<TEntity>
        where TEntity : class
    {
        IQueryable<TEntity> Queryable { get; }
    }
}