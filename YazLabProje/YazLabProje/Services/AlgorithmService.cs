using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using projedeneme.Algorithms;
using projedeneme.Models;

namespace projedeneme.Services;

public class AlgorithmService
{
    private readonly Graph _graph;

    public AlgorithmService(Graph graph)
    {
        _graph = graph;
    }

    public (List<int> Order, long ElapsedMs) RunBfs(int startId)
    {
        var sw = Stopwatch.StartNew();
        var order = Bfs.Run(_graph, startId);
        sw.Stop();
        return (order, sw.ElapsedMilliseconds);
    }

    public (List<int> Order, long ElapsedMs) RunDfs(int startId)
    {
        var sw = Stopwatch.StartNew();
        var order = Dfs.Run(_graph, startId);
        sw.Stop();
        return (order, sw.ElapsedMilliseconds);
    }
}

