using Magicube.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Magicube.Text.TrieTree {
    public class Trie : IEnumerable<string> {
        private int      _count { get; set; }
        private TrieNode _root  { get; set; }

        public Trie() {
            _count = 0;
            _root  = new TrieNode(' ');
        }

        public int  Count   => _count;
        public bool IsEmpty => _count == 0; 

        public void Add(string word) {
            if (word.IsNullOrEmpty()) throw new ArgumentException("Word is empty or null.");

            var node = SearchPrefix(word);
            if (node != null) {
                word = word.Replace(node.Word, "");
                if (!node.CanEnd) node.CanEnd = true;
            }

            var current = _root;

            for (int i = 0; i < word.Length; ++i) {
                if (!current.Children.ContainsKey(word[i])) {                    
                    var newTrieNode = new TrieNode(word[i]);
                    newTrieNode.Parent = current;
                    current.Children.Add(word[i], newTrieNode);
                }
                current = current.Children[word[i]];
            }

            ++_count;
            current.CanEnd = true;
        }

        public void Remove(string word) {
            if (word.IsNullOrEmpty()) throw new ArgumentException("Word is empty or null.");

            var current = _root;

            for (int i = 0; i < word.Length; ++i) {
                if (!current.Children.ContainsKey(word[i])) throw new KeyNotFoundException("Word doesn't belong to trie.");
                current = current.Children[word[i]];
            }

            if (current.Children.Count != 0) throw new KeyNotFoundException("Word doesn't belong to trie.");

            --_count;
            current.Remove();
        }

        public List<TrieNode> ContainsWords(string word) {
            if (word.IsNullOrEmpty()) throw new ApplicationException("Word is either null or empty.");

            var result  = new List<TrieNode>();
            var current = _root;

            for (int i = 0; i < word.Length && i+1 < word.Length; ++i) {
                if (!current.Children.ContainsKey(word[i])) {
                    if (current != _root) --i;
                    current = _root;
                    continue;
                }

                current = current.Children[word[i]];
                if (current.CanEnd && !current.Children.ContainsKey(word[i+1])) {
                    result.Add(current);
                    current = _root;
                }
            }

            return result;
        }

        public bool ContainsPrefix(string prefix) {
            if (prefix.IsNullOrEmpty()) throw new ApplicationException("Prefix is either null or empty.");

            var current = _root;

            for (int i = 0; i < prefix.Length; ++i) {
                if (!current.Children.ContainsKey(prefix[i]))
                    return false;

                current = current.Children[prefix[i]];
            }

            return true;
        }

        public IEnumerable<string> SearchByPrefix(string prefix) {
            if (prefix.IsNullOrEmpty()) throw new ApplicationException("Prefix is either null or empty.");

            var node = SearchPrefix(prefix);

            if (node == null) return Enumerable.Empty<string>();

            var result = GetNodeWords(node);

            return result;
        }

        public TrieNode SearchPrefix(string prefix) {
            if (prefix.IsNullOrEmpty()) throw new ApplicationException("Prefix is either null or empty.");

            var current = _root;
            for (int i = 0; i < prefix.Length; ++i) {
                if (!current.Children.ContainsKey(prefix[i]))
                    return null;

                current = current.Children[prefix[i]];
            }
            return current;
        }

        public void Clear() {
            _count = 0;
            _root.Clear();
            _root = new TrieNode(' ');
        }

        public IEnumerator<string> GetEnumerator() {
            return _root.GetTerminalChildren().Select(node => node.Word).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        private IEnumerable<string> GetNodeWords(TrieNode node) {
            var list = new List<string>();

            foreach (var child in node.Children) {
                if (child.Value.CanEnd) {
                    list.Add(child.Value.Word);
                }
                if (child.Value != null && child.Value.Children.Count > 0) {
                    list.AddRange(GetNodeWords(child.Value));
                }
            }
            return list;
        }
    }
}
