using Magicube.Executeflow.Activities;
using Magicube.Executeflow.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Executeflow.Services {
    public class ActivityProvider: IActivityProvider {
        private readonly ILogger<ActivityProvider> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly ExecuteflowOptions _options;

        private readonly Lazy<IDictionary<string, IActivity>> _keyedActivities;
        public ActivityProvider(IOptions<ExecuteflowOptions> options, IServiceProvider serviceProvider, ILogger<ActivityProvider> logger) {
            _serviceProvider = serviceProvider;
            _options         = options.Value;
            _logger          = logger;

            _keyedActivities = new Lazy<IDictionary<string, IActivity>>(() => _options.Activities.Where(x => !x.IsAbstract).Select(x => (IActivity)ActivatorUtilities.CreateInstance(_serviceProvider, x)).ToDictionary(x => x.Name));
        }

        public IEnumerable<IActivity> Activities     => _keyedActivities.Value.Values;

        public IActivity Retrieve(Type assemblyInfo) => (IActivity)ActivatorUtilities.CreateInstance(_serviceProvider, assemblyInfo);
    }
}
