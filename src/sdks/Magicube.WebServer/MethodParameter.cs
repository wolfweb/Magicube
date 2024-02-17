using System;

namespace Magicube.WebServer {
    public class MethodParameter {
        public string Description;
        public bool   IsDynamic;
        public bool   IsOptional;
        public string Name;
        public int    Position;
        public Type   Type;
        public bool   HasDefaultValue;
        public object DefaultValue;
    }

}
