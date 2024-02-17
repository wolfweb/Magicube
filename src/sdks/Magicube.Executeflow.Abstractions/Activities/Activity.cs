using Magicube.Core;
using Magicube.Core.Models;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow {
    public abstract class Activity : IActivity {
		protected IStringLocalizer L { get; }

		public Activity(IStringLocalizer localizer) {
			Name = GetType().Name;
			L    = localizer; 
		}
		public TransferContext               Properties  { get; set; } = new TransferContext();

		public string                        Id          { get; } = Guid.NewGuid().ToString("n");

		public string                        Name        { get => GetProperty<string>(); set => SetProperty(value); }
														 
		public string                        Title       { get => GetProperty<string>(); set => SetProperty(value); }

		public string                        Description { get => GetProperty<string>(); set => SetProperty(value); }

		public abstract string               Category    { get; }

		public abstract IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default);

        public virtual  Task<bool>           CanExecuteAsync(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return Task.FromResult(CanExecute(context, cancellationToken));
        }

		public virtual bool CanExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return true;
		}

		public virtual async Task<ActivityExecutionResult> ExecuteAsync(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return await OnExecute(context, cancellationToken);
		}

		public virtual async Task<ActivityExecutionResult> ResumeAsync(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return await Noop();
		}

		public virtual Task OnActivityExecutingAsync(ExecuteflowContext context, ActivityContext activityContext, CancellationToken cancellationToken = default(CancellationToken)) {
			return Task.CompletedTask;
		}

		public virtual Task OnActivityExecutedAsync(ExecuteflowContext context, ActivityContext activityContext) {
			return Task.CompletedTask;
		}

		protected virtual ValueTask<NoopExecutionResult>   Noop() => new ValueTask<NoopExecutionResult>(new NoopExecutionResult());

		protected IEnumerable<Outcome> BuildOutcomes(params string[] names) {
			return names.Select(x => new Outcome(x));
		}

		protected ActivityExecutionResult Outcomes(params string[] names) {
			return new OutcomeExecutionResult(names);
		}

		protected virtual T GetProperty<T>(Func<T> defaultValue = null, [CallerMemberName] string name = null) {
			if (Properties.ContainsKey(name)) return Properties.TryGet(name, v => {
				if (v == null) return default;

				if(typeof(T).IsSimpleType() && v.GetType().IsSimpleType()) {
					return new ValueObject(v).ConvertTo<T>();
                }

				var str = v.ToString();
				if (str.IsJson()) {
					return Json.Parse<T>(str);
				}
				return (T)v;
			});

			if (defaultValue != null) return defaultValue();

			return default;
		}

		protected virtual void SetProperty(object value, [CallerMemberName] string name = null) {
			if (Properties.ContainsKey(name)) {
				Properties[name] = value;
			} else {
				Properties.TryAdd(name, value);
			}
		}
		
		protected virtual async Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
			return await Noop();
		}
	}
}
