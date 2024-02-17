using Magicube.Core.IO;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicube.IO {
    public static class ServiceCollectionExtension {
        public static IServiceCollection AddIO(this IServiceCollection services) {
            services.AddTransient<IBatchReader, FileBatchReader>();

            return services;
        }
    }
}
