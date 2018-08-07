using System;
using System.Collections;

namespace Scm.Sys
{
    public static class DynamicArray
    {
        public static Array AsArray(this ICollection collection, Type elementType)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            if (elementType == null)
                throw new ArgumentNullException(nameof(elementType));
            if (collection is Array array && array.GetType().GetElementType() == elementType)
                return array;
            var a = Array.CreateInstance(elementType, collection.Count);
            collection.CopyTo(a, 0);
            return a;
        }
        public static Array AsArray(this IEnumerable enumerable, Type elementType)
        {
            if (!(enumerable is ICollection c))
                c = enumerable.AsList(elementType);
            return c.AsArray(elementType);
        }

    }
}
