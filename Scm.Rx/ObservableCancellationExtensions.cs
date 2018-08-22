using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Scm.Rx
{
    public static class ObservableCancellationExtensions
    {
        /// <summary>
        /// Generates observable that throws OperationCancelledException when the <paramref name="cancellationToken"/> is cancelled.
        /// 
        /// Observers will never experience <see cref="IObserver{T}.OnNext"/> or <see cref="IObserver{T}.OnCompleted"/>.
        /// 
        /// When <paramref name="cancellationToken"/> is cancelled, observers will get <see cref="IObserver{T}.OnError"/>.
        /// </summary>
        /// <see cref="ToObservable(System.Threading.CancellationToken,string,string,int)"/>
        public static IObservable<Unit> ToObservable(this CancellationToken cancellationToken,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
            => cancellationToken.ToObservable(() =>
                new OperationCanceledException(
                    $"Observable cancellation activated (requested from: {callerMemberName}({callerFilePath}:{callerLineNumber})",
                    cancellationToken));

        /// <summary>
        /// Generates observable that completes when the <paramref name="cancellationToken"/> is cancelled.
        /// 
        /// Observers will never experience <see cref="IObserver{T}.OnNext"/> or <see cref="IObserver{T}.OnError"/>.
        /// 
        /// When <paramref name="cancellationToken"/> is cancelled, observers will get <see cref="IObserver{T}.OnCompleted"/>.
        /// </summary>
        /// <see cref="ToObservable(System.Threading.CancellationToken,string,string,int)"/>
        [SuppressMessage("ReSharper", "ExplicitCallerInfoArgument")]
        public static IObservable<Unit> ToObservableCompletion(this CancellationToken cancellationToken,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
            => cancellationToken.ToObservableCompletion<Unit>(callerMemberName, callerFilePath, callerLineNumber);

        public static IObservable<T> ToObservableCompletion<T>(this CancellationToken cancellationToken,
            [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerFilePath = null,
            [CallerLineNumber] int callerLineNumber = 0)
            => cancellationToken.ToObservable(() => Observable.Empty<T>());

        /// <summary>
        /// Generates observable that throws <paramref name="exceptionFunc"/>() when the <paramref name="cancellationToken"/> is cancelled.
        /// 
        /// Observers will never experience <see cref="IObserver{T}.OnNext"/> or <see cref="IObserver{T}.OnCompleted"/>.
        /// 
        /// When <paramref name="cancellationToken"/> is cancelled, observers will get <see cref="IObserver{T}.OnError"/>.
        /// </summary>
        public static IObservable<Unit> ToObservable(
            this CancellationToken cancellationToken,
            Func<Exception> exceptionFunc)
            => cancellationToken.ToObservable(() => Observable.Throw<Unit>(exceptionFunc?.Invoke() ?? new OperationCanceledException(cancellationToken)));

        public static IObservable<T> ToObservable<T>(this CancellationToken cancellationToken, Func<IObservable<T>> onCancellation)
        {
            return Observable.Create<T>(obs =>
            {
                var d = new MultipleAssignmentDisposable();
                var reg = new IDisposable[1];
                reg[0] = cancellationToken.Register(() =>
                {
                    var o = onCancellation();
                    var sub = o.Subscribe(obs);
                    d.Disposable = new CompositeDisposable(reg[0], sub);
                });
                d.Disposable = reg[0];
                return d;
            });
        }
    }
}