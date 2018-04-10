using System;
using System.Threading;

namespace Scm.Concurrency
{
    public interface ICancellationScope : IDisposable
    {
        CancellationToken Token { get; }
    }
}