using System;
using System.IO;
using Scm.Concurrency;

namespace Scm.Sys.StreamShaping
{
    public static class StreamExtensions
    {
        public static IClock DefaultClock { get; set; } = SystemClockUtc.Default;
        public static Stream Shape<TStream>(this TStream stream, IStreamShaping shaping)
            where TStream: Stream
            => new ShapedStream<TStream>(stream, shaping.Schedulers, shaping.Costs);
        public static Stream ShapeIops<TStream>(this TStream stream, IStreamShedulers streamShedulers)
            where TStream: Stream
            => new ShapedStream<TStream>(stream, streamShedulers, IopsStreamCost.Default);

        public static IStreamShedulers AveragingSchedulersPerSecond(
            double sharedBudget,
            double? separateWriteBudget,
            double? separateSeekBudget,
            TimeSpan? averageTimeSpan = null,
            IClock clock = null)
        {
            var timeSpan = averageTimeSpan ?? TimeSpan.FromSeconds(3);
            var clk = clock ?? DefaultClock;
            IActionScheduler MakeSheduler(double iops)
                => new TimeScheduler(new TimeAverageCostScheduler(timeSpan, iops * timeSpan.TotalSeconds, clk), clk);
            return new StreamSchedulers(
                MakeSheduler(sharedBudget),
                separateWriteBudget == null ? default(IActionScheduler) : MakeSheduler(separateWriteBudget.Value),
                separateSeekBudget == null ? default(IActionScheduler) : MakeSheduler(separateSeekBudget.Value));
        }


        public static Stream ShapeIops(this Stream stream,
            double sharedIops,
            double? separateWriteIops,
            double? separateSeekIops,
            TimeSpan? averageTimeSpan = null, IClock clock = null)
            => stream.ShapeIops(AveragingSchedulersPerSecond(sharedIops, separateWriteIops, separateSeekIops, averageTimeSpan, clock));
        public static Stream ShapeBandwidth<TStream>(this TStream stream, IStreamShedulers streamShedulers)
            where TStream: Stream
            => new ShapedStream<TStream>(stream, streamShedulers, BandwidthStreamCost.Default);
        public static Stream ShapeBandwidthBytesPerSecond(this Stream stream,
            double sharedBytesPerSecond,
            double? separateWriteBytesPerSecond,
            double? separateSeekBytesPerSecond,
            TimeSpan? averageTimeSpan = null, IClock clock = null)
            => stream.ShapeBandwidth(AveragingSchedulersPerSecond(sharedBytesPerSecond, separateWriteBytesPerSecond, separateSeekBytesPerSecond));
    }
}