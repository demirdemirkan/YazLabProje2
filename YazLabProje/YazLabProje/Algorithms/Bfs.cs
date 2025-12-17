using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using projedeneme.Models;

namespace projedeneme.Algorithms;

public static class Bfs
{
    public static List<int> Run(Graph graph, int startId)
    {
        var start = graph.Nodes.FirstOrDefault(n => n.Id == startId)
                    ?? throw new ArgumentException($"Start node bulunamadı: {startId}");

        var visited = new HashSet<int>();
        var q = new Queue<Node>();
        var order = new List<int>();

        visited.Add(start.Id);
        q.Enqueue(start);

        while (q.Count > 0)
        {
            var cur = q.Dequeue();
            order.Add(cur.Id);

            foreach (var nb in GetNeighbors(graph, cur))
            {
                if (visited.Add(nb.Id))
                    q.Enqueue(nb);
            }
        }

        return order;
    }

    private static IEnumerable<Node> GetNeighbors(Graph graph, Node node)
    {
        foreach (var e in graph.Edges)
        {
            if (e.From == node) yield return e.To;
            else if (e.To == node) yield return e.From;
        }
    }
}

