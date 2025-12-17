using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using projedeneme.Models;
using projedeneme.Services;
using projedeneme.Algorithms;

namespace projedeneme
{
    public partial class MainWindow : Window
    {
        private bool nextClickIsStart = true;

        private readonly Dictionary<int, Ellipse> nodeEllipses = new Dictionary<int, Ellipse>();
        private readonly Dictionary<(int a, int b), Line> edgeLines = new Dictionary<(int a, int b), Line>();

        private int? selectedStartId = null;
        private int? selectedEndId = null;

        private Graph _graph = null; // nullable kapalı uyum

        public MainWindow()
        {
            InitializeComponent();
            Log("UI hazır. CSV seçip 'Yükle'ye bas.");
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            var csvFile = (CsvCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (string.IsNullOrWhiteSpace(csvFile))
            {
                Log("[HATA] CSV seçilmedi.");
                return;
            }

            try
            {
                // Path çakışması olmasın diye full name
                var path = System.IO.Path.Combine(AppContext.BaseDirectory, "Data", csvFile);
                Log($"[UI] CSV path: {path}");

                var rows = CsvNodeLoader.Load(path);          // List<NodeRow>
                _graph = GraphBuilder.BuildFromRows(rows);    // Services/GraphBuilder.cs

                DrawGraphFromGraph(_graph);
                Log($"[OK] Yüklendi. Node={_graph.Nodes.Count}, Edge={_graph.Edges.Count}");
            }
            catch (Exception ex)
            {
                Log("[HATA] " + ex.Message);
            }
        }

        private void BtnRun_Click(object sender, RoutedEventArgs e)
        {
            if (_graph == null)
            {
                Log("[HATA] Önce CSV yükle.");
                return;
            }

            var algo = (AlgoCombo.SelectedItem as ComboBoxItem)?.Content?.ToString();
            Log($"[UI] Algo: {algo}, Start={TxtStart.Text}, End={TxtEnd.Text}");

            int startId;
            if (!int.TryParse(TxtStart.Text, out startId))
            {
                Log("[HATA] Start seç (node'a tıkla).");
                return;
            }

            try
            {
                // BFS/DFS start ile çalışıyor (end yok)
                List<int> order;
                if (algo == "BFS")
                    order = Bfs.Run(_graph, startId);
                else if (algo == "DFS")
                    order = Dfs.Run(_graph, startId);
                else
                    throw new Exception("Şimdilik sadece BFS/DFS bağlı.");

                Log($"[OK] {algo} order: {string.Join("->", order)}");

                // Traversal sırasını highlight etmeyi dener
                HighlightPath(order.ToArray());
            }
            catch (Exception ex)
            {
                Log("[HATA] " + ex.Message);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            GraphCanvas.Children.Clear();

            nodeEllipses.Clear();
            edgeLines.Clear();

            selectedStartId = null;
            selectedEndId = null;
            nextClickIsStart = true;

            _graph = null;

            Log("[UI] Canvas temizlendi.");
        }

        // CSV'de X/Y yok => çember yerleşimi
        private void DrawGraphFromGraph(Graph graph)
        {
            GraphCanvas.Children.Clear();

            nodeEllipses.Clear();
            edgeLines.Clear();

            selectedStartId = null;
            selectedEndId = null;
            nextClickIsStart = true;

            double W = GraphCanvas.ActualWidth > 10 ? GraphCanvas.ActualWidth : 800;
            double H = GraphCanvas.ActualHeight > 10 ? GraphCanvas.ActualHeight : 450;

            double cx = W / 2.0;
            double cy = H / 2.0;
            double radius = Math.Min(W, H) * 0.35;

            var nodes = graph.Nodes.OrderBy(n => n.Id).ToList();
            int nCount = nodes.Count;

            var pos = new Dictionary<int, (double x, double y)>();
            for (int i = 0; i < nCount; i++)
            {
                double angle = 2 * Math.PI * i / nCount;
                pos[nodes[i].Id] = (cx + radius * Math.Cos(angle), cy + radius * Math.Sin(angle));
            }

            // Edge çiz
            foreach (var e in graph.Edges)
            {
                (double x, double y) p1;
                (double x, double y) p2;
                if (!pos.TryGetValue(e.From.Id, out p1)) continue;
                if (!pos.TryGetValue(e.To.Id, out p2)) continue;

                var line = new Line
                {
                    X1 = p1.x,
                    Y1 = p1.y,
                    X2 = p2.x,
                    Y2 = p2.y,
                    StrokeThickness = 2,
                    Stroke = Brushes.DimGray,
                    IsHitTestVisible = false
                };
                GraphCanvas.Children.Add(line);

                int u = e.From.Id, v = e.To.Id;
                if (u > v) { int tmp = u; u = v; v = tmp; }
                edgeLines[(u, v)] = line;

                // Weight checkbox XAML'de yoksa derleme hatası olur; varsa çalışır
                if (ChkShowWeights != null && ChkShowWeights.IsChecked == true)
                {
                    var t = new TextBlock
                    {
                        Text = e.Weight.ToString("0.##"),
                        Foreground = Brushes.White,
                        FontSize = 12,
                        IsHitTestVisible = false
                    };
                    Canvas.SetLeft(t, (p1.x + p2.x) / 2 + 4);
                    Canvas.SetTop(t, (p1.y + p2.y) / 2 + 2);
                    GraphCanvas.Children.Add(t);
                }
            }

            // Node çiz
            foreach (var n in nodes)
            {
                var p = pos[n.Id];
                DrawNode(n.Id, p.x, p.y);
            }

            UpdateNodeColors();
        }

        private void DrawNode(int id, double x, double y)
        {
            double r = 16;

            var ellipse = new Ellipse
            {
                Width = r * 2,
                Height = r * 2,
                Fill = Brushes.Orange,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            ellipse.Tag = id;
            ellipse.MouseLeftButtonDown += Node_MouseLeftButtonDown;

            Canvas.SetLeft(ellipse, x - r);
            Canvas.SetTop(ellipse, y - r);
            GraphCanvas.Children.Add(ellipse);

            nodeEllipses[id] = ellipse;

            if (ChkShowIds.IsChecked == true)
            {
                var text = new TextBlock
                {
                    Text = id.ToString(),
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    IsHitTestVisible = false
                };
                Canvas.SetLeft(text, x - 4);
                Canvas.SetTop(text, y - 10);
                GraphCanvas.Children.Add(text);
            }
        }

        private void Node_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var el = sender as Ellipse;
            if (el == null) return;

            int id;
            if (el.Tag == null || !int.TryParse(el.Tag.ToString(), out id)) return;

            if (nextClickIsStart)
            {
                selectedStartId = id;
                TxtStart.Text = id.ToString();
                Log($"[UI] Start seçildi: {id}");
            }
            else
            {
                selectedEndId = id;
                TxtEnd.Text = id.ToString();
                Log($"[UI] End seçildi: {id}");
            }

            nextClickIsStart = !nextClickIsStart;
            UpdateNodeColors();
        }

        private void UpdateNodeColors()
        {
            foreach (var kvp in nodeEllipses)
            {
                int id = kvp.Key;
                Ellipse el = kvp.Value;

                if (selectedStartId.HasValue && id == selectedStartId.Value)
                    el.Fill = Brushes.LimeGreen;
                else if (selectedEndId.HasValue && id == selectedEndId.Value)
                    el.Fill = Brushes.Red;
                else
                    el.Fill = Brushes.Orange;
            }
        }

        private void HighlightPath(int[] pathNodeIds)
        {
            foreach (var kvp in edgeLines)
            {
                kvp.Value.Stroke = Brushes.DimGray;
                kvp.Value.StrokeThickness = 2;
            }

            for (int i = 0; i < pathNodeIds.Length - 1; i++)
            {
                int a = pathNodeIds[i];
                int b = pathNodeIds[i + 1];
                int u = a, v = b;
                if (u > v) { int tmp = u; u = v; v = tmp; }

                Line line;
                if (edgeLines.TryGetValue((u, v), out line))
                {
                    line.Stroke = Brushes.DeepSkyBlue;
                    line.StrokeThickness = 5;
                }
            }

            Log($"[UI] Highlight: {string.Join("->", pathNodeIds)}");
        }

        private void Log(string msg)
        {
            TxtLog.AppendText(msg + "\n");
            TxtLog.ScrollToEnd();
        }
    }
}
