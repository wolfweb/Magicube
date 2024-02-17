using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Threading;

namespace Magicube.Executeflow.Activities {
    public class Join : Activity {
        public enum JoinMode {
            WaitAll,
            WaitAny
        }

        public Join(IStringLocalizer localizer) : base(localizer) {
        }

        public override string Category => L["ControlFlow"];

        public JoinMode Mode {
            get => GetProperty(() => JoinMode.WaitAll);
            set => SetProperty(value);
        }

        private IList<string> Branches {
            get => GetProperty(() => new List<string>());
            set => SetProperty(value);
        }

        public override IEnumerable<Outcome> GetOutcomes(ExecuteflowContext context, CancellationToken cancellationToken = default) {
            return BuildOutcomes(L["Joined"]);
        }


    }
}
