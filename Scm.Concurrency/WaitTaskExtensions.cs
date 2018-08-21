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

        public static Task WaitAsync(this WaitHandle waitHandle, CancellationToken cancellationToken)
        {
            // Already completed?
            if (waitHandle.WaitOne(0))
                return Task.CompletedTask;

            var tcs = new TaskCompletionSource<int>();
            void Callback(object state, bool timedOut)
            {
                var cbTcs = (TaskCompletionSource<int>)state;
                cbTcs.TrySetResult(0);
            }
            var reg = ThreadPool.RegisterWaitForSingleObject(waitHandle, Callback, tcs, -1, executeOnlyOnce: true);
            var c = cancellationToken.CanBeCanceled ? cancellationToken.Register(() => tcs.TrySetCanceled())
                : default(CancellationTokenRegistration);
            // ReSharper disable once MethodSupportsCancellation -- this continuation should run even if task is cancelled
#pragma warning disable 4014 // Is continuation for cleanup, not for return
            tcs.Task.ContinueWith(t =>
#pragma warning restore 4014
            {
                if (c != default(CancellationTokenRegistration))
                    c.Dispose();
                reg.Unregister(waitHandle);
            });
            return tcs.Task;
        }

        public static async Task<TResult> OnCompletion<TResult>(this Task task, Func<Task<TResult>> f)
        {
            await task;
            return await f();
        }

        public static async Task<TResult> OnCancel<TResult>(
            this Task<TResult> task, Func<Task<TResult>> onCancellation)
        {
            try
            {
                return await task.ConfigureAwait(false);
            } catch (Exception) when (task.IsCanceled) // when (ex is TaskCanceledException)
            {
                return await onCancellation().ConfigureAwait(false);
            }
        }

        public static async Task OnCancel(this Task task, Func<Task> onCancellation)
            => await task.OnCompletion(() => Task.FromResult(0)).OnCancel(async () =>
            {
                await onCancellation().ConfigureAwait(false);
                return 0;
            }).ConfigureAwait(false);
    }
}