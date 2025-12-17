using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projedeneme.Models;

public class Edge
{
    public Node From { get; set; } = null!;
    public Node To { get; set; } = null!;
    public double Weight { get; set; }
}

