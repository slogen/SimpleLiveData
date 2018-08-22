using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Scm.Sys
{
    public static class DynamicEnumerable
    {
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
        public class InconclusiveElementTypeException : ArgumentException
        {
            public object SourceObject { get; }
            public IEnumerable<Type> CandidateTypes { get; }

            public InconclusiveElementTypeException(object sourceObject, IEnumerable<Type> candidateTypes, string argument)
            :base(MakeMessage(sourceObject, candidateTypes, argument), argument)
            {
                SourceObject = sourceObject;
                CandidateTypes = candidateTypes;
            }

            protected static string MakeMessage(object source, IEnumerable<Type> candidateTypes, string argument)
            {
                if (candidateTypes.Skip(1).Any())
                    return
                        $"Unable to deduce elementtype from {source}. It implements IEnumerable<T> multiples times"
                        + $"for: {string.Join(", ", candidateTypes.Select(t => $"{t.GetGenericArguments().Single()}"))}";
                return $"Unable to deduce elementtype from {source}";
            }
        }

        private static IEnumerable<Type> GenericTypes(this object o)
            => o.GetType().GetInterfaces()
                .Where(t => t.IsGenericType);
        public static Type ElementType(this IEnumerable enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));
            var enumTypes = enumerable.GenericTypes()
                .Where(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .ToList();
            if (enumTypes.Count != 1)
                throw new InconclusiveElementTypeException(enumerable, enumTypes, nameof(enumerable));
            return enumTypes.Single().GenericArguments().Single();
        }

        public static Type ElementType(this object o)
        {
            if (o == null)
                return null;
            var enumTypes = o.GenericTypes()
                    .Where(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .ToList();
            var asyncEnumTypes = o.GenericTypes()
                .Where(t => t.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
                .ToList();
            var candidates = enumTypes.Concat(asyncEnumTypes).Select(x => x.GenericTypeArguments.Single()).Distinct().ToList();
            if (candidates.Count <= 0 || candidates.Count > 1)
                throw new InconclusiveElementTypeException(o, enumTypes.Concat(asyncEnumTypes), nameof(o));
            return candidates.Single();
        }
    }
}