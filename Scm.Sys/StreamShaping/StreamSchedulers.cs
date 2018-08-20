using Scm.Concurrency;

namespace Scm.Sys.StreamShaping
{
    public class StreamSchedulers: IStreamShedulers
    {
        public static IActionScheduler DefaultScheduler { get; set; } = NoScheduler.Default;
        public StreamSchedulers(IActionScheduler readScheduler, IActionScheduler writeScheduler = null, IActionScheduler seekScheduler = null)
        {
            _readScheduler = readScheduler;
            _writeScheduler = writeScheduler;
            _seekScheduler = seekScheduler;
        }

        private readonly IActionScheduler _readScheduler;
        public IActionScheduler ReadScheduler => _readScheduler ?? DefaultScheduler;
        private readonly IActionScheduler _seekScheduler;
        public IActionScheduler SeekScheduler => _seekScheduler ?? ReadScheduler;

        private readonly IActionScheduler _writeScheduler;

        public IActionScheduler WriteScheduler => _writeScheduler ?? ReadScheduler;
    }
}
