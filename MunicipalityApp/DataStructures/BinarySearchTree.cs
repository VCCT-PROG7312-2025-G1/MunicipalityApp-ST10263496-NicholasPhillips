using System;
using System.Collections.Generic;

namespace MunicipalityApp.DataStructures
{
    // Demonstrates ordered insertion, search, and in-order traversal semantics.
    public class BinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
    {

        // Internal tree node holding a key, value and left/right children.
        public class Node
        {
            public TKey Key;
            public TValue Value;
            public Node Left;
            public Node Right;
            public Node(TKey key, TValue value)
            {
                Key = key; Value = value;
            }
        }

        public Node Root { get; private set; }

        // Inserts or updates a value by key following BST ordering rules.
        public void Insert(TKey key, TValue value)
        {
            Root = Insert(Root, key, value);
        }

        private Node Insert(Node node, TKey key, TValue value)
        {
            if (node == null) return new Node(key, value);
            int cmp = key.CompareTo(node.Key);
            if (cmp < 0) node.Left = Insert(node.Left, key, value);
            else if (cmp > 0) node.Right = Insert(node.Right, key, value);
            else node.Value = value;
            return node;
        }

        // Attempts to retrieve a value associated with the specified key.
        public bool TryGetValue(TKey key, out TValue value)
        {
            var node = Root;
            while (node != null)
            {
                int cmp = key.CompareTo(node.Key);
                if (cmp == 0) { value = node.Value; return true; }
                node = cmp < 0 ? node.Left : node.Right;
            }
            value = default!;
            return false;
        }
        
        // Iterates values in ascending key order via in-order traversal.
        public IEnumerable<TValue> InOrder()
        {
            var stack = new Stack<Node>();
            var node = Root;
            while (stack.Count > 0 || node != null)
            {
                while (node != null) { stack.Push(node); node = node.Left; }
                node = stack.Pop();
                yield return node.Value;
                node = node.Right;
            }
        }
    }
}
