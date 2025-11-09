using System;

namespace MunicipalityApp.DataStructures
{
    // Left-leaning Red-Black Tree
    // Provides balanced insertions with near O(log N) operations.

    public class RedBlackTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private const bool Red = true;
        private const bool Black = false;


        // Internal node with color information used for balancing.

        public class Node
        {
            public TKey Key;
            public TValue Value;
            public Node Left;
            public Node Right;
            public bool Color = Red; // new nodes are red
            public Node(TKey key, TValue value) { Key = key; Value = value; }
        }

        public Node Root { get; private set; }

        private static bool IsRed(Node n) => n != null && n.Color == Red;

        private static Node RotateLeft(Node h)
        {
            var x = h.Right; h.Right = x.Left; x.Left = h; x.Color = h.Color; h.Color = Red; return x;
        }
        private static Node RotateRight(Node h)
        {
            var x = h.Left; h.Left = x.Right; x.Right = h; x.Color = h.Color; h.Color = Red; return x;
        }
        private static void FlipColors(Node h)
        {
            h.Color = Red; if (h.Left != null) h.Left.Color = Black; if (h.Right != null) h.Right.Color = Black;
        }

        // Inserts or updates a value by key while maintaining red-black invariants.

        public void Insert(TKey key, TValue value)
        {
            Root = Insert(Root, key, value);
            Root.Color = Black;
        }
        private Node Insert(Node h, TKey key, TValue value)
        {
            if (h == null) return new Node(key, value);
            int cmp = key.CompareTo(h.Key);
            if (cmp < 0) h.Left = Insert(h.Left, key, value);
            else if (cmp > 0) h.Right = Insert(h.Right, key, value);
            else h.Value = value;

            if (IsRed(h.Right) && !IsRed(h.Left)) h = RotateLeft(h);
            if (IsRed(h.Left) && IsRed(h.Left.Left)) h = RotateRight(h);
            if (IsRed(h.Left) && IsRed(h.Right)) FlipColors(h);
            return h;
        }

        // Attempts to retrieve a value by key using standard BST search over the RBT.

        public bool TryGetValue(TKey key, out TValue value)
        {
            var x = Root;
            while (x != null)
            {
                int cmp = key.CompareTo(x.Key);
                if (cmp == 0) { value = x.Value; return true; }
                x = cmp < 0 ? x.Left : x.Right;
            }
            value = default!; return false;
        }
    }
}
