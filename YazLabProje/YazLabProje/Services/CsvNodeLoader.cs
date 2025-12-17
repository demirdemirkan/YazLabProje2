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
}
