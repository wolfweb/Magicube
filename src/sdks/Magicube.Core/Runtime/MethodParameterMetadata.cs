using System;

namespace Magicube.Core.Runtime {
    public sealed class MethodParameterMetadata {
		public string Name             { get; set; }
		public string Title            { get; set; }
	    public string Descript         { get; set; }
		public int    Position         { get; set; }
		public object DefaultValue     { get; set; }
		public Type   ParameterType    { get; set; }
		public bool   HasDefaultValue  { get; set; }
	}
}
