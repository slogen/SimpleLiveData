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
            if (!headerSupported)
                return false;
            var elementType = context.Object.ElementType();
            return elementType != null;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var a = await context.Object.ObjectAsArray(context.HttpContext.RequestAborted).ConfigureAwait(false);
            var response = context.HttpContext.Response;
            await a.ParquetSerializeAsync(response.Body, context.HttpContext.RequestAborted, null, CompressionMethod, RowGroupSize).ConfigureAwait(false);
        }
    }
}