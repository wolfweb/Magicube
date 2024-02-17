using Magicube.Core;
using Magicube.Executeflow.Entities;
using Magicube.Executeflow.Scripting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Executeflow {
    public class ExecuteflowContext {
		private readonly Stack<ActivityContext> Scheduled;
		private readonly IActivityProvider _activityProvider;

		public IDictionary<long, ActivityContext> Activities { get; }

		public object                             LastResult { get; set; }
					                          
		public Executeflow                        Execflow   { get; }
	
		public IDictionary<string, object>        Input      { get; }

		public ExecuteflowStatus                  Status     {
			get => Execflow.Status;
			set => Execflow.Status = value;
		}

		public ActivityContext                    CurrentActivityContext { get; set; }

		public ExecutionContextScope              ContextScope { get; }

		public ExecuteflowContext(
			IDictionary<string, object> input, 
			ExecuteflowStore execflow, 
			IActivityProvider activityProvider, 
			ActivityContext blockActivityContext = null
			) {
			_activityProvider = activityProvider;

			Input = input;
			Execflow = new Executeflow {
				Status		= ExecuteflowStatus.Idle,
				Connections = execflow.Transitions
			};
			Activities = execflow.Activities?.Select(x =>
			{
				var activity = new ActivityContext {
					Entity = x,
				};
				return new {
					Key = x.Id,
					Value = activity
				};
			})
			.ToDictionary(x => x.Key, x => x.Value);

			Scheduled = new Stack<ActivityContext>();

			CurrentActivityContext = blockActivityContext == null ? Activities?.FirstOrDefault().Value : blockActivityContext;
			Scheduled.Push(Initialize(CurrentActivityContext));
			ContextScope = new ExecutionContextScope();
		}

		public void Fault(Exception exception) {
			Execflow.Status       = ExecuteflowStatus.Faulted;
			Execflow.FaultMessage = exception.Message;
		}

		public void Suspend() {
			Execflow.Status = ExecuteflowStatus.Suspended;
        }

		public ActivityContext Pop() {
			if (Scheduled.Count > 0) return CurrentActivityContext = Scheduled.Pop();
			return null;
		}

		public void ScheduleActivity(Transition conn) {
			Scheduled.Push(GetActivity(conn.DestinationActivityId));			
		}

		private ActivityContext GetActivity(long activityId) {
			return Initialize( Activities[activityId]);
		}

		private ActivityContext Initialize(ActivityContext activity) {
			if (activity == null) return null;
			activity.Activity = _activityProvider.Retrieve(activity.Entity.Activity);
			activity.Activity.Properties = activity.Entity.Properties;
			return activity;
		}

        public sealed class ExecutionContextScope {
			private readonly ConcurrentDictionary<string, ConcurrentQueue<IVariableExpression>> _activitiesVariables;
			private const string RootBlockKey = "_root";
			public ExecutionContextScope() {
				_activitiesVariables = new ConcurrentDictionary<string, ConcurrentQueue<IVariableExpression>>();
			}

			public ExecutionContextScope Push(IActivity activity, IVariableExpression expression) {
				var activityId = activity?.Id ;
				if (activity == null) {
					activityId = RootBlockKey;
				}

				if (!activity.GetType().IsInherited<IActivityBlock>()) {
					if (_activitiesVariables.Any()) {
						var region = _activitiesVariables.Last();
						if (expression != null && region.Value.All(x => x.Name != expression.Name)) {
							region.Value.Enqueue(expression);
						}
					}
				}				

				_activitiesVariables.AddOrUpdate(activityId, key => {
					var v = new ConcurrentQueue<IVariableExpression>();
					if (expression != null) v.Enqueue(expression);
					return v;
				}, (k, v) => {
					if (expression != null && v.All(x => x.Name != expression.Name)) {
						v.Enqueue(expression);
					}
					return v;
				});

				return this;
			}

			public ExecutionContextScope LoadAllVariables(IScriptingScope scope) {
				_activitiesVariables.SelectMany(x => x.Value).ForEach(x => {
					scope.LoadVariable(x.Name, x.Value);
				});

				return this;
            }

			public ExecutionContextScope Refresh(IScriptingScope scope) {
				_activitiesVariables.SelectMany(x => x.Value).ForEach(x => {
					x.Value = scope.GetVariable(x.Name);
				});

				return this;
            }

			public ExecutionContextScope Done(ExecuteflowContext context) {
				_activitiesVariables.TryRemove(context.CurrentActivityContext.Activity.Id, out _);
				return this;
			}
		}
    }
}
