using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace Scm.Rx
{
    public static class CombinedSubscriptionExtensions
    {

        /// <summary>
        /// Create a function that will generate an <paramref name="aggregate"/> key from all invocations 
        /// and do a switch to a new subscription on <paramref name="sourceFactory"/>(<paramref name="aggregate"/>(keys)).
        /// 
        /// 
        /// </summary>
        /// <remarks>The new subscription is activated (just) before unsubscription to the old.
        /// 
        /// This is useful for ordering a union of stuff from an expensive source. Each observer places his request and the aggregate is requested and distributed</remarks>
        /// <typeparam name="TKey">Key type for combination of subscriptions</typeparam>
        /// <typeparam name="TElement"><see cref="IObservable{T}"/></typeparam>
        /// <param name="sourceFactory">Factory that <see cref="CombinedSubscriptions{TKey,TElement}"/> can invoke to get a new observable when a new <paramref name="aggregate"/> key is seen</param>
        /// <param name="aggregate">Aggregator applied to all the keys currently having their results subscribed  </param>
        /// <param name="keyComparer">Comparer used to decide when <typeparamref name="TKey"/> are equal. Defaults to <see cref="EqualityComparer{T}"/></param>
        /// <param name="observed">Storage to track all current subscriptions. Defaults to a <see cref="HashSet{T}"/></param>
        public static Func<TKey, IObservable<TElement>> CombinedSubscriptions<TKey, TElement>(
            this Func<TKey, IObservable<TElement>> sourceFactory,
            Func<TKey, TKey, TKey> aggregate,
            IEqualityComparer<TKey> keyComparer = null,
            IDictionary<long, TKey> observed = null)
        {
            if (keyComparer == null)
                keyComparer = EqualityComparer<TKey>.Default;
            if (observed == null)
                observed = new Dictionary<long, TKey>();
            var lck = (observed as ICollection)?.SyncRoot ?? observed;
            IDisposable[] currentSubscription = {default(IDisposable)};
            var currentKey = default(TKey);
            var nextId = 0L;


            var sub = new Subject<TElement>();

            void NewSubscriptionIfRequired(TKey nextKey)
            {
                // New requirements for subscription?
                // ReSharper disable once InvertIf -- Much messier
                if (currentSubscription[0] == null || !keyComparer.Equals(currentKey, nextKey))
                    try
                    {
                        currentSubscription[0]?.Dispose();
                    }
                    finally
                    {
                        currentSubscription[0] = sourceFactory(nextKey).SubscribeSafe(sub);
                        currentKey = nextKey;
                    }
            }

            void HandleUnsubscription(long id)
            {
                lock (lck)
                {
                    if (!observed.Remove(id))
                        return;
                    if (observed.Count == 0)
                    {
                        currentSubscription[0]?.Dispose();
                        currentSubscription[0] = null;
                    } else 
                        NewSubscriptionIfRequired(observed.Values.Aggregate(observed.Values.First(), aggregate));
                }
            }

            return key => Observable.Create<TElement>(obs =>
            {
                IDisposable unsub;
                lock (lck)
                {
                    var id = Interlocked.Increment(ref nextId);
                    unsub = Disposable.Create(() => HandleUnsubscription(id));
                    NewSubscriptionIfRequired(observed.Values.Aggregate(key, aggregate));
                    observed.Add(id, key);
                }

                return new CompositeDisposable(unsub, sub.Subscribe(obs));
            });
        }
    }
}