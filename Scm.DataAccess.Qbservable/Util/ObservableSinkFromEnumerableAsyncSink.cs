using System;
using Scm.DataAccess.Queryable;

namespace Scm.DataAccess.Qbservable.Util
{
    public class ObservableSinkFromEnumerableAsyncSink<TEntity>: AbstractObservableSinkFromEnumerableAsyncSink<TEntity>
        where TEntity: class
    {
        public ObservableSinkFromEnumerableAsyncSink(IEnumerableAsyncSink<TEntity> sink, TimeSpan? timeSpan, int? ChunkSize)
        {
            _sink = sink;
            _chunkSize = ChunkSize ?? DefaultChunkSize;
            _timeSpan = timeSpan ?? DefaultTimeSpan;
        }
        private IEnumerableAsyncSink<TEntity> _sink;
        public override IEnumerableAsyncSink<TEntity> Sink { get; }
        private int? _chunkSize;
        public override int? ChunkSize => _chunkSize ?? DefaultChunkSize;
        private TimeSpan? _timeSpan;
        public override TimeSpan? TimeSpan => _timeSpan ?? DefaultTimeSpan;
        public static int DefaultChunkSize = 1000;
        public static TimeSpan DefaultTimeSpan = System.TimeSpan.FromSeconds(1);
    }
}
