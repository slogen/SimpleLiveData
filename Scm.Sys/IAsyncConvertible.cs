using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scm.Sys
{
    public interface IAsyncConvertible
    {
        Task<object> Convert(Type targetType, CancellationToken cancellationToken);
    }
}