using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using projedeneme.Models;

namespace projedeneme.Services;

public static class GraphBuilder
{
    public static Graph BuildFromRows(List<NodeRow> rows)
    {
        var graph = new Graph();

        // 1) Node oluştur
        foreach (var r in rows)
        {
            var node = new Node
            {
                Id = r.DugumId,
                Aktiflik = r.Aktiflik,
                Etkilesim = r.Etkilesim,
                BaglantiSayisi = r.BaglantiSayisi,
                KomsuIdler = r.Komsular.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                      .Select(s => int.Parse(s.Trim()))
                                      .ToList()
            };
            graph.Nodes.Add(node);
        }

        var dict = graph.Nodes.ToDictionary(n => n.Id);

        // 2) Edge üret (yönsüz, çift edge yok, self-loop yok)
        foreach (var node in graph.Nodes)
        {
            foreach (var komsuId in node.KomsuIdler)
            {
                if (komsuId == node.Id) continue;            // self-loop engel
                if (!dict.ContainsKey(komsuId)) continue;     // hatalı komşu id engel

                // çift edge engeli: sadece küçük id -> büyük id ekle
                if (node.Id >= komsuId) continue;

                var other = dict[komsuId];

                var edge = new Edge
                {
                    From = node,
                    To = other,
                    Weight = WeightCalculator.Calculate(node, other)
                };

                graph.Edges.Add(edge);
            }
        }

        return graph;
    }
}

