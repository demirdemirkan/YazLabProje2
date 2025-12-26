using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using projedeneme.Models;

namespace projedeneme.Services;

public static class CsvNodeLoader
{
    public static List<NodeRow> Load(string path)
    {
        var lines = File.ReadAllLines(path);
        var list = new List<NodeRow>();

        for (int i = 1; i < lines.Length; i++) // header'ı atla
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            var cols = SplitCsvLine(lines[i]);

            list.Add(new NodeRow
            {
                DugumId = int.Parse(cols[0]),
                Aktiflik = double.Parse(cols[1], CultureInfo.InvariantCulture),
                Etkilesim = int.Parse(cols[2]),
                BaglantiSayisi = int.Parse(cols[3]),
                Komsular = cols[4].Trim('"')
            });
        }

        return list;
    }

    private static string[] SplitCsvLine(string line)
    {
        var list = new List<string>();
        bool inQuotes = false;
        var current = "";

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (c == ',' && !inQuotes)
            {
                list.Add(current);
                current = "";
            }
            else
            {
                current += c;
            }
        }

        list.Add(current);
        return list.ToArray();
    }
    public static void Save(string path, IEnumerable<NodeRow> rows)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var sb = new StringBuilder();
        sb.AppendLine("DugumId,Aktiflik,Etkilesim,BaglantiSayisi,Komsular");

        foreach (var r in rows.OrderBy(x => x.DugumId))
        {
            // komşular: "2,6,71,75" formatına normalize
            var neighbors = ParseNeighbors(r.Komsular).Distinct().OrderBy(x => x).ToList();
            var komsularStr = string.Join(",", neighbors);
            var baglantiSayisi = neighbors.Count;

            sb.Append(r.DugumId).Append(",");
            sb.Append(r.Aktiflik.ToString(CultureInfo.InvariantCulture)).Append(",");
            sb.Append(r.Etkilesim).Append(",");
            sb.Append(baglantiSayisi).Append(",");
            sb.Append("\"").Append(komsularStr).AppendLine("\"");
        }

        File.WriteAllText(path, sb.ToString());
    }

    private static IEnumerable<int> ParseNeighbors(string komsular)
    {
        if (string.IsNullOrWhiteSpace(komsular))
            yield break;

        var parts = komsular.Trim().Trim('"')
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var p in parts)
            if (int.TryParse(p, out var id))
                yield return id;
    }

}
