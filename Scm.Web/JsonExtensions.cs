using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Scm.Web
{
    public static class JsonExtensions
    {
        public static Task<object> DeserializeAsync(this JsonSerializer jsonSerializer, JsonTextReader reader,
            Type targetType, CancellationToken cancellationToken)
            => Task.Factory.StartNew(() => jsonSerializer.Deserialize(reader, targetType), cancellationToken);
    }
}