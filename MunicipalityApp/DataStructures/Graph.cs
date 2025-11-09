using System;
using System.Collections.Generic;

namespace MunicipalityApp.DataStructures
{
    // Lightweight undirected weighted graph with BFS traversal and Prim-based MST.
    // Nodes are indexed integers mapping to stored values of type <typeparamref name="T"/>.

    public class Graph<T>
    {

        // Edge representation used for MST output.
        public class Edge
        {
            public int U;
            public int V;
            public double W;
        }

        private readonly List<T> _nodes = new();
        private readonly List<List<(int v, double w)>> _adj = new();
        private readonly Dictionary<T, int> _index = new();


        // Adds a node with the provided value if it doesn't exist and returns its index.
        public int AddNode(T value)
        {
            if (_index.TryGetValue(value, out var idx)) return idx;
            idx = _nodes.Count; _nodes.Add(value); _adj.Add(new()); _index[value] = idx; return idx;
        }
        // Adds an undirected edge between two node indices with weight <paramref name="w"/>.
        public void AddUndirectedEdge(int u, int v, double w)
        {
            _adj[u].Add((v, w));
            _adj[v].Add((u, w));
        }

        // Breadth-first traversal starting at the specified node index, yielding node values.
        public IEnumerable<T> BfsFrom(int start)
        {
            var seen = new bool[_nodes.Count];
            var q = new Queue<int>();
            q.Enqueue(start); seen[start] = true;
            while (q.Count > 0)
            {
                int u = q.Dequeue();
                yield return _nodes[u];
                foreach (var (v, _) in _adj[u]) if (!seen[v]) { seen[v] = true; q.Enqueue(v); }
            }
        }
        
        // Minimum Spanning Tree using Prim's algorithm
        public List<Edge> MinimumSpanningTree()
        {
            // Prim's algorithm
            int n = _nodes.Count;
            if (n == 0) return new List<Edge>();
            var inMst = new bool[n];
            var pq = new PriorityQueue<(double w, int u, int v), double>();
            inMst[0] = true;
            foreach (var (v, w) in _adj[0]) pq.Enqueue((w, 0, v), w);
            var mst = new List<Edge>();
            while (pq.Count > 0 && mst.Count < n - 1)
            {
                var (w, u, v) = pq.Dequeue();
                if (inMst[v]) continue;
                inMst[v] = true;
                mst.Add(new Edge { U = u, V = v, W = w });
                foreach (var (nv, nw) in _adj[v]) if (!inMst[nv]) pq.Enqueue((nw, v, nv), nw);
            }
            return mst;
        }
    }
}
