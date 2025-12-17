using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using projedeneme.Models;

namespace projedeneme.Services;

public static class WeightCalculator
{
    public static double Calculate(Node a, Node b)
    {
        return 1.0 / (1
            + Math.Pow(a.Aktiflik - b.Aktiflik, 2)
            + Math.Pow(a.Etkilesim - b.Etkilesim, 2)
            + Math.Pow(a.BaglantiSayisi - b.BaglantiSayisi, 2));
    }
}
