using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Scm.Concurrency;
using Scm.Sys.StreamShaping;
using Xunit;

namespace Scm.Sys.Tests.StreamScheduling
{
    public class StreamSchedulingTests
    {
        protected CancellationToken CancellationToken => default(CancellationToken);
        protected LimitedWait Waiter => LimitedWait.Default;
        /// <summary>
        /// Write data to <paramref name="s"/> in array with data left and right, to provoke issues
        /// </summary>
        protected async Task WriteAsync(Stream s, int startval, int count, CancellationToken? cancellationToken = null, int? left = null, int? right = null)
        {
            var l = left ?? 1;
            var r = right ?? 1;
            var buf = new byte[count + l + r];
            for (var i = 0; i < count; ++i)
                buf[l + i] = (byte) ((startval + i) & 0xFF);
            await s.WriteAsync(buf, l, count, cancellationToken ?? CancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Read data from <paramref name="s"/> in array with data left and right, to provoke issues
        /// </summary>
        protected async Task<IList<byte>> ReadAsync(Stream s, int count, CancellationToken? cancellationToken = null, int? left = null, int? right = null)
        {
            var l = left ?? 1;
            var r = right ?? 1;
            var buf = new byte[count + l + r];
            var read = await s.ReadAsync(buf, l, count, cancellationToken ?? CancellationToken).ConfigureAwait(false);
            return buf.Skip(l).Take(read).ToList();
        }

        [Theory]
        [InlineData(1,1)]
        [InlineData(3, 7)]
        public async Task CopyToAppliesStreamScheduling(int blocks, int blockSize)
        {
            var clock = new ManualClock();
            var src = new MemoryStream(Enumerable.Range(0, blockSize * blocks).Select(x => (byte) x).ToArray());
            var scheduler = new ShapeStreamAverage(BandwidthStreamCost.Default, blockSize,
                averageSpan: TimeSpan.FromSeconds(1), clock: clock);
            var dst = new MemoryStream();
            var bw = new ReadMaxStream<Stream>(src.Shape(scheduler), blockSize);
            var cpTask = bw.CopyToAsync(dst, blockSize, CancellationToken);
            for (var i = 0; i < blocks; ++i)
            {
                await Waiter.TimesOut(cpTask.WaitAsync, cancellationToken: CancellationToken)
                    .ConfigureAwait(false);
                dst.Length.Should().BeLessOrEqualTo((i + 1) * blockSize);
                await Task.WhenAll(clock.Advance(TimeSpan.FromSeconds(1))).ConfigureAwait(false);
            }

            await Waiter.WaitAsync(cpTask.WaitAsync, cancellationToken: CancellationToken)
                .ConfigureAwait(false);
            dst.Length.Should().Be(blocks * blockSize);
            dst.ToArray()
                .Should()
                .BeEquivalentTo(src.ToArray(), cfg => cfg.WithStrictOrdering());
        }

        [Fact]
        public async Task StreamShedulingIsAppliedAccordingToClockAndSchedulers()
        {
            var clock = new ManualClock(new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            var stream = new MemoryStream();
            var scheduler = new ShapeStreamAverage(BandwidthStreamCost.Default, 1000, 2000, 10,
                averageSpan: TimeSpan.FromSeconds(5), clock: clock);
            var bw = stream.Shape(scheduler);

            // Writing (2000b/s over 5s = 10kb/5s)
            await WriteAsync(bw, 1, 9000).ConfigureAwait(false);
            stream.Length.Should().Be(9000);

            await WriteAsync(bw, 2, 1000).ConfigureAwait(false);
            stream.Length.Should().Be(10000);

            await Task.WhenAll(clock.Advance(TimeSpan.FromSeconds(4), CancellationToken)).ConfigureAwait(false);

            // Exceed the shaping: will timeout and not get invoked
            await Waiter.TimesOut(ct => WriteAsync(bw, 3, 2000, ct), cancellationToken: CancellationToken).ConfigureAwait(false);
            stream.Length.Should().Be(10000);

            // Try writing again
            var wtask = WriteAsync(bw, 4, 2000);
            // That won't run immediately
            await Waiter.TimesOut(wtask.WaitAsync, cancellationToken: CancellationToken).ConfigureAwait(false);
            // And one second is not enough, as we are accounted the 2000 from time timed out write
            await Task.WhenAll(clock.Advance(TimeSpan.FromSeconds(1), CancellationToken)).ConfigureAwait(false);
            await Waiter.TimesOut(wtask.WaitAsync, cancellationToken: CancellationToken).ConfigureAwait(false);
            // So we need another second 
            await Task.WhenAll(clock.Advance(TimeSpan.FromSeconds(1), CancellationToken));
            // Now, It will be completed (pretty quickly)
            await Waiter.WaitAsync(wtask.WaitAsync, cancellationToken: CancellationToken);
            // And now have the required state
            stream.Length.Should().Be(12000);

            stream.Position = 0;
            var streamBytes = stream.ToArray();
            IList<IList<byte>> readParts = new List<IList<byte>>();
            // ReSharper disable once ImplicitlyCapturedClosure - bw, this captured
            void AssertReadCorrect() 
                => readParts.Aggregate(0, (offset, part) =>
                {
                    part.Should().BeEquivalentTo(streamBytes.Skip(offset).Take(part.Count));
                    return offset + part.Count;
                });

            // Reading (1000b/s over 5s = 50kb/5s)
            readParts.Add(await ReadAsync(bw, 4999, CancellationToken).ConfigureAwait(false));
            AssertReadCorrect();
            readParts.Add(await ReadAsync(bw, 1).ConfigureAwait(false));
            AssertReadCorrect();

            // Exceed the shaping: will timeout and not get invoked
            // ReSharper disable once ImplicitlyCapturedClosure -- streamBytes captured
            await Waiter.TimesOut(async ct =>  readParts.Add(await ReadAsync(bw, 1, ct).ConfigureAwait(false)), cancellationToken: CancellationToken).ConfigureAwait(false);
            AssertReadCorrect();

            // Try reading again
            var rtask = ReadAsync(bw, 6000);
            // That won't run immediately
            await Waiter.TimesOut(rtask.WaitAsync, cancellationToken: CancellationToken).ConfigureAwait(false);
            // And 6 seconds is not enough, as we are exceeding average in the read itself and pay for the extra 1 failing byte
            await Task.WhenAll(clock.Advance(TimeSpan.FromSeconds(6), CancellationToken)).ConfigureAwait(false);
            await Waiter.TimesOut(rtask.WaitAsync, cancellationToken: CancellationToken).ConfigureAwait(false);
            // But a little more is
            await Task.WhenAll(clock.Advance(TimeSpan.FromSeconds(.1), CancellationToken)).ConfigureAwait(false);
            // Now, It will be completed (pretty quickly)
            readParts.Add(await Waiter.WaitAsync(rtask.WaitAsync, cancellationToken: CancellationToken));
            AssertReadCorrect();
            readParts.Sum(x => x.Count).Should().Be(4999 + 1 + 6000);
        }
    }

    public class LimitedWait
    {
        public static LimitedWait Default = new LimitedWait();
        public static TimeSpan DefaultDebugLimitedWaitTime { get; set; } = TimeSpan.FromMinutes(60);
        public TimeSpan? DebugLimitedWaitTime;
        public static TimeSpan DefaultLimitedWaitTime { get; set; } = TimeSpan.FromMilliseconds(50);
        public TimeSpan? LimitedWaitTime;
        public virtual TimeSpan WaitTime(TimeSpan? spanWhenNotDebugging)
            => Debugger.IsAttached ? (DebugLimitedWaitTime ?? DefaultDebugLimitedWaitTime)
            : (spanWhenNotDebugging ?? LimitedWaitTime ?? DefaultLimitedWaitTime);
        public async Task<TResult> WaitAsync<TResult>(Func<CancellationToken, Task<TResult>> f, TimeSpan? spanWhenNotDebugging = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var cts = cancellationToken.Timeout(WaitTime(spanWhenNotDebugging)))
                return await f(cts.Token).ConfigureAwait(false);
        }
        public async Task WaitAsync(Func<CancellationToken, Task> f, TimeSpan? spanWhenNotDebugging = null, CancellationToken cancellationToken = default(CancellationToken))
            => await WaitAsync(async ct => { await f(ct).ConfigureAwait(false); return 0; }, spanWhenNotDebugging, cancellationToken);
        public async Task TimesOut(Func<CancellationToken, Task> f, TimeSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var cts = cancellationToken.Timeout(span ?? LimitedWaitTime ?? DefaultLimitedWaitTime))
                try
                {
                    await f(cts.Token).ConfigureAwait(false);
                }
                catch (Exception) when (cts.Token.IsCancellationRequested)
                {
                    return; // That was the expected behaviour
                }
            throw new InvalidOperationException("Expected timeout");
        }
    }
}
