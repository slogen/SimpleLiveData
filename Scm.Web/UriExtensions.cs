using System;
using System.Diagnostics.CodeAnalysis;

namespace Scm.Web
{
    public static class UriExtensions
    {
        public static string LastSegment(this Uri uri)
        {
            var segs = uri.Segments;
            return segs[segs.Length - 1];
        }

        [SuppressMessage("ReSharper", "TailRecursiveCall", Justification = "Tail recursion much more readable here")]
        public static Uri Slash(this Uri uri, string relativeUri)
            => uri.LastSegment().EndsWith("/")
                ? new Uri(uri, relativeUri)
                : new Uri(uri, uri.LastSegment() + "/").Slash(relativeUri);
    }
}