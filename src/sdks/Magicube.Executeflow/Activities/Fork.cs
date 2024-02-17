using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Magicube.Executeflow.Activities {
    public class Fork : Activity {
        public Fork(IStringLocalizer localizer) : base(localizer) {
        }

        public override string Category => L["ControlFlow"];

        public IList<string> Forks {
            get => GetProperty(() => new List<string>());
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default) {
            return Forks.SelectMany(x => BuildOutcomes(L[x]));
        }

        protected override Task<ActivityExecutionResult> OnExecute(ExecuteflowContext context, CancellationToken cancellationToken = default) {
            throw new NotImplementedException();
        }
    }
}
