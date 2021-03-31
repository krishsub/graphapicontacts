using Polly;
using Polly.Wrap;
using System;

namespace Krish.Graph
{
    /// <summary>
    /// Creates resiliency strategies by combining two or more resiliency policies in a chain.
    /// </summary>
    /// <typeparam name="TResult">The fallback value to use when the resiliency strategy fails.</typeparam>
    internal static class PolicyFactory<TResult>
    {
        /// <summary>
        /// Create a default resiliency strategy by combining a Fallback and a Retry policy.
        /// </summary>
        /// <typeparam name="TException">The exception type that triggers the Fallback and Retry policy.</typeparam>
        /// <param name="retryCount">The number of times a retry should execute before attempting a fallback.</param>
        /// <returns>The default value of <typeparamref name="TResult"/></returns>
        public static AsyncPolicyWrap<TResult> CreateAsyncResiliencyPolicy<TException>(int retryCount) where TException : Exception
        {
            var asyncFallbackPolicy = Policy<TResult>
                .Handle<TException>()
                .FallbackAsync(default(TResult));

            var asyncRetryPolicy = Policy<TResult>
                .Handle<TException>()
                .RetryAsync(retryCount);

            return Policy.WrapAsync<TResult>(asyncFallbackPolicy, asyncRetryPolicy);
        }

        /// <summary>
        /// Create a default resiliency strategy by combining a Fallback and a Retry policy with predicates
        /// to evaluate when the Fallback and Retry policies should execute.
        /// </summary>
        /// <typeparam name="TException">The exception type that triggers the Fallback and Retry policy.</typeparam>
        /// <param name="retryCount">>The number of times a retry should execute before attempting a fallback.</param>
        /// <param name="fallbackExceptionPredicate">The exception predicate to evaluate for the Fallback policy to trigger.</param>
        /// <param name="retryExceptionPredicate">The exception predicate to evaluate for the Retry policy to trigger.</param>
        /// <returns></returns>
        public static AsyncPolicyWrap<TResult> CreateAsyncResiliencyPolicy<TException>(
            int retryCount,
            Func<TException, bool> fallbackExceptionPredicate,
            Func<TException, bool> retryExceptionPredicate) where TException : Exception
        {
            var asyncFallbackPolicy = Policy<TResult>
                .Handle<TException>(fallbackExceptionPredicate)
                .FallbackAsync(default(TResult));

            var asyncRetryPolicy = Policy<TResult>
                .Handle<TException>(retryExceptionPredicate)
                .RetryAsync(retryCount);

            return Policy.WrapAsync<TResult>(asyncFallbackPolicy, asyncRetryPolicy);
        }
    }
}
