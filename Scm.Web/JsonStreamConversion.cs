using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Scm.Web
{
    public class JsonStreamConversion : AbstractJsonTextReaderConversion
    {
        public JsonStreamConversion(JsonSerializer deserializer, Stream stream, Encoding encoding)
        {
            Deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            Stream = stream ?? throw new ArgumentNullException(nameof(stream));
            Encoding = encoding;
        }

        protected override JsonSerializer Deserializer { get; }
        protected Stream Stream { get; }
        protected Encoding Encoding { get; }

        protected override TextReader Reader
        {
            get { return new StreamReader(Stream, Encoding); }
        }
    }
}