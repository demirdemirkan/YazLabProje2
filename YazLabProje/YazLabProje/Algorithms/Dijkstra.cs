using System;
using System.Collections.Generic;
using projedeneme.Models;

namespace projedeneme.Algorithms
{
    public static class Dijkstra
    {
        // Sonuç: Path (node id listesi) + toplam maliyet
        public static (List<int> Path, double Cost) Run(Graph graph, int startId, int endId)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            if (startId == endId) return (new List<int> { startId }, 0);

            // Node var mı?
            var nodeById = new Dictionary<int, Node>();
            foreach (var n in graph.Nodes) nodeById[n.Id] = n;

            if (!nodeById.ContainsKey(startId)) throw new Exception($"Start node yok: {startId}");
            if (!nodeById.ContainsKey(endId)) throw new Exception($"End node yok: {endId}");

            // Adjacency: id -> (neighborId, weight)
            var adj = new Dictionary<int, List<(int nb, double w)>>();
            foreach (var n in graph.Nodes) adj[n.Id] = new List<(int, double)>();

            foreach (var e in graph.Edges)
            {
                // yönsüz kabul ediyoruz
                adj[e.From.Id].Add((e.To.Id, e.Weight));
                adj[e.To.Id].Add((e.From.Id, e.Weight));
            }

            // Dijkstra
            var dist = new Dictionary<int, double>();
            var prev = new Dictionary<int, int?>();

            foreach (var id in nodeById.Keys)
            {
                dist[id] = double.PositiveInfinity;
                prev[id] = null;
            }
            dist[startId] = 0;

            // .NET 8: PriorityQueue var
            var pq = new PriorityQueue<int, double>();
            pq.Enqueue(startId, 0);

            while (pq.Count > 0)
            {
                var u = pq.Dequeue();

                if (u == endId) break;

                foreach (var (v, w) in adj[u])
                {
                    if (w < 0) throw new Exception("Dijkstra negatif ağırlık istemez.");

                    double alt = dist[u] + w;
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                        pq.Enqueue(v, alt);
                    }
                }
            }

            if (double.IsPositiveInfinity(dist[endId]))
                throw new Exception($"Ulaşılamıyor: {startId} -> {endId}");

            // Path’i geri sar
            var path = new List<int>();
            int cur = endId;
            while (true)
            {
                path.Add(cur);
                if (cur == startId) break;
                cur = prev[cur] ?? throw new Exception("Path çıkarılamadı (prev null).");
            }
            path.Reverse();

            return (path, dist[endId]);
        }
    }
}
