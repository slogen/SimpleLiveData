using System;
using Scm.Sys;

namespace Scm.Rx.Trace
{
    public class ActionHereTracer : AbstractHereTracer
    {
        public Action<string, object[]> Action { get; set; }
        public ActionHereTracer(Action<string, object[]> action, ICallerInfo callerInfo, bool? enabled = null) : base(callerInfo, enabled)
        {
            Action = action;
        }

        protected override void WriteLine(string format, params object[] args)
            => Action?.Invoke(format, args);
    }
}