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
            if (enumTypes.Count != 1)
                throw new InconclusiveElementTypeException(enumerable, enumTypes, nameof(enumerable));
            return enumTypes.Single().GenericArguments().Single();
        }
    }
}