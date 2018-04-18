using System;

namespace SimpleLiveData.App.Presentation
{
    public class QueryTimeSpan: IQueryTimeSpan
    {
        private class DefaultQueryTimeSpan : IQueryTimeSpan
        {
            // TODO: Read from config
            public static DefaultQueryTimeSpan Default { get; } = new DefaultQueryTimeSpan();
            public TimeSpan? QueryTimeout => TimeSpan.FromSeconds(10);
        }

        public static IQueryTimeSpan DefaultInstance { get; set; } = DefaultQueryTimeSpan.Default;
        public TimeSpan? QueryTimeout { get; set; }
    }
}