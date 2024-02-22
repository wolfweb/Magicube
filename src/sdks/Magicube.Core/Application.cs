using Magicube.Core.Modular;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Magicube.Core {
    public class Application {
        public string Root            { get; }
        public string Name            { get; }
        public bool   IsDevelop       { get; }
        public string RunFramework    { get; }
        public string ThemeRootPath   { get; set; }
        public string ModularRootPath { get; set; }
        public string EnvironmentName { get; set; }
        
        public Application(IHostEnvironment environment) {
            Name            = environment.ApplicationName;
            Root            = environment.ContentRootPath;
            IsDevelop       = environment.IsDevelopment();
            EnvironmentName = environment.EnvironmentName;
            RunFramework    = RuntimeInformation.FrameworkDescription;
        }

        public IServiceScope CreateScope() => ServiceProvider.CreateScope();

        public IEnumerable<ModularInfo> Modulars        { get; set; }

        public IServiceProvider         ServiceProvider { get; set; }
    }
}