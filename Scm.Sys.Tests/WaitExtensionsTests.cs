using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Scm.Concurrency;
using Xunit;

namespace Scm.Sys.Tests
{
    public class WaitExtensionsTests
    {
        [Fact]
        public void WaitAsyncOnWaitHandleTerminatesWithCancellation()
        {
            var sem = new SemaphoreSlim(0);
            using (var cts = new CancellationTokenSource())
            {
                var task = sem.AvailableWaitHandle.WaitAsync(cts.Token);
                cts.Cancel();
                0.Awaiting(async _ => { await task.ConfigureAwait(false); }).Should().Throw<TaskCanceledException>();
            }
        }
        [Fact]
        public void WaitAsyncOnWaitHandleAlreadyReadyCoversHotPath()
        {
            var sem = new SemaphoreSlim(1);
            using (var cts = new CancellationTokenSource())
            {
                var task = sem.AvailableWaitHandle.WaitAsync(cts.Token);
                cts.Cancel();
                0.Awaiting(async _ => { await task.ConfigureAwait(false); }).Should().NotThrow();
            }
        }

        [Fact]
        public void WaitAsyncOnUncancellationTokenAvoidsRegistering()
        {
            var sem = new SemaphoreSlim(0);
            var task = sem.AvailableWaitHandle.WaitAsync(default(CancellationToken));
            sem.Release();
            0.Awaiting(async async => await task.ConfigureAwait(false)).Should().NotThrow();
        }


        [Fact]
        public void OnCancelWillContinueOnCancellation()
        {
            var sem = new SemaphoreSlim(0);
            var onCancelCount = 0L;
            async Task OnCancellation()
            {
                await Task.Yield();
                // ReSharper disable once AccessToModifiedClosure -- intended
                Interlocked.Increment(ref onCancelCount);
                await Task.Yield();
            }

            using (var cts = new CancellationTokenSource())
            {
                var task = sem.WaitAsync(cts.Token).OnCancel(OnCancellation);
                Interlocked.Read(ref onCancelCount).Should().Be(0);
                cts.Cancel();
                0.Awaiting(async _ => { await task.ConfigureAwait(false); })
                    .Should().NotThrow();
            }
            Interlocked.Read(ref onCancelCount).Should().Be(1);
        }
    }
}
