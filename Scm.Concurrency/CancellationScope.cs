using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Scm.Concurrency
{
    public static class CancellationScope
    {
        private static readonly ICancellationScope NoCancellationScope =
            new CancellationTokenScope(CancellationToken.None);

        private static readonly ICancellationScope CancelledScope = MakeCancelled();
        public static ICancellationScope None() => NoCancellationScope;

        private static ICancellationScope MakeCancelled()
        {
            var cts = new CancellationTokenSource();
            cts.Cancel();
            return new CancellationTokenScope(cts.Token);
        }

        public static ICancellationScope Cancelled() => CancelledScope;

        public static ICancellationScope ToScope(this CancellationToken cancellationToken) =>
            new CancellationTokenScope(cancellationToken);

        public static ICancellationScope ToScope(this CancellationTokenSource cancellationTokenSource) =>
            new CancellationSourceScope(cancellationTokenSource);

        public static ICancellationScope Link(this IEnumerable<CancellationToken> cancellationTokens)
        {
            var tokens = cancellationTokens.Where(x => x.CanBeCanceled).ToArray();
            return tokens.Length > 1 ? CancellationTokenSource.CreateLinkedTokenSource(tokens).ToScope()
                : tokens.Length == 1 ? tokens[0].ToScope()
                : None();
        }

        public static ICancellationScope Link(this CancellationToken token,
            params CancellationToken[] additionalCancellationTokens)
        {
            return !additionalCancellationTokens.Any() ? token.ToScope()
                : token.CanBeCanceled ? new[] {token}.Concat(additionalCancellationTokens).Link()
                : additionalCancellationTokens.Link();
        }
    }
}