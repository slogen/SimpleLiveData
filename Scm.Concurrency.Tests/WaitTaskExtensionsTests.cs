using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Scm.Concurrency.Tests
{
    public class WaitTaskExcensionsTests
    {
        protected CancellationToken CancellationToken => default(CancellationToken);

        [Fact]
        public async Task WaitAsyncShouldCompleteWithTaskResult()
        {
            var x = await Task.FromResult(1).WaitAsync(CancellationToken);
            x.Should().Be(1);
        }
        [Fact]
        public async Task WaitAsyncShouldCompleteWithTask()
        {
            var t = Task.Delay(1, CancellationToken).WaitAsync(CancellationToken);
            await t.ConfigureAwait(false);
        }
        [Fact]
        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "await applied inside using")]
        public void WaitAsyncShouldCancelWhenCancelled()
        {
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(CancellationToken))
            {
                var t = Task.Delay(int.MaxValue, CancellationToken).WaitAsync(cts.Token);
                t.IsCompleted.Should().BeFalse();
                cts.Cancel();
                Func<Task> act = async () => await t.ConfigureAwait(false);
                act.Should().ThrowExactly<TaskCanceledException>();
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "new used to create side-effect in test")]
        public void InitializeWaitCountBelowZeroFails(int setCount)
        {
            setCount.Invoking(i => new AutoResetAsyncBarrier(i)).Should().Throw<ArgumentException>();
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement", Justification = "new used to create side-effect in test")]
        public void SettingWaitCountBelowZeroFails(int setCount)
        {
            new AutoResetAsyncBarrier(1).Invoking(b => b.WaitCount = setCount).Should().Throw<ArgumentException>();
        }
    }
}