using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Parquet;
using Scm.Sys;

namespace Scm.Presentation.Mvc.Parquet
{
    public abstract class AbstractParquetOutputFormatter : OutputFormatter
    {
        public CompressionMethod? CompressionMethod { get; set; } = null;
        public int? RowGroupSize { get; set; } = 5000;

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            // Even though we do not support application/json replay we apparently get called anyway?
            var accept = context.HttpContext.Request.GetTypedHeaders()?.Accept;
            var headerSupported = accept?.Any(a => SupportedMediaTypes.Any(m => a.MediaType == m)) ?? false;
            return  headerSupported && (context.Object as IEnumerable)?.ElementType() != null;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            // ReSharper disable PossibleMultipleEnumeration -- Only enumerated once
            var o = (IEnumerable)context.Object;
            var a = o.AsArray(o.ElementType());
            // ReSharper restore PossibleMultipleEnumeration

            var response = context.HttpContext.Response;
            await a.ParquetSerializeAsync(response.Body, context.HttpContext.RequestAborted, null, CompressionMethod, RowGroupSize).ConfigureAwait(false);
        }
    }
}