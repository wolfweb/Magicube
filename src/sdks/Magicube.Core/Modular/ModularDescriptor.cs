using System;

namespace Magicube.Core.Modular {
    public abstract class ModularDescriptor {        
        public abstract string      Name         { get; }
        public abstract string      Display      { get; }
        
        public virtual  string      Author       { get; set; } = "wolfweb";
        public virtual  string      Category     { get; set; }
        public virtual  string      Description  { get; } = "Magicube Framework";
        
        public virtual  ModularType Type         { get; set; }
        public virtual  Version     Version      => GetType().Assembly.GetName().Version;
    }

    public enum ModularType {
        Modular,
        Theme
    }
}
