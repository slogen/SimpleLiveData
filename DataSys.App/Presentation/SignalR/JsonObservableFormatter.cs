using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Scm.Web;

namespace DataSys.App.Presentation.SignalR
{
    public class JsonObservableFormatter<T> : TextOutputFormatter
    {
        protected Type TargetType = typeof(IObservable<T>);

        public override bool CanWriteResult(OutputFormatterCanWriteContext context)
        {
            return TargetType.IsAssignableFrom(context.ObjectType)
                   && context.ContentType.EndsWith("json", StringComparison.CurrentCultureIgnoreCase);
        }

        protected override bool CanWriteType(Type type)
        {
            return TargetType.IsAssignableFrom(type);
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure", Justification = "await ensures lifetime")]
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context,
            Encoding selectedEncoding)
        {
            var ct = context.HttpContext.RequestAborted;
            var ser = new JsonSerializer();
            using (var w = context.WriterFactory(context.HttpContext.Response.Body, selectedEncoding))
            using (var jw = new JsonTextWriter(w))
            {
                await ser.SerializeAsync(jw, ((IObservable<T>) context.Object).ToEnumerable(),
                        objectType: typeof(IEnumerable<T>), cancellationToken: ct)
                    .ConfigureAwait(false);
            }
        }
    }
}