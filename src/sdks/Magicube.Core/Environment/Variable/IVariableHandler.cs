using System;
using System.Text.RegularExpressions;

namespace Magicube.Core.Environment.Variable {
    public interface IVariableHandler {
        string Name        { get; }
        int    Priority    { get; }
        string Description { get; }
        string ParseVariable(string template, string source);
    }

    public abstract class BaseVariableHandler : IVariableHandler {
        public virtual  int    Priority     => 0;
        public abstract string Name         { get; }
        public abstract string Description  { get; }
        public abstract string ParseVariable(string template, string source);

        protected Match Find(string str) {
            return Regex.Match(str, @$"\{Name}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}
