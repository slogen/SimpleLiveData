using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Scm.Concurrency
{
    public static class CancellationScope
    {
        /// <summary>
        /// Scope for constant-scope
        /// </summary>
        private sealed class CancellationTokenScope : ICancellationScope
        {
            public CancellationToken Token { get; }

            public CancellationTokenScope(CancellationToken token)
            {
                Token = token;
            }

            public void Dispose()
            {
                // no-op
            }
        }

        private static ICancellationScope NoCancellationScope { get; } =
            new CancellationTokenScope(CancellationToken.None);

        private static ICancellationScope CancelledScope { get; } =
            new CancellationTokenScope(new CancellationToken(true));

        public static ICancellationScope None() => NoCancellationScope;

        public static ICancellationScope Cancelled() => CancelledScope;

        public static ICancellationScope ToScope(this CancellationToken cancellationToken) =>
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken).ToScope();

        public static ICancellationScope ToScope(this CancellationTokenSource cancellationTokenSource) =>
            new CancellationSourceScope(cancellationTokenSource);

        public static ICancellationScope Link(this IEnumerable<CancellationToken> cancellationTokens)
        {
            var tokens = cancellationTokens.Where(x => x.CanBeCanceled).ToArray();
            return tokens.Length >= 1 ? CancellationTokenSource.CreateLinkedTokenSource(tokens).ToScope()
                : None();
        }

        public static ICancellationScope Link(this CancellationToken token,
            params CancellationToken[] additionalCancellationTokens)
        {
            return !additionalCancellationTokens.Any() ? token.ToScope()
                : token.CanBeCanceled ? new[] {token}.Concat(additionalCancellationTokens).Link()
                : additionalCancellationTokens.Link();
        }

        public static ICancellationScope Timeout(this CancellationToken cancellationToken, TimeSpan timeSpan)
        {
            var cts = new CancellationTokenSource(timeSpan);
            return cancellationToken.CanBeCanceled ? cancellationToken.Link(cts.Token) : cts.ToScope();
        }
    }
}