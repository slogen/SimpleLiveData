using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    public static class WaitTaskExtensions
    {
        public static async Task WaitAsync(this Task task, CancellationToken cancellationToken)
        {
            await task.ContinueWith(t => 0, cancellationToken).WaitAsync(cancellationToken).ConfigureAwait(false);
        }

        public static async Task<TResult> WaitAsync<TResult>(this Task<TResult> task,
            CancellationToken cancellationToken)
        {
            await (await Task.WhenAny(task, Task.Delay(-1, cancellationToken)).ConfigureAwait(false))
                .ConfigureAwait(false);
            return await task;
        }
    }
}