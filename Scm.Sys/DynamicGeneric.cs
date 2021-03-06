﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Scm.Sys
{
    public static class DynamicGeneric
    {
        public static IEnumerable<Type> GenericArguments(this Type t)
        {
            if (t == null)
                throw new ArgumentNullException(nameof(t));
            return !t.IsGenericType ? Enumerable.Empty<Type>() : t.GetGenericArguments();
        }
    }
}