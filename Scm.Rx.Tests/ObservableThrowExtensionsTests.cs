using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace Scm.Rx.Tests
{
    public class ObservableThrowExtensionsTests
    {
        protected CancellationToken CancellationToken => CancellationToken.None;

        protected Exception ThrowIfCalled(int x) => new NotSupportedException("Unexpected");

        [Fact]
        public void ThrowWillNotThrowOnUnsubscription()
        {
            Observable.Never<int>().Awaiting(async nvr =>
                    await nvr.Throw(ThrowIfCalled)
                        .Timeout(TimeSpan.Zero, Observable.Empty<int>())
                        .LastOrDefaultAsync()
                        .ToTask(CancellationToken).ConfigureAwait(false))
                .Should().NotThrow();
        }

        [Fact]
        public void ThrowWillThrowOnCompletion()
        {
            const string expectedMessage = "expected msg";
            new int[0].Awaiting(async xs =>
                    await xs.ToObservable().Throw(x =>
                    {
                        x.Should().Be(default(int));
                        return new NotSupportedException(expectedMessage);
                    }).ToTask(CancellationToken).ConfigureAwait(false))
                .Should().Throw<NotSupportedException>().WithMessage(expectedMessage);
        }

        [Fact]
        public void ThrowWillThrowOnItemReceived()
        {
            const string expectedMessage = "expected msg";
            new[] {1}.Awaiting(async xs =>
                    await xs.ToObservable().Throw(x =>
                    {
                        x.Should().Be(1);
                        return new NotSupportedException(expectedMessage);
                    }).ToTask(CancellationToken).ConfigureAwait(false))
                .Should().Throw<NotSupportedException>().WithMessage(expectedMessage);
        }

        ///// <summary>
        ///// Throws <paramref name="throwFunc"/>(firstOrDefault(<paramref name="source"/>))
        ///// </summary>
        //public static IObservable<T> Throw<T, TException>(this IObservable<T> source, Func<T, TException> throwFunc)
        //    where TException : Exception
        //    => source.FirstOrDefaultAsync().Select<T, T>(x => throw throwFunc(x));
    }
}