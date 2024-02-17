using Polly;
using Polly.Retry;
using System;
using System.Threading.Tasks;

namespace Magicube.RetryTask {
    public class RetryContext {
        public int       Times     { get; internal set; }
        public int       Count     { get; internal set; }
        public Exception LastError { get; internal set; }
        
        public override string ToString() {
            return $"Times=>{Times}, Count=>{Count}, Error=>{LastError.Message}";
        }
    }

    public class RetryTaskBuilder<T> where T : RetryContext {
        private readonly Action<T> _action;
        private readonly int _retryCount;
        private readonly int _waitSecond;

        public RetryTaskBuilder(Action<T> action, int retryCount = 3, int waitSecond = 5) {
            _action     = action;
            _retryCount = retryCount;
            _waitSecond = waitSecond;
        }

        public RetryPolicy Build() {
            return Policy.Handle<Exception>().WaitAndRetry(_retryCount, i => TimeSpan.FromSeconds(i * _waitSecond), (ex, time, i, ctx) => {
                var retryContext = (T)ctx[RetryPolicyExtension.RetryContextDataKey];
                retryContext.Times = i;
                retryContext.Count = _retryCount;
                retryContext.LastError = ex;
                _action(retryContext);
            });
        }

        public RetryPolicy Build<TException>() where TException : Exception {
            return Policy.Handle<TException>().WaitAndRetry(_retryCount, i => TimeSpan.FromSeconds(i * _waitSecond), (ex, time, i, ctx) => {
                var retryContext = (T)ctx[RetryPolicyExtension.RetryContextDataKey];
                retryContext.Times = i;
                retryContext.Count = _retryCount;
                retryContext.LastError = ex;
                _action(retryContext);
            });
        }

        public AsyncRetryPolicy BuildAsync() {
            return Policy.Handle<Exception>().WaitAndRetryAsync(_retryCount, i => TimeSpan.FromSeconds(i * _waitSecond), (ex, time, i, ctx) => {
                var retryContext = (T)ctx[RetryPolicyExtension.RetryContextDataKey];
                retryContext.Times = i;
                retryContext.Count = _retryCount;
                retryContext.LastError = ex;
                _action(retryContext);
            });
        }

        public AsyncRetryPolicy BuildAsync<TException>() where TException : Exception {
            return Policy.Handle<TException>().WaitAndRetryAsync(_retryCount, i => TimeSpan.FromSeconds(i * _waitSecond), (ex, time, i, ctx) => {
                var retryContext = (T)ctx[RetryPolicyExtension.RetryContextDataKey];
                retryContext.Times = i;
                retryContext.Count = _retryCount;
                retryContext.LastError = ex;
                _action(retryContext);
            });
        }
    }

    public static class RetryPolicyExtension {
        public const string RetryContextDataKey = "dataContext";

        public static void Execute<T>(this RetryPolicy policy, Action<T> action, T t) where T : RetryContext {
            policy.Execute((context) => {
                var ctx = (T)context[RetryContextDataKey];
                action(ctx);
            }, new Context { { RetryContextDataKey, t } });
        }

        public static TResult Execute<T, TResult>(this RetryPolicy policy, Func<T, TResult> action, T t) {
            return policy.Execute((context) =>
            {
                var ctx = (T)context[RetryContextDataKey];
                return action(ctx);
            }, new Context { { RetryContextDataKey, t } });
        }

        public static Task ExecuteAsync<T>(this AsyncRetryPolicy policy, Func<T, Task> action, T t) {
            return policy.ExecuteAsync(async (context) =>
            {
                var ctx = (T)context[RetryContextDataKey];
                await action(ctx);
            }, new Context { { RetryContextDataKey, t } });
        }

        public static Task<TResult> ExecuteAsync<T, TResult>(this AsyncRetryPolicy policy, Func<T, Task<TResult>> action, T t) {
            return policy.ExecuteAsync(async (context) =>
            {
                var ctx = (T)context[RetryContextDataKey];
                return await action(ctx);
            }, new Context { { RetryContextDataKey, t } });
        }
    }
}
