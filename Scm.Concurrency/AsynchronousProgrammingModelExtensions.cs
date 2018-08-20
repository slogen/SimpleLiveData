using System;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    /// <summary>
    /// Helper functions for conversion from Task to BeginX/EndX model
    /// </summary>
    /// See <a href="https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/interop-with-other-asynchronous-patterns-and-types"/>
    public static class AsynchronousProgrammingModelExtensions
    {
        public static IAsyncResult ToApm(this Task task, AsyncCallback callback, object state)
            => task.ContinueWith(_ => 0).ToApm(callback, state);
        public static IAsyncResult ToApm<T>(this Task<T> task, AsyncCallback callback, object state)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));
            var tcs = new TaskCompletionSource<T>(state);
            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                    tcs.TrySetException(t.Exception.InnerExceptions);
                else if (t.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(t.Result);

                callback?.Invoke(tcs.Task);
            }, TaskScheduler.Default);
            return tcs.Task;
        }
    }
}
