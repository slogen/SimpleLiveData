using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Scm.Sys
{
    public static class DynamicEnumerable
    {
        public static Type ElementType(this IEnumerable enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            var enumTypes = enumerable.GetType().GetInterfaces().Where(t =>
            {
                if (!t.IsGenericType)
                    return false;
                var tg = t.GetGenericTypeDefinition();
                return tg == typeof(IEnumerable<>);
            }).ToList();
            if (!enumTypes.Any())
                throw new ArgumentException($"Unable to deduce elementtype from a {enumerable.GetType()}");
            if (enumTypes.Count > 1)
                throw new ArgumentException(
                    $"Unable to deduce elementtype from a {enumerable.GetType()}. It implements IEnumerable<T> {enumTypes.Count} times"
                    + $"for: {string.Join(", ", enumTypes.Select(t => $"{t.GetGenericArguments().Single()}"))}");
            return enumTypes.Single().GenericArguments().Single();
        }
    }
}