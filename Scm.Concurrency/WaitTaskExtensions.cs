using System;
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

        public static async Task<TResult> WaitAsync<TResult>(this Task<TResult> task, CancellationToken cancellationToken)
        {
            using (var cts = cancellationToken.Link())
            {
                await (await Task.WhenAny(task, Task.Delay(-1, cts.Token)).ConfigureAwait(false))
                    .ConfigureAwait(false);
                return await task;

            }
        }

        public static async Task WaitAsync(this WaitHandle waitHandle, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<int>();
            void Callback(object state, bool timedOut)
            {
                var cbTcs = (TaskCompletionSource<int>)state;
                if (timedOut)
                    cbTcs.TrySetCanceled();
                else
                    cbTcs.TrySetResult(0);
            }
            var reg = ThreadPool.RegisterWaitForSingleObject(waitHandle, Callback, tcs, -1, executeOnlyOnce: true);
            var c = cancellationToken.CanBeCanceled ? cancellationToken.Register(() => tcs.TrySetCanceled())
                : default(CancellationTokenRegistration);
            // ReSharper disable once MethodSupportsCancellation -- this continuation should run even if task is cancelled
            await tcs.Task.ContinueWith(t =>
            {
                if (c != default(CancellationTokenRegistration))
                    c.Dispose();
                reg.Unregister(waitHandle);
            }).ConfigureAwait(false);
        }

        public static async Task<TResult> OnCancel<TResult>(
            this Task<TResult> task, Func<Task<TResult>> onCancellation)
        {
            try
            {
                return await task.ConfigureAwait(false);
            } catch (Exception ex) when (ex is TaskCanceledException)
            {
                return await onCancellation().ConfigureAwait(false);
            }
        }
    }
}