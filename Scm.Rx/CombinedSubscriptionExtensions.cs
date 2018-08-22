using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Scm.Rx
{
    public static class CombinedSubscriptionExtensions
    {

        /// <summary>
        /// Simplified version of <see cref="CombinedSubscriptions{TKey,TAgg,TElement}"/>
        /// </summary>
        public static Func<TKey, IObservable<TElement>> CombinedSubscriptions<TKey, TElement>(
            this Func<TKey, IObservable<TElement>> sourceFactory,
            Func<TKey, TKey, TKey> aggregate,
            TKey seed = default(TKey),
            IEqualityComparer<TKey> keyComparer = null)
            => sourceFactory.CombinedSubscriptions<TKey, TKey, TElement>(aggregate, seed, keyComparer);

        /// <summary>
        /// Create a function that will generate an <paramref name="aggregate"/> from all returned observables, and switch subscription to <paramref name="sourceFactory"/> accordingly
        /// </summary>
        /// <remarks>This is useful for ordering a union of stuff from an expensive source. Each observer places his request and an aggregate is requested and distributed to all listeners</remarks>
        /// <typeparam name="TKey">Key type for combination of subscriptions</typeparam>
        /// <typeparam name="TElement"><see cref="IObservable{T}"/></typeparam>
        /// <typeparam name="TAgg">Type of aggregate across <typeparamref name="TKey"/></typeparam>
        /// <param name="sourceFactory">Factory that <see cref="CombinedSubscriptions{TKey,TElement}"/> can invoke to get a new observable when a new <paramref name="aggregate"/> is seen</param>
        /// <param name="aggregate">Aggregator applied to all the keys currently having their results subscribed in order to produce <typeparamref name="TAgg"/> for <paramref name="sourceFactory"/> </param>
        /// <param name="seed">Seed to start aggregator on</param>
        /// <param name="keyComparer">Comparer used to decide when <typeparamref name="TAgg"/> are equal. Defaults to <see cref="EqualityComparer{TAgg}"/>.Default</param>
        public static Func<TKey, IObservable<TElement>> CombinedSubscriptions<TKey, TAgg, TElement>(
            this Func<TAgg, IObservable<TElement>> sourceFactory,
            Func<TAgg, TKey, TAgg> aggregate,
            TAgg seed = default(TAgg),
            IEqualityComparer<TAgg> keyComparer = null)
        {
            // Source that expresses all the function calls to the returned function
            var calls = new Subject<IObservable<TKey>>();
            // Combined output from all calls
            var combined = sourceFactory.CombineSubscriptions(calls, aggregate, seed, keyComparer);

            return key => Observable.Create<TElement>(obs =>
            {
                // 1. Subscribe the observer to the combined output
                var subToSub = combined.Subscribe(obs);
                // 2. Create an observable that will represent the call and track termination of subscriptions in "d"
                var d = new CancellationDisposable();
                //// function-call can only generate exactly one key, and then wait for unsubscription by observer
                var keyObs = Observable.Return(key).Concat(d.Token.ToObservableCompletion<TKey>());
                // 3. Inject that "call" into combined
                calls.OnNext(keyObs);
                // 4. Return disposable that will end keyObs and unsubscribe obs from combined
                return new CompositeDisposable(subToSub, d);
            });
        }

        /// <summary>
        /// Combine the latest values from individual sequences in <paramref name="sources"/> by calling <paramref name="combine"/>
        /// </summary> 
        public static IObservable<TResult> CombineLatest<T, TResult>(this IObservable<IObservable<T>> sources,
            Func<IEnumerable<T>, TResult> combine)
        {
            var latest = new Dictionary<int, T>(); // Keep existence and latest state
            var final = new Subject<int>(); // Signal end of indivisual source
            return
                // Ensure we can detect end of individual sources (and distinguish their values)
                sources.Select((src, i) => src.Select(t => new {i, t}).Finally(() => final.OnNext(i)))
                    // Utilize the observable guarantee on non-concurrent OnNext to avoid races 
                    .Merge()
                    // Also combine when a source ends. Note that ordering will still be preserved wrt. each source
                    .Merge(final.Select(i => new {i = -i, t = default(T)}))
                    // Update our state and emit new combination
                    .Select(x =>
                    {
                        if (x.i >= 0)
                            latest[x.i] = x.t;
                        if (x.i < 0)
                            latest.Remove(-x.i);
                        return combine(latest.Values);
                    });
        }

        /// <summary>
        /// Aggregate the latest state across <paramref name="sources"/> using the <paramref name="aggregate"/> starting with <paramref name="seed"/>
        /// </summary>
        public static IObservable<TAgg> AggregateLatest<T, TAgg>(
            this IObservable<IObservable<T>> sources,
            Func<TAgg, T, TAgg> aggregate,
            TAgg seed = default(TAgg))
            => sources.CombineLatest(current => current.Aggregate(seed, aggregate));

        /// <summary>
        /// Keep an <paramref name="aggregate"/> subscription latest key in all <paramref name="keySequences"/> via <paramref name="sourceFactory"/>
        /// </summary>
        /// <remarks>Will call <paramref name="sourceFactory"/> exactly when the <paramref name="aggregate"/> changes (as decided by <paramref name="keyComparer"/>) </remarks>
        /// <returns></returns>
        public static IObservable<TElement> CombineSubscriptions<TKey, TAgg, TElement>(
            this Func<TAgg, IObservable<TElement>> sourceFactory,
            IObservable<IObservable<TKey>> keySequences,
            Func<TAgg, TKey, TAgg> aggregate,
            TAgg seed = default(TAgg),
            IEqualityComparer<TAgg> keyComparer = null)
        {
            if (keyComparer == null)
                keyComparer = EqualityComparer<TAgg>.Default;
            return keySequences.AggregateLatest(aggregate, seed)
                .DistinctUntilChanged(keyComparer)
                .Select(sourceFactory)
                .Switch()
                .Publish().RefCount();
        }

    }
}