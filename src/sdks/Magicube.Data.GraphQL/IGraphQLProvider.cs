using GraphQL;
using GraphQL.Instrumentation;
using GraphQL.Types;
using GraphQL.NewtonsoftJson;
using System.Threading.Tasks;
using Magicube.Core;
using System.Collections.Generic;
using GraphQL.Execution;
using System.Linq;

namespace Magicube.Data.GraphQL {
    public interface IGraphQLProvider {
        Task<string> ExecuteAsync(string query, GraphQLArguementBuilder args = null);
        Task<T> ExecuteAsync<T>(string query, GraphQLArguementBuilder args = null);
    }

    public class GraphQLProvider : IGraphQLProvider {
        private readonly IEnumerable<ISchemaBuilder> _schemaBuilders;
        private readonly IDocumentExecuter _executer;
        private readonly Application _application;
        private readonly ISchema _schema;
        public GraphQLProvider(IDocumentExecuter executer, IEnumerable<ISchemaBuilder> schemaBuilders, Application application) {
            _executer       = executer;
            _application    = application;
            _schemaBuilders = schemaBuilders;

            _schema = new Schema {
                Query        = new ObjectGraphType { Name = "Query" },
                Mutation     = new ObjectGraphType { Name = "Mutation" },
                Subscription = new ObjectGraphType { Name = "Subscription" }
            };

            Intialize();
        }

        public async Task<string> ExecuteAsync(string query, GraphQLArguementBuilder args = null) {
            return await _schema.ExecuteAsync(_ => { 
                _.Query       = query;
                _.UserContext = new GraphQLContext {
                    ServiceProvider = _application.ServiceProvider
                };
                if (args != null) {
                    _.Variables = args;
                }                
            });
        }

        public async Task<T> ExecuteAsync<T>(string query, GraphQLArguementBuilder args = null) {
            var option = new ExecutionOptions {
                Query       = query,
                Schema      = _schema,
                UserContext = new GraphQLContext {
                    ServiceProvider = _application.ServiceProvider
                },
            };

            if (args != null) {
                option.Variables = args;
            }

            var result = await _executer.ExecuteAsync(option);

            if (result.Data == null) return default;

            if(result.Data is ExecutionNode node) {
                return ParseData<T>(node);
            } else {
                return (T)result.Data;
            }
        }

        private T ParseData<T>(ExecutionNode node) {
            if (node.Result != null) return (T)node.Result;
            if (node is ValueExecutionNode valueExecutionNode) {
                return (T)valueExecutionNode.ToValue();
            } else if (node is ObjectExecutionNode objectExecutionNode) {
                if (objectExecutionNode.SubFields == null) return default;
                foreach (var childNode in objectExecutionNode.SubFields) {
                   return ParseData<T>(childNode);
                }
            } else if (node is ArrayExecutionNode arrayExecutionNode) {
                return (T)arrayExecutionNode.Result;
            } else {
                return default;
            } 

            return default;
        }

        private void Intialize() {
            _schema.FieldMiddleware.Use(new DynamicEntityFieldMiddleware());
            foreach(var builder in _schemaBuilders) {
                builder.BuildAsync(_schema).ConfigureAwait(false).GetAwaiter().GetResult();
            }

            if (!_schema.Query.Fields.Any()) {
                _schema.Query = null;
            }

            if (!_schema.Mutation.Fields.Any()) {
                _schema.Mutation = null;
            }

            if (!_schema.Subscription.Fields.Any()) {
                _schema.Subscription = null;
            }

            _schema.Initialize();
        }
    }
}
