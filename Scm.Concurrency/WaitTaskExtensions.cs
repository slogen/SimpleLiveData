using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Concurrency
{
    public static class WaitTaskExtensions
    {
        public static async Task WaitAsync(this Task task, CancellationToken cancellationToken)
        {
            await (await Task.WhenAny(task, Task.Delay(-1, cancellationToken)).ConfigureAwait(false)).ConfigureAwait(false);
        }
    }
}
