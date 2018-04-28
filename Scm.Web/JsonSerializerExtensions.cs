using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Scm.Web
{
    public static class JsonSerializerExtensions
    {
        public static Task SerializeAsync(this JsonSerializer serializer, JsonWriter writer, object value,
            Type objectType, CancellationToken cancellationToken)
            => Task.Factory.StartNew(() => serializer.Serialize(writer, value, objectType), cancellationToken);
    }
}