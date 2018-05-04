using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.DataAccess
{
    public static class UnitOfWorkExtensions
    {
        public static async Task InUnitOfWorkAsync<TUnitOfWork>(
            this Func<Task<TUnitOfWork>> factory,
            Func<TUnitOfWork, Task> f,
            CancellationToken cancellationToken)
            where TUnitOfWork : IAsyncUnitOfWork
        {
            using (var uow = await factory().ConfigureAwait(false))
            {
                await f(uow).ConfigureAwait(false);
                await uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task InUnitOfWorkAsync<TUnitOfWork>(
            this Func<CancellationToken, Task<TUnitOfWork>> factory,
            Func<TUnitOfWork, Task> f,
            CancellationToken cancellationToken)
            where TUnitOfWork : IAsyncUnitOfWork
        {
            using (var uow = await factory(cancellationToken).ConfigureAwait(false))
            {
                await f(uow).ConfigureAwait(false);
                await uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task InUnitOfWorkAsync<TUnitOfWork>(
            this CancellationToken cancellationToken,
            Func<CancellationToken, Task<TUnitOfWork>> factory,
            Func<TUnitOfWork, Task> f)
            where TUnitOfWork : IAsyncUnitOfWork
        {
            using (var uow = await factory(cancellationToken).ConfigureAwait(false))
            {
                await f(uow).ConfigureAwait(false);
                await uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task<TResult> InUnitOfWorkAsync<TUnitOfWork, TResult>(
            this Func<Task<TUnitOfWork>> factory,
            Func<TUnitOfWork, Task<TResult>> f,
            CancellationToken cancellationToken)
            where TUnitOfWork : IAsyncUnitOfWork
        {
            using (var uow = await factory().ConfigureAwait(false))
            {
                var v = await f(uow).ConfigureAwait(false);
                await uow.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return v;
            }
        }
    }
}