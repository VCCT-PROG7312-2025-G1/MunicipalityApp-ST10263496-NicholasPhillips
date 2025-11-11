using System;
using System.Collections.Generic;

namespace MunicipalityApp.DataStructures
{
    // Self-balancing AVL Tree
    // Maintains height-balance via rotations to guarantee O(log N) operations.
    public class AvlTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        // Internal AVL node containing key, value, children and height.
        public class Node
        {
            public TKey Key;
            public TValue Value;
            public Node? Left;
            public Node? Right;
            public int Height = 1;
            public Node(TKey key, TValue value) { Key = key; Value = value; }
        }

        public Node? Root { get; private set; }

        // Inserts or updates a value by key, rebalancing as needed.
        public void Insert(TKey key, TValue value)
        {
            Root = Insert(Root, key, value);
        }

        private Node Insert(Node? node, TKey key, TValue value)
        {
            if (node == null) return new Node(key, value);
            int cmp = key.CompareTo(node.Key);
            if (cmp < 0) node.Left = Insert(node.Left, key, value);
            else if (cmp > 0) node.Right = Insert(node.Right, key, value);
            else node.Value = value;
            UpdateHeight(node);
            return Rebalance(node);
        }


        // Attempts to retrieve a value by key using standard BST search.

        public bool TryGetValue(TKey key, out TValue value)
        {
            var n = Root;
            while (n != null)
            {
                int cmp = key.CompareTo(n.Key);
                if (cmp == 0) { value = n.Value; return true; }
                n = cmp < 0 ? n.Left : n.Right;
            }
            value = default!; return false;
        }


        // Enumerates values in ascending key order.

        public IEnumerable<TValue> InOrder()
        {
            var stack = new Stack<Node>(); var n = Root;
            while (stack.Count > 0 || n != null)
            {
                while (n != null) { stack.Push(n); n = n.Left; }
                n = stack.Pop(); yield return n.Value; n = n.Right;
            }
        }

    private static int HeightOf(Node? n) => n?.Height ?? 0;
    private static void UpdateHeight(Node n) => n.Height = Math.Max(HeightOf(n.Left), HeightOf(n.Right)) + 1;
    private static int Balance(Node? n) => HeightOf(n?.Left) - HeightOf(n?.Right);

        private static Node RotateRight(Node y)
        {
            var x = y.Left!; var T2 = x.Right; x.Right = y; y.Left = T2; UpdateHeight(y); UpdateHeight(x); return x;
        }
        private static Node RotateLeft(Node x)
        {
            var y = x.Right!; var T2 = y.Left; y.Left = x; x.Right = T2; UpdateHeight(x); UpdateHeight(y); return y;
        }
        private static Node Rebalance(Node n)
        {
            int bal = Balance(n);
            if (bal > 1)
            {
                if (Balance(n.Left) < 0) n.Left = RotateLeft(n.Left!);
                return RotateRight(n);
            }
            if (bal < -1)
            {
                if (Balance(n.Right) > 0) n.Right = RotateRight(n.Right!);
                return RotateLeft(n);
            }
            return n;
        }
    }
}
