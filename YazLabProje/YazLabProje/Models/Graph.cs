using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projedeneme.Models;

public class Graph
{
    public List<Node> Nodes { get; } = new();
    public List<Edge> Edges { get; } = new();
}

