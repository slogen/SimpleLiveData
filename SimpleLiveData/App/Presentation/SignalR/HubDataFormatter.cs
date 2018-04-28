using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Scm.Web;

namespace SimpleLiveData.App.Presentation.SignalR
{
    public class HubDataFormatter : TextOutputFormatter
    {
        protected Type TargetType = typeof(IObservable<HubData>);

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
                await ser.SerializeAsync(jw, ((IObservable<HubData>) context.Object).ToEnumerable(),
                        objectType: typeof(IEnumerable<HubData>), cancellationToken: ct)
                    .ConfigureAwait(false);
            }
        }
    }
}