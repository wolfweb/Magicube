using Magicube.Core;
using Magicube.Executeflow.Entities;
using System.Collections.Generic;

namespace Magicube.Executeflow {
    public class Executeflow {
		public Executeflow() {
			State = new TransferContext();
		}
		public ExecuteflowStatus       Status       { get; set; }
		public TransferContext         State        { get; set; }
		public string                  FaultMessage { get; set; }
		public IEnumerable<Transition> Connections  { get; set; }
	}
}
