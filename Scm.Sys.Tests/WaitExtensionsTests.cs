using System;
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
            long[] onCancelCount = {0L};
            using (var cts = new CancellationTokenSource())
            {
                var task = sem.WaitAsync(cts.Token).OnCancel(() => Interlocked.Increment(ref onCancelCount[0]));
                Interlocked.Read(ref onCancelCount[0]).Should().Be(0);
                cts.Cancel();
                0.Awaiting(async _ => { await task.ConfigureAwait(false); })
                    .Should().NotThrow();
            }
            Interlocked.Read(ref onCancelCount[0]).Should().Be(1);
        }
        [Fact]
        public void OnCancelAsyncWillContinueOnCancellation()
        {
            var sem = new SemaphoreSlim(0);
            long[] onCancelCount = { 0L };
            using (var cts = new CancellationTokenSource())
            {
                var task = sem.WaitAsync(cts.Token).OnCancelAsync(() => Task.FromResult(Interlocked.Increment(ref onCancelCount[0])));
                Interlocked.Read(ref onCancelCount[0]).Should().Be(0);
                cts.Cancel();
                0.Awaiting(async _ => { await task.ConfigureAwait(false); })
                    .Should().NotThrow();
            }
            Interlocked.Read(ref onCancelCount[0]).Should().Be(1);
        }

        [Fact]
        public async Task OnCompletionWillInvokeWhenParentRanToCompletion()
        {
            var sem = new SemaphoreSlim(0);
            var task = sem.AvailableWaitHandle.WaitAsync(default(CancellationToken));
            sem.Release();
            var r = await task.OnCompletion(1).ConfigureAwait(false);
            r.Should().Be(1);
        }
        [Fact]
        public void OnCompletionWillNotInvokeWhenParentRanToCompletion()
        {
            var sem = new SemaphoreSlim(0);
            var callCount = 0L;
            using (var cts = new CancellationTokenSource())
            {
                // ReSharper disable once AccessToModifiedClosure -- intended
                var task = sem.WaitAsync(cts.Token).OnCompletion(() => Interlocked.Increment(ref callCount));
                cts.Cancel();
                0.Awaiting(async _ => { await task.ConfigureAwait(false); })
                    .Should().Throw<OperationCanceledException>();
            }
            Interlocked.Read(ref callCount).Should().Be(0);
        }
    }
}
