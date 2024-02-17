using Microsoft.Extensions.DependencyInjection;

namespace Magicube.Core.Modular {
    public interface IModularStartup {
        int  Order { get; }

        void ConfigureServices(IServiceCollection services);
    }
}
