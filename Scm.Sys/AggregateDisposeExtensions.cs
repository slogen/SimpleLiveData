using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scm.Sys
{
    public static class AggregateDisposeExtensions
    {
        public static void DisposeAll<T>(this IEnumerable<T> disposables) where T: IDisposable
        {
            var exceptions = new List<Exception>();
            foreach(var d in disposables) 
                try
                {
                    d.Dispose();
                } catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            if (exceptions.Any())
                throw new AggregateException(exceptions);
        }
    }
}
