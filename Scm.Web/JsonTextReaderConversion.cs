using System;
using System.IO;
using Newtonsoft.Json;

namespace Scm.Web
{
    public class JsonTextReaderConversion : AbstractJsonTextReaderConversion
    {
        public JsonTextReaderConversion(JsonSerializer deserializer, TextReader reader)
        {
            Deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        protected override JsonSerializer Deserializer { get; }

        protected override TextReader Reader { get; }
    }
}