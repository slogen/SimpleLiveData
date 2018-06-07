using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Scm.Concurrency.Tests
{
    public class AsyncBarrierTests
    {
        protected CancellationToken CancellationToken => default(CancellationToken);

        public static IEnumerable<object[]> CreateWaitBarriers(int waitCount) =>
            new[]
            {
                new object[] {new ManualResetAsyncBarrier(waitCount)},
                new[] {new AutoResetAsyncBarrier(waitCount)}
            };

        [Theory]
        [MemberData(nameof(CreateWaitBarriers), parameters: 1)]
        protected void AsyncBarrierCompletesImmediatelyForFirst(AbstractAsyncBarrier barrier)
        {
            var waitingTask = barrier.WaitAsync(CancellationToken);
            waitingTask.Status.Should().Be(TaskStatus.RanToCompletion);
        }

        [Theory]
        [MemberData(nameof(CreateWaitBarriers), parameters: 2)]
        public void AsyncBarrierOf2CompletesWhenSecondArrives(AbstractAsyncBarrier barrier)
        {
            var waitingTask1 = barrier.WaitAsync(CancellationToken);
            waitingTask1.IsCompleted.Should().BeFalse();
            var waitingTask2 = barrier.WaitAsync(CancellationToken);
            waitingTask1.Status.Should().Be(TaskStatus.RanToCompletion);
            waitingTask2.Status.Should().Be(TaskStatus.RanToCompletion);
            waitingTask1.Should().BeSameAs(waitingTask2);
        }

        [Theory]
        [MemberData(nameof(CreateWaitBarriers), parameters: 5)]
        public void CancellationOfWaitForAsyncBarrierPropogates(AbstractAsyncBarrier barrier)
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken))
            {
                var w = new[]
                {
                    barrier.WaitAsync(CancellationToken),
                    barrier.WaitAsync(cts.Token),
                    barrier.WaitAsync(CancellationToken),
                    barrier.WaitAsync(cts.Token)
                };
                cts.Cancel();
                w.Should().OnlyContain(x => x.IsCanceled,
                    because: "Cancellation should be synchroneously distributed to all waiters");
            }
        }

        [Fact]
        public void AutoResetBarrierShouldAutoResetUponCancellation()
        {
            var b = new AutoResetAsyncBarrier(4);
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken))
            {
                var tA = new[]
                {
                    b.WaitAsync(CancellationToken),
                    b.WaitAsync(cts.Token),
                    b.WaitAsync(CancellationToken)
                };
                cts.Cancel();
                var tB2 = b.WaitAsync(CancellationToken);
                tB2.IsCompleted.Should().BeFalse();
                var tBrest = new[]
                {
                    b.WaitAsync(CancellationToken),
                    b.WaitAsync(CancellationToken),
                    b.WaitAsync(CancellationToken)
                };
                tB2.IsCompletedSuccessfully.Should().BeTrue();
                tBrest.Should().OnlyContain(t => t.IsCompletedSuccessfully);
            }
        }

        [Fact]
        public void AutoResetBarrierShouldAutoResetUponCompletion()
        {
            var b = new AutoResetAsyncBarrier(2);
            var t1 = b.WaitAsync(CancellationToken);
            var t2 = b.WaitAsync(CancellationToken);
            var t3 = b.WaitAsync(CancellationToken);
            t3.IsCompleted.Should().BeFalse();
            var t4 = b.WaitAsync(CancellationToken);
            t3.IsCompleted.Should().BeTrue();
            t4.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void ManualResetBarrierShouldNotResetUponCancellation()
        {
            var b = new ManualResetAsyncBarrier(4);
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken))
            {
                var tA = new[]
                {
                    b.WaitAsync(CancellationToken),
                    b.WaitAsync(cts.Token),
                    b.WaitAsync(CancellationToken)
                };
                cts.Cancel();
                var tA2 = b.WaitAsync(CancellationToken);
                tA2.IsCanceled.Should().BeTrue();
                b.Reset();
                var tB = new[]
                {
                    b.WaitAsync(CancellationToken),
                    b.WaitAsync(CancellationToken),
                    b.WaitAsync(CancellationToken),
                    b.WaitAsync(CancellationToken)
                };
                tB.Should().OnlyContain(t => t.IsCompletedSuccessfully);
            }
        }

        [Fact]
        public void ManualResetBarrierShouldResetWhenManuallyReset()
        {
            var b = new ManualResetAsyncBarrier(2);
            var t1 = b.WaitAsync(CancellationToken);
            var t2 = b.WaitAsync(CancellationToken);
            var t3 = b.WaitAsync(CancellationToken);
            t3.IsCompleted.Should().BeTrue();
            b.Reset();
            var t4 = b.WaitAsync(CancellationToken);
            t4.IsCompleted.Should().BeFalse();
            var t5 = b.WaitAsync(CancellationToken);
            t4.IsCompleted.Should().BeTrue();
            t5.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void SettingWaitCountTriggersCancelOnManual()
        {
            var b = new ManualResetAsyncBarrier(2);
            var t1 = b.WaitAsync(CancellationToken);
            b.WaitCount = 2;
            t1.IsCanceled.Should().BeTrue();
            var t2 = b.WaitAsync(CancellationToken);
            t2.IsCanceled.Should().BeTrue();
            var t3 = b.WaitAsync(CancellationToken);
            t3.IsCanceled.Should().BeTrue();
            b.Reset();
            var t4 = b.WaitAsync(CancellationToken);
            t4.IsCompleted.Should().BeFalse();
            var t5 = b.WaitAsync(CancellationToken);
            t4.IsCompletedSuccessfully.Should().BeTrue();
            t5.IsCompletedSuccessfully.Should().BeTrue();
        }
        [Fact]
        public void SettingWaitCountTriggersCancelOnAuto()
        {
            var b = new AutoResetAsyncBarrier(2);
            var t1 = b.WaitAsync(CancellationToken);
            b.WaitCount = 2;
            t1.IsCanceled.Should().BeTrue();
            var t2 = b.WaitAsync(CancellationToken);
            t2.IsCompleted.Should().BeFalse();
            var t3 = b.WaitAsync(CancellationToken);
            t2.IsCompleted.Should().BeTrue();
            t3.IsCompleted.Should().BeTrue();
        }
    }
}