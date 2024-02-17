﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Magicube.Core {
    public class HostedBackgroundService : ScopedProcessor {
        private readonly ILogger _logger;
        public HostedBackgroundService(ILogger<HostedBackgroundService> logger, IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory) {
            _logger = logger;
        }

        public override Task ProcessInScope(CancellationToken stoppingToken, IServiceProvider serviceProvider) {
            var backgroundTasks =  serviceProvider.GetServices<IBackgroundTask>();
            return backgroundTasks.InvokeAsync(task => task.Process(stoppingToken), _logger);
        }
    }
}
