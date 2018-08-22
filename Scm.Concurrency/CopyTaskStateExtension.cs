using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    public static class CopyTaskStateExtension
    {
        public static void CopyTo<T>(this Task<T> src, TaskCompletionSource<T> dst)
        {
            src.ContinueWith(t =>
            {
                var st = t.Status;
                switch (t.Status)
                {
                    case TaskStatus.Canceled:
                        dst.SetCanceled();
                        return;
                    case TaskStatus.Faulted:
                        dst.SetException(t.Exception.InnerExceptions);
                        return;
                    case TaskStatus.RanToCompletion:
                        dst.SetResult(t.Result);
                        return;
                    default:
                        break;
                }

                throw new NotSupportedException($"Unsupported task state in .ContinueWith: {st}");
            }, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
