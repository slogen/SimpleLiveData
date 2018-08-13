using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Scm.Sys
{
    public static class DynamicList
    {
        public static int DefaultInitialCapacity = 1000;
        public static IList Create(Type elementType, int? initialCapacity = null)
        {
            if (elementType == null)
                throw new ArgumentNullException(nameof(elementType));
            return (IList)Activator.CreateInstance(typeof(List<>)
                    .MakeGenericType(elementType), initialCapacity ?? DefaultInitialCapacity);
        }
        public static IList AsList(this IEnumerable enumerable, Type elementType, int? initialCapacity = null)
        {
            if ( enumerable == null )
                throw new ArgumentNullException(nameof(enumerable));
            if (elementType == null)
                throw new ArgumentNullException(nameof(elementType));
            // Already has some info?
            if (enumerable is ICollection lt)
            {
                // Might be exactly the right kind?
                var glt = enumerable.GetType().GetInterfaces().FirstOrDefault(t => {
                    if (!t.IsGenericType)
                        return false;
                    var tg = t.GetGenericTypeDefinition();
                    return tg == typeof(IList<>) && t.GetGenericArguments().Single() == elementType;
                });
                if (!ReferenceEquals(glt, null))
                    return (IList)lt;
                // Conversion required, we know length so prefer to create array
                return lt.AsArray(elementType);
            }
            // Not chance to predict length
            var l = Create(elementType, initialCapacity);
            foreach (var x in enumerable)
                l.Add(x);
            return l;
        }
    }
}
