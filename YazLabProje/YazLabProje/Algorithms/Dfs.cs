using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using projedeneme.Models;

namespace projedeneme.Algorithms;

public static class Dfs
{
    public static List<int> Run(Graph graph, int startId)
    {
        var start = graph.Nodes.FirstOrDefault(n => n.Id == startId)
                    ?? throw new ArgumentException($"Start node bulunamadı: {startId}");

        var visited = new HashSet<int>();
        var order = new List<int>();

        DfsVisit(graph, start, visited, order);
        return order;
    }

    private static void DfsVisit(Graph graph, Node cur, HashSet<int> visited, List<int> order)
    {
        visited.Add(cur.Id);
        order.Add(cur.Id);

        foreach (var nb in GetNeighbors(graph, cur))
        {
            if (!visited.Contains(nb.Id))
                DfsVisit(graph, nb, visited, order);
        }
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

