using System;
using System.Diagnostics.CodeAnalysis;
using Scm.DataAccess.Queryable;

namespace Scm.DataAccess.Qbservable.Util
{
    public class ObservableSinkFromEnumerableAsyncSink
    {
        public static int DefaultChunkSize { get; set; } = 1000;
        public static TimeSpan DefaultTimeSpan { get; set; } = TimeSpan.FromSeconds(1);
    }

    public class ObservableSinkFromEnumerableAsyncSink<TEntity> : AbstractObservableSinkFromEnumerableAsyncSink<TEntity>
        where TEntity : class
    {
        private readonly int? _chunkSize;
        private readonly TimeSpan? _timeSpan;

        public ObservableSinkFromEnumerableAsyncSink(IEnumerableAsyncSink<TEntity> sink, TimeSpan? timeSpan,
            int? chunkSize)
        {
            Sink = sink;
            _chunkSize = chunkSize;
            _timeSpan = timeSpan;
        }

        [SuppressMessage("ReSharper", "StaticMemberInGenericType",
            Justification = "Default for specific TEntity")]
        public static int? DefaultChunkSize { get; set; } = null;

        [SuppressMessage("ReSharper", "StaticMemberInGenericType",
            Justification = "Default for specific TEntity")]
        public static TimeSpan? DefaultTimeSpan { get; set; } = null;

        public override IEnumerableAsyncSink<TEntity> Sink { get; }

        public override int? ChunkSize =>
            _chunkSize ?? DefaultChunkSize ?? ObservableSinkFromEnumerableAsyncSink.DefaultChunkSize;

        public override TimeSpan? TimeSpan =>
            _timeSpan ?? DefaultTimeSpan ?? ObservableSinkFromEnumerableAsyncSink.DefaultTimeSpan;
    }
}