using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Scm.Concurrency.Tests
{
    [SuppressMessage("ReSharper", "ArgumentsStyleLiteral", Justification ="Improved readbility")]
    public class CancellationScopeTests
    {
        protected static void AssertState(CancellationToken cancellationToken, bool canBeCancelled, bool isCancellationRequested)
        {
            cancellationToken
                .Should().BeEquivalentTo(
                    new
                    {
                        CanBeCancelled = canBeCancelled,
                        IsCancellationRequested = isCancellationRequested
                    }, cfg => cfg.ExcludingMissingMembers());
        }
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed", Justification = "Probing for Token to provoke exception")]
        protected static void AssertDisposed(ICancellationScope source)
        {
            Action tryToken = () => { source?.Token.GetHashCode(); };
            tryToken.Should().Throw<ObjectDisposedException>();
        }

        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed", Justification = "Probing for Token to provoke exception")]
        protected static void AssertDisposed(CancellationTokenSource source)
        {
            Action tryToken = () => { source?.Token.GetHashCode(); };
            tryToken.Should().Throw<ObjectDisposedException>();
        }
        [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed", Justification = "Probing for Token to provoke exception")]
        protected static void AssertNotDisposed(CancellationTokenSource source)
        {
            Action tryToken = () => { source?.Token.GetHashCode(); };
            tryToken.Should().NotThrow();
        }

        [Fact]
        public void NoCancellationShouldBehaveAsSpect()
        {
            CancellationToken ct;
            using (var cts = CancellationScope.None())
            {
                ct = cts.Token;
                AssertState(ct, canBeCancelled: false, isCancellationRequested: false);
            }
            AssertState(ct, canBeCancelled: false, isCancellationRequested: false);
        }


        [Fact]
        public void CancelledScopeShouldBehaveAsSpec()
        {
            CancellationToken ct;
            using (var cts = CancellationScope.Cancelled())
            {
                ct = cts.Token;
                AssertState(ct, canBeCancelled: true, isCancellationRequested: true);
            }
            AssertState(ct, canBeCancelled: true, isCancellationRequested: true);
        }
        [Fact]
        public void TokenCancellationScopeShouldBehaveAsSpec()
        {
            using (var source = new CancellationTokenSource())
            {
                CancellationToken ct;
                ICancellationScope cts;
                using (cts = source.Token.ToScope())
                {
                    ct = cts.Token;
                    AssertState(ct, canBeCancelled: true, isCancellationRequested: false);
                    source.Cancel();
                    AssertState(ct, canBeCancelled: true, isCancellationRequested: true);
                }
                AssertState(ct, canBeCancelled: true, isCancellationRequested: true);
                AssertNotDisposed(source);
            }
        }
        [Fact]
        public void SourceCancellationScopeShouldBehaveAsSpec()
        {
            using (var source = new CancellationTokenSource())
            {
                CancellationToken ct;
                ICancellationScope cts;
                using (cts = source.ToScope())
                {
                    ct = cts.Token;
                    AssertState(ct, canBeCancelled: true, isCancellationRequested: false);
                    source.Cancel();
                    AssertState(ct, canBeCancelled: true, isCancellationRequested: true);
                }
                AssertState(ct, canBeCancelled: true, isCancellationRequested: true);
                AssertDisposed(cts);
                AssertDisposed(source);
            }
        }
        [Fact]
        public void Link0TokensProducesNoCancellationSource()
        {
            AssertState(new CancellationToken[0].Link().Token, canBeCancelled: false, isCancellationRequested: false);
        }
        [Fact]
        public void LinkingOnlyNonCancellableTokensProducesNoCancellationScope()
        {
            AssertState(Enumerable.Range(0, 3).Select(x => CancellationToken.None).Link().Token, canBeCancelled: false, isCancellationRequested: false);
        }
        [Fact]
        public void LinkingTokensProducesSharedCancellation()
        {
            const int count = 3;
            for (var i = 0; i < count; ++i)
            {
                var sources = Enumerable.Range(0, count).Select(_ => new CancellationTokenSource()).ToList();
                using (var cts = sources.First().Token.Link(sources.Skip(1).Select(x => x.Token).ToArray()))
                {
                    // ReSharper disable once AccessToDisposedClosure -- awaited
                    Task WaitForCancel() => Task.Delay(Timeout.Infinite, cts.Token);

                    async Task DoCancel()
                    {
                        await Task.Yield();
                        // ReSharper disable once AccessToModifiedClosure -- recreated each invocation
                        sources[i].Cancel();
                    }

                    Func<Task> act = async () => await Task.WhenAll(WaitForCancel(), DoCancel()).ConfigureAwait(false);
                    act.Should().Throw<TaskCanceledException>();
                }
            }
        }
    }
}
