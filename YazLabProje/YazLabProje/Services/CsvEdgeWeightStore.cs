using System.Globalization;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using projedeneme.Models;

namespace projedeneme.Services;

public static class CsvEdgeWeightStore
{
    public static void Save(string path, IEnumerable<Edge> edges)
    {
        using var sw = new StreamWriter(path, false);
        sw.WriteLine("From,To,Weight");

        foreach (var e in edges)
        {
            int a = e.From.Id, b = e.To.Id;
            int u = a < b ? a : b;
            int v = a < b ? b : a;

            sw.WriteLine($"{u},{v},{e.Weight.ToString(CultureInfo.InvariantCulture)}");
        }
    }

    public static Dictionary<(int u, int v), double> LoadAsMap(string path)
    {
        var map = new Dictionary<(int u, int v), double>();
        if (!File.Exists(path)) return map;

        var lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;
            var cols = lines[i].Split(',');

            int u = int.Parse(cols[0]);
            int v = int.Parse(cols[1]);
            double w = double.Parse(cols[2], CultureInfo.InvariantCulture);

            if (u > v) (u, v) = (v, u);
            map[(u, v)] = w;
        }
        return map;
    }
}
