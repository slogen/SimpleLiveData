using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Scm.Linq
{
    public static class ExecuteExtensions
    {
        /// <summary>
        /// Execute the <paramref name="source"/>, returning if not exception gets thrown.
        /// 
        /// Usefull to just cause side-effects
        /// </summary>
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed", Justification =
            "That is precisely what execute does")]
        public static void Execute<TSource>(this IEnumerable<TSource> source)
        {
            source.LongCount();
        }
    }
}