using Esprima;
using Jint;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace Magicube.Executeflow.Scripting {
    public class JavaScriptScope : IScriptingScope {
        private readonly IMemoryCache _memoryCache;
        public JavaScriptScope(Engine engine, IMemoryCache memoryCache ,IServiceProvider serviceProvider) {
            Engine          = engine;
            _memoryCache    = memoryCache;
            ServiceProvider = serviceProvider;
        }

        public object GetVariable(string name) => Engine.GetValue(name).ToObject();

        public void LoadVariables(IDictionary<string, object> variables) {
            foreach (var variable in variables) {
                Engine.SetValue(variable.Key, variable.Value);
            }
        }

        public void LoadVariable(string name,  object value) {
            Engine.SetValue(name, value);            
        }

        public object Execute(IExecuteflowExpression expression) {
            var parsedAst = _memoryCache.GetOrCreate(expression.Expression, entry => {
                var parser = new JavaScriptParser();
                return parser.ParseScript(expression.Expression);
            });

            var result = Engine.Evaluate(parsedAst)?.ToObject();
            return result;
        }

        public Engine           Engine          { get; }

        public IServiceProvider ServiceProvider { get; }
    }
}
