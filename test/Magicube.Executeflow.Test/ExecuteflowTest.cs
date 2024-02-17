using Magicube.Core;
using Magicube.Core.Runtime;
using Magicube.Core.Runtime.Attributes;
using Magicube.Executeflow.Activities;
using Magicube.Executeflow.Entities;
using Magicube.Executeflow.Scripting;
using Magicube.Executeflow.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Magicube.Executeflow.Test {
    public class ExecuteflowTest {
        private readonly IServiceProvider ServiceProvider;

        public ExecuteflowTest() {
            ServiceProvider = new ServiceCollection()
				.AddTransient<IFooService, FooService>()
                .AddCore()
				.AddMemoryCache()
				.AddSingleton<IStringLocalizer, NullStringLocalizer> ()
                .AddExecuteflow()
                .BuildServiceProvider();
        }

		[Fact]
		public void Func_Activity_State_Test() {
			var scriptEngine = ServiceProvider.GetService<IScriptingEngine>();
			Assert.NotNull(scriptEngine);

			var setInput = ServiceProvider.Resolve<SetInput>();
			Assert.NotNull(setInput);
			setInput.Variable = new VariableExpression("x", new ExecuteExpression("x = 100"));
			var scope = scriptEngine.CreateScope(Enumerable.Empty<GlobalMethod>());
			var result = setInput.Variable.Execute<object>(null, scope);
			Assert.Equal(100, result.ConvertTo<int>());
			setInput.Variable = new VariableExpression("x", 99);
			result = setInput.Variable.Execute<object>(null, scope);
			Assert.Equal(99, result.ConvertTo<int>());
		}

		[Fact]
		public async Task Func_Executeflow_Demo() {
			var str = Json.Stringify(new ExecuteExpression("x=10"));
			var obj = Json.Parse<IExecuteflowExpression>(str);
			Assert.NotNull(obj);
			Assert.True(obj is ExecuteExpression);

			var ctx = new ExecuteflowContext(null, new ExecuteflowStore(), ServiceProvider.GetService<IActivityProvider>());

			var evaluator = ServiceProvider.GetService<EvaluatorFactory>();
			var (result,_) = await evaluator.Evaluate<string>("", ctx);
			Assert.Equal(default, result);

			(result,_) = await evaluator.Evaluate<string>("literal('Content-Type:application/json')", ctx);
			Assert.Equal("Content-Type:application/json", result);
		}

        [Fact]
        public void Func_Executeflow_Test() {
            var runtimeMetadataProvider = ServiceProvider.GetRequiredService<RuntimeMetadataProvider>();
            var activityProvider        = ServiceProvider.GetService<IActivityProvider>();
            var executeflowService      = ServiceProvider.GetService<ExecuteflowService>();

            Assert.NotNull(runtimeMetadataProvider);
            Assert.NotNull(activityProvider);
            Assert.NotNull(executeflowService);
        }

		[Fact]
        public async Task Func_Executeflow_While_Test() {
			var executeflowService = ServiceProvider.GetService<ExecuteflowService>();
			var activities = new List<ActivityStore> {
				new ActivityStore{
					Id = 1,
					Name = "输入变量",
					Activity = typeof(SetInput),
					Properties = new TransferContext{ 
						["Variable"] = Json.Stringify( new VariableExpression("x", 15) )
					}
				},
				new ActivityStore{
					Id = 2,
					Name = "循环",
					Activity = typeof(While),
					Properties = new TransferContext{
						["Condition"] = Json.Stringify(new ExecuteExpression("x>10"))
					}
				},
				new ActivityStore{
					Id = 3,
					Name = "设置值",
					Activity = typeof(SetProperty),
					Properties = new TransferContext{
						["Value"] = Json.Stringify(new ExecuteExpression("x-=1"))
					}
				},
				new ActivityStore{
					Id = 4,
					Name = "输出",
					Activity = typeof(SetOutput),
					Properties = new TransferContext{
						["Value"] = Json.Stringify(new ExecuteExpression("x"))
					}
				}
			};
	
			var entity = new ExecuteflowStore {
				Id = 1,
				Activities = activities,
				Transitions = new List<Transition>{
					new Transition{
						SourceActivityId      = 1,
						SourceOutcomeName     = OutcomeNames.Next,
						DestinationActivityId = 2,				
					},
					new Transition{
						SourceActivityId      = 2,
						SourceOutcomeName     = OutcomeNames.Iterate,
						DestinationActivityId = 3,
					},
					new Transition{
						SourceActivityId      = 2,
						SourceOutcomeName     = OutcomeNames.Done,
						DestinationActivityId = 4,
					},
					new Transition{
						SourceActivityId      = 3,
						SourceOutcomeName     = OutcomeNames.Done,
						DestinationActivityId = 2,
					},
				}
			};

			var context = await executeflowService.CreateExecutionContextAsync(entity, null);
			await executeflowService.Start(context);

			Assert.Equal(10, int.Parse(context.LastResult.ToString()));
        }

		[Fact]
		public async Task Func_Executeflow_For_Test() {
			var executeflowService = ServiceProvider.GetService<ExecuteflowService>();

			var activities = new List<ActivityStore> {
				new ActivityStore{
					Id = 1,
					Name = "循环",
					Activity = typeof(For),
					Properties = new TransferContext{
						["From"] = Json.Stringify(new LiteralExpression("From","0")),
						["To"]   = Json.Stringify(new LiteralExpression("To","10")),
						["Step"] = Json.Stringify(new LiteralExpression("Step","1"))
					}
				},
				new ActivityStore{
					Id = 2,
					Name = "输出",
					Activity = typeof(SetOutput),
					Properties = new TransferContext{
						["Value"] = Json.Stringify(new ExecuteExpression("x"))
					}
				}
			};

			var entity = new ExecuteflowStore {
				Id = 1,
				Activities = activities,
				Transitions = new List<Transition>{
					new Transition{
						SourceActivityId      = 1,
						SourceOutcomeName     = OutcomeNames.Iterate,
						DestinationActivityId = 1,
					},
					new Transition{
						SourceActivityId      = 1,
						SourceOutcomeName     = OutcomeNames.Done,
						DestinationActivityId = 2,
					},
					new Transition{
						SourceActivityId      = 2,
						SourceOutcomeName     = OutcomeNames.Done,
						DestinationActivityId = 0,
					},
				}
			};

			var context = await executeflowService.CreateExecutionContextAsync(entity, null);
			await executeflowService.Start(context);

			Assert.Equal(10, int.Parse(context.LastResult.ToString()));
		}

		[Fact]
		public async Task Func_Executeflow_Foreach_Test() {

			await Task.CompletedTask;
        }

		[Fact]
		public async Task Func_Executeflow_CallService_Test() {
			var executeflowService = ServiceProvider.GetService<ExecuteflowService>();
			var him = new Person { Name = "wolfweb", Pwd = "123456", CreateAt = DateTime.Now };

			var activities = new List<ActivityStore> {
					new ActivityStore{
						Id = 2,
						Name = "设置值",
						Activity = typeof(SetProperty),
						Properties = new TransferContext{
							["Value"]     = Json.Stringify(new ExecuteExpression("$('him').Pwd=生成Token()"))
						}
					}
				};

			var entity = new ExecuteflowStore {
				Id = 1,
				Activities = activities,
				Transitions = new List<Transition>{
					new Transition{
						SourceActivityId      = 1,
						SourceOutcomeName     = OutcomeNames.Done,
						DestinationActivityId = 2,
					}
				}
			};

			var input   = new TransferContext { ["him"] = him };
			var context = await executeflowService.CreateExecutionContextAsync(entity, input);
			await executeflowService.Start(context);

			Assert.NotNull(context.LastResult);
			Assert.Equal(him.Pwd, context.LastResult.ToString());
		}

		[Fact]
		public async Task Func_Executeflow_If_Test() {
			var executeflowService = ServiceProvider.GetService<ExecuteflowService>();

			var activities = new List<ActivityStore> {
				    new ActivityStore{ 
						Id = 1,
						Name = "输入变量",
						Activity = typeof(SetInput),
						Properties = new TransferContext{
							["Variable"] = Json.Stringify( new VariableExpression("x", 15) )
						}
					},
					new ActivityStore{ 
						Id = 2,
						Name = "判断",
						Activity = typeof(IfElse),
						Properties = new TransferContext {
							["Condition"] = Json.Stringify(new ExecuteExpression("x>10"))
						}
					},
					new ActivityStore{
						Id = 3,
						Name = "设置值",
						Activity = typeof(SetOutput),
						Properties = new TransferContext{
							["OutputName"] = "x",
							["Value"] = Json.Stringify(new ExecuteExpression("x=10"))
						}
					},
					new ActivityStore{
						Id = 4,
						Name = "设置值",
						Activity = typeof(SetOutput),
						Properties = new TransferContext{
							["OutputName"] = "x",
							["Value"] = Json.Stringify(new ExecuteExpression("x=0"))
						}
					}
				};

			var entity = new ExecuteflowStore {
				Id = 1,
				Activities = activities,
				Transitions = new List<Transition>{
					new Transition{
						SourceActivityId      = 1,
						SourceOutcomeName     = OutcomeNames.Next,
						DestinationActivityId = 2,
					},
					new Transition{
						SourceActivityId      = 2,
						SourceOutcomeName     = OutcomeNames.True,
						DestinationActivityId = 3,
					},
					new Transition{
						SourceActivityId      = 2,
						SourceOutcomeName     = OutcomeNames.False,
						DestinationActivityId = 4,
					}
				}
			};

			var input = new TransferContext { ["x"] = 100 };
			var context = await executeflowService.CreateExecutionContextAsync(entity, input);
			await executeflowService.Start(context);

			Assert.NotNull(context.LastResult);
			Assert.True(context.LastResult.ToString() == "10");
		}
	}

	public interface IFooService : IRuntimeMetadata {
		[RuntimeMethod(Title = "生成Token")]
		string Token();

		[RuntimeMethod(Title = "转base64")]
		string ToBase64(string v);

		[RuntimeMethod]
		string FromBase64(string v);
	}

	public class FooService : IFooService {		
		public string Token() => Guid.NewGuid().ToString("N");
		
		public string ToBase64(string v) => Convert.ToBase64String(v.ToByte());

		public string FromBase64(string v) => Convert.FromBase64String(v).ToString("utf-8");
	}

	public class Person {
		public string Name { get; set; }
		public string Pwd { get; set; }
		public DateTime CreateAt { get; set; }
	}

    public class NullStringLocalizer : IStringLocalizer {
        public LocalizedString this[string name] => new LocalizedString(name, name);

        public LocalizedString this[string name, params object[] arguments] => new LocalizedString(name, name);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) {
            throw new NotImplementedException();
        }
    }
}
