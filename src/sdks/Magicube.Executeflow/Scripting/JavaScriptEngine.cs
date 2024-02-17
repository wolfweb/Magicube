using Jint;
using Magicube.Core;
using Magicube.Core.Runtime;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Executeflow.Scripting {
    public class JavaScriptEngine : IScriptingEngine {
        static string[] Regions = { "execute", "literal", "variable" };

        private readonly Dictionary<string, Delegate> _globalMethods = new Dictionary<string, Delegate>();

        private readonly IMemoryCache _memoryCache;
        private readonly IServiceProvider _serviceProvider;

        public bool CanExecute(string region) => Regions.Contains(region);

        public JavaScriptEngine(
            RuntimeMetadataProvider metadataProvider, 
            IMemoryCache memoryCache, 
            IServiceProvider serviceProvider
            ) {
            foreach (var item in metadataProvider.RuntimeMethods) {
                _globalMethods.TryAdd(item.Title, item.ToDelegate(metadataProvider.GetInstance(item)));
            }

            _memoryCache           = memoryCache;
            _serviceProvider       = serviceProvider;
        }

        public IScriptingScope CreateScope(IEnumerable<GlobalMethod> methods) {
            var engine = new Engine();
            foreach (var method in _globalMethods) {
                engine.SetValue(method.Key, method.Value);
            }
            
            foreach(var method in methods) {
                engine.SetValue(method.Name, method.Method(_serviceProvider));
            }

            return new JavaScriptScope(engine, _memoryCache, _serviceProvider);
        }
    }
}
