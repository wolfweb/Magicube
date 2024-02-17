using Magicube.Core;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Magicube.Eventbus {
    public abstract class OnEntityEvent<T> : IEvent<T> where T : class {
        public virtual int Priority => 1;

        public string Name     => GetType().Name;
                               
        public string Display  => $"{Name.ToFriendly()}";

        public string Category => $"{typeof(T).Name}";

        public virtual async Task OnHandlingAsync(EventContext<T> ctx) {
            await Task.CompletedTask;
        }
    }

    public class OnCreating<T>   : OnEntityEvent<T> where T : class { }

    public class OnCreated<T>    : OnEntityEvent<T> where T : class { }

    public class OnDeleting<T>   : OnEntityEvent<T> where T : class { }

    public class OnDeleted<T>    : OnEntityEvent<T> where T : class { }

    public class OnLoading<T>    : OnEntityEvent<T> where T : class { }

    public class OnLoaded<T>     : OnEntityEvent<T> where T : class { }

    public class OnUpdating<T>   : OnEntityEvent<T> where T : class { }

    public class OnUpdated<T>    : OnEntityEvent<T> where T : class { }

    public class OnPublishing<T> : OnEntityEvent<T> where T : class { }

    public class OnPublished<T>  : OnEntityEvent<T> where T : class { }
}
