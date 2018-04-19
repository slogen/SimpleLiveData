using System;

namespace Scm.Presentation.OData
{
    public class QueryTimeSpan : IQueryTimeSpan
    {
        public static IQueryTimeSpan DefaultInstance { get; set; } = DefaultQueryTimeSpan.Default;
        public TimeSpan? QueryTimeout { get; set; }

        private class DefaultQueryTimeSpan : IQueryTimeSpan
        {
            // TODO: Read from config
            public static DefaultQueryTimeSpan Default { get; } = new DefaultQueryTimeSpan();
            public TimeSpan? QueryTimeout => TimeSpan.FromSeconds(10);
        }
    }
}