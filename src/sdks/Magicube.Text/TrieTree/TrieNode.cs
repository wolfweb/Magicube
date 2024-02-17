using System.Collections.Generic;
using System.Text;

namespace Magicube.Text.TrieTree {
    public class TrieNode {
        public virtual char                       Key        { get; set; }
        public virtual bool                       CanEnd     { get; set; }
        public virtual TrieNode                   Parent     { get; set; }
        public virtual Dictionary<char, TrieNode> Children   { get; set; }

        public TrieNode(char key) : this(key, false) { }

        public TrieNode(char key, bool canEnd) {
            Key        = key;        
            CanEnd     = canEnd;
            Children   = new Dictionary<char, TrieNode>();
        }

        public virtual string Word {
            get {
                var curr  = this;
                var stack = new Stack<char>();

                while (curr.Parent != null) {
                    stack.Push(curr.Key);
                    curr = curr.Parent;
                }

                return new string(stack.ToArray());
            }
        }

        public virtual IEnumerable<string> GetByPrefix() {
            if (Children.Count == 0)  yield return Word;

            foreach (var childKeyVal in Children)
                foreach (var terminalNode in childKeyVal.Value.GetByPrefix())
                    yield return terminalNode;
        }

        public virtual IEnumerable<TrieNode> GetTerminalChildren() {
            foreach (var child in Children.Values) {
                if (child.Children.Count == 0)
                    yield return child;

                foreach (var grandChild in child.GetTerminalChildren())
                    if (grandChild.Children.Count == 0)
                        yield return grandChild;
            }
        }

        public virtual void Remove() {
            if (Children.Count == 0 && Parent != null) {
                Parent.Children.Remove(Key);
                Parent.Remove();
            }
        }

        public int CompareTo(TrieNode other) {
            if (other == null)
                return -1;

            return Key.CompareTo(other.Key);

        }

        public void Clear() {
            Children.Clear();
            Children = null;
        }
    }
}
