using System;
using System.Collections.Generic;
using System.Linq;
using MunicipalityApp.Data;
using MunicipalityApp.Models;
using MunicipalityApp.DataStructures;

namespace MunicipalityApp.Services
{

    // Builds and exposes multiple advanced data structures over the in-memory service requests.
    public class ServiceRequestIndex
    {
        private readonly BinarySearchTree<Guid, UserIssue> _bstById = new();
        private readonly AvlTree<long, UserIssue> _avlByTimeKey = new();
        private readonly RedBlackTree<string, UserIssue> _rbtByTitle = new();
        private readonly MinHeap<UserIssue> _minHeapByProgress;
        private readonly Graph<string> _categoryGraph = new();

        private readonly List<UserIssue> _all;

        // Constructs a new index over the provided set of issues and builds all structures.
        public ServiceRequestIndex(IEnumerable<UserIssue> issues)
        {
            _all = issues?.ToList() ?? new List<UserIssue>();
            _minHeapByProgress = new MinHeap<UserIssue>(Comparer<UserIssue>.Create((a, b) =>
            {
                int c = a.Progress.CompareTo(b.Progress);
                if (c != 0) return c;
                return a.ReportedDate.CompareTo(b.ReportedDate);
            }));
            Build();
        }

        private static long TimeKey(DateTime dt, Guid id)
        {
            // Create a mostly-unique long key from time ticks with a tie-breaker using guid hash code
            unchecked
            {
                return dt.Ticks ^ (long)id.GetHashCode();
            }
        }

        private void Build()
        {
            // Build trees and heap
            foreach (var issue in _all)
            {
                _bstById.Insert(issue.Id, issue);
                _avlByTimeKey.Insert(TimeKey(issue.ReportedDate, issue.Id), issue);
                _rbtByTitle.Insert(issue.Title ?? string.Empty, issue);
                _minHeapByProgress.Push(issue);
            }

            // Build a simple category graph from categories present; connect consecutive categories
            var categories = _all.Select(i => i.Category ?? "Uncategorised").Distinct().ToList();
            var indices = new Dictionary<string, int>();
            for (int i = 0; i < categories.Count; i++)
            {
                indices[categories[i]] = _categoryGraph.AddNode(categories[i]);
            }
            for (int i = 1; i < categories.Count; i++)
            {
                int u = indices[categories[i - 1]];
                int v = indices[categories[i]];
                _categoryGraph.AddUndirectedEdge(u, v, 1.0);
            }
        }


        // Looks up a service request by its unique identifier using the BST index.
        public bool TryFindById(Guid id, out UserIssue issue) => _bstById.TryGetValue(id, out issue);


        // Enumerates service requests in chronological order using the AVL in-order traversal.
        public IEnumerable<UserIssue> InOrderByReportedDate()
        {
            return _avlByTimeKey.InOrder();
        }


        // Attempts to find a request by title using the Red-Black Tree index.
        public bool TryFindByTitle(string title, out UserIssue issue)
        {
            return _rbtByTitle.TryGetValue(title ?? string.Empty, out issue);
        }


        // Returns up to <paramref name="max"/> requests with the smallest progress values (ties broken by reported date).
        public IEnumerable<UserIssue> SmallestProgressFirst(int max = 10)
        {
            // non-destructive: copy heap
            var temp = new MinHeap<UserIssue>(Comparer<UserIssue>.Create((a, b) =>
            {
                int c = a.Progress.CompareTo(b.Progress);
                if (c != 0) return c;
                return a.ReportedDate.CompareTo(b.ReportedDate);
            }));
            foreach (var i in _all) temp.Push(i);
            var result = new List<UserIssue>();
            for (int k = 0; k < max && temp.Count > 0; k++) result.Add(temp.Pop());
            return result;
        }

        // Performs a BFS across the category graph starting from the first node to demonstrate traversal.
        public IEnumerable<string> CategoryBfs()
        {
            // BFS over category graph nodes starting at 0 if exists
            if (!(_all?.Any() ?? false)) return Enumerable.Empty<string>();
            return _categoryGraph.BfsFrom(0);
        }


        // Computes a category Minimum Spanning Tree using Prim's algorithm and returns edges.
        public IEnumerable<(string u, string v, double w)> CategoryMst()
        {
            var edges = _categoryGraph.MinimumSpanningTree();
            // We don't keep reverse map here, but for demo we return weights only
            return edges.Select(e => (u: e.U.ToString(), v: e.V.ToString(), w: e.W));
        }
    }
}
