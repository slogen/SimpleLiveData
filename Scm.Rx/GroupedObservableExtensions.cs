using System;
using System.Reactive.Linq;

namespace Scm.Rx
{
    public static class GroupedObservableExtensions
    {
        public static IObservable<IGroupedObservable<TKey, TElement2>> GroupedSelect<TKey, TElement, TElement2>(
            this IObservable<IGroupedObservable<TKey, TElement>> source,
            Func<IGroupedObservable<TKey, TElement>, IObservable<TElement2>> selector)
        {
            return source.Select(grp => selector(grp).Select(element => new {grp.Key, element})).Merge()
                .GroupBy(x => x.Key, x => x.element);
        }
    }
}