using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Scm.Sys;

namespace Scm.Web
{
    public abstract class AbstractJsonTextReaderConversion : IAsyncConvertible
    {
        protected abstract JsonSerializer Deserializer { get; }
        protected abstract TextReader Reader { get; }

        public async Task<object> Convert(Type targetType, CancellationToken cancellationToken)
        {
            using (var r = new JsonTextReader(Reader))
            {
                return await Deserializer.DeserializeAsync(r, targetType, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}