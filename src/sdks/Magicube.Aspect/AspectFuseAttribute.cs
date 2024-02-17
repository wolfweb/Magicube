using AspectCore.DynamicProxy;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Aspect {
    [AttributeUsage(AttributeTargets.Method)]
    public class AspectFuseAttribute : AbstractInterceptorAttribute {
        private bool circuitBreakerAble = false;

        /// <summary>
        /// 最多重试几次，如果为0则不重试
        /// </summary>
        public int    MaxRetryTimes                   { get; set; } = 0;

        /// <summary>
        /// 熔断多长时间（毫秒）
        /// </summary>
        public int    MillisecondsOfBreak             { get; set; } = 5000;

        /// <summary>
        /// 执行超过多少毫秒则认为超时（0表示不检测超时）
        /// </summary>
        public int    TimeOutMilliseconds             { get; set; } = 0;

        /// <summary>
        /// 缓存多少毫秒（0表示不缓存）
        /// </summary>
        public int    CacheTTLMilliseconds            { get; set; } = 0;

        /// <summary>
        /// 是否启用熔断
        /// </summary>
        public bool   EnableCircuitBreaker            { 
            get {
                return circuitBreakerAble;
            } set {
                circuitBreakerAble = value;
                if (value) CacheTTLMilliseconds = 5000;
            } 
        }

        /// <summary>
        /// 重试间隔的毫秒数
        /// </summary>
        public int    RetryIntervalMilliseconds       { get; set; } = 100;

        /// <summary>
        /// 熔断前重试次数
        /// </summary>
        public int    BeforeBreakingRetryCount        { get; set; } = 3;

        public string FallBackMethod                  { get; set; }

        private static ConcurrentDictionary<MethodInfo, IAsyncPolicy> policies = new ConcurrentDictionary<MethodInfo, IAsyncPolicy>();
        private static IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public AspectFuseAttribute(string fallBackMethod) {
            FallBackMethod = fallBackMethod;
        }

        public override async Task Invoke(AspectContext context, AspectDelegate next) {
            policies.TryGetValue(context.ServiceMethod, out IAsyncPolicy policy);
            await semaphoreSlim.WaitAsync();

            try {
                if (policy == null) {
                    policy = Policy.NoOpAsync();
                    if (EnableCircuitBreaker) {
                        policy = policy.WrapAsync(Policy
                            .Handle<Exception>()
                            .CircuitBreakerAsync(BeforeBreakingRetryCount, TimeSpan.FromMilliseconds(MillisecondsOfBreak))
                            );
                    }
                    if (TimeOutMilliseconds > 0) {
                        policy = policy.WrapAsync(Policy.TimeoutAsync(() => TimeSpan.FromMilliseconds(TimeOutMilliseconds), Polly.Timeout.TimeoutStrategy.Pessimistic));
                    }
                    if (MaxRetryTimes > 0) {
                        policy = policy.WrapAsync(Policy.Handle<Exception>().WaitAndRetryAsync(MaxRetryTimes, i => TimeSpan.FromMilliseconds(RetryIntervalMilliseconds)));
                    }
                    var policyFallBack = Policy
                    .Handle<Exception>()
                    .FallbackAsync(async (ctx, t) => {
                        var aspectContext = (AspectContext)ctx["aspectCtx"];
                        var fallBackMethod = context.ServiceMethod.DeclaringType.GetMethod(FallBackMethod);
                        var fallBackResult = fallBackMethod.Invoke(context.Implementation, context.Parameters);
                        aspectContext.ReturnValue = fallBackResult;
                        await Task.CompletedTask;
                    }, async (ex, t) => {
                        await Task.CompletedTask;
                    });

                    policy = policyFallBack.WrapAsync(policy);
                    policies.TryAdd(context.ServiceMethod, policy);
                }

                Context pollyCtx = new Context {
                    ["aspectCtx"] = context
                };

                if (CacheTTLMilliseconds > 0) {
                    string cacheKey = "MethodCacheManager_Key_" + context.ServiceMethod.DeclaringType + "." + context.ServiceMethod + string.Join("_", context.Parameters);
                    
                    context.ReturnValue = memoryCache.GetOrCreate(cacheKey, entry => {
                        policy.ExecuteAsync(ctx => next(context), pollyCtx).ConfigureAwait(false).GetAwaiter().GetResult();
                        entry.Value              = context.ReturnValue;
                        entry.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMilliseconds(CacheTTLMilliseconds);
                        return entry.Value;
                    });
                } else {
                    await policy.ExecuteAsync(ctx => next(context), pollyCtx);
                }
            } finally {
                semaphoreSlim.Release();
            }            
        }
    }
}
