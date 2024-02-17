using Magicube.Executeflow.Entities;

namespace Magicube.Executeflow {
    public class ActivityContext {
		public string         Outcome  { get; set; }
		public ActivityStore Entity   { get; set; }
		public IActivity      Activity { get; set; }
	}
}
