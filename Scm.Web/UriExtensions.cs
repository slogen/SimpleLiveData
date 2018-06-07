using System;

namespace Scm.Web
{
    public static class UriExtensions
    {
        public static string LastSegment(this Uri uri)
        {
            var segs = uri.Segments;
            return segs[segs.Length - 1];
        }

        public static Uri Slash(this Uri uri, string relativeUri)
            => uri.LastSegment().EndsWith("/")
                ? new Uri(uri, relativeUri)
                : new Uri(uri, uri.LastSegment() + "/").Slash(relativeUri);
    }
}