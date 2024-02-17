using System.Threading.Tasks;

namespace Magicube.Eventbus {
    public static class EventProviderExtension {
        public static Task OnLoadingAsync<T>(this IEventProvider provider, EventContext<T> ctx) where T : class {
            return provider.OnHandlerAsync<T, OnLoading<T>>(ctx);
        }

        public static Task OnLoadedAsync<T>(this IEventProvider provider, EventContext<T> ctx) where T : class {
            return provider.OnHandlerAsync<T, OnLoaded<T>>(ctx);
        }

        public static Task OnUpdatingAsync<T>(this IEventProvider provider, EventContext<T> ctx) where T : class {
            return provider.OnHandlerAsync<T, OnUpdating<T>>(ctx);
        }

        public static Task OnUpdatedAsync<T>(this IEventProvider provider, EventContext<T> ctx) where T : class {
            return provider.OnHandlerAsync<T, OnUpdated<T>>(ctx);
        }

        public static Task OnDeletingAsync<T>(this IEventProvider provider, EventContext<T> ctx) where T : class {
            return provider.OnHandlerAsync<T, OnDeleting<T>>(ctx);
        }

        public static Task OnDeletedAsync<T>(this IEventProvider provider, EventContext<T> ctx) where T : class {
            return provider.OnHandlerAsync<T, OnDeleted<T>>(ctx);
        }

        public static Task OnCreatingAsync<T>(this IEventProvider provider, EventContext<T> ctx) where T : class {
            return provider.OnHandlerAsync<T, OnCreating<T>>(ctx);
        }

        public static Task OnCreatedAsync<T>(this IEventProvider provider, EventContext<T> ctx) where T : class {
            return provider.OnHandlerAsync<T, OnCreated<T>>(ctx);
        }

        public static Task OnPublishingAsync<T>(this IEventProvider provider, EventContext<T> ctx) where T : class {
            return provider.OnHandlerAsync<T, OnPublishing<T>>(ctx);
        }

        public static Task OnPublishedAsync<T>(this IEventProvider provider, EventContext<T> ctx) where T : class {
            return provider.OnHandlerAsync<T, OnPublished<T>>(ctx);
        }
    }
}