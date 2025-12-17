using System;
using System.IO;
using System.Windows;

using projedeneme.Services;
using projedeneme.Models;


namespace projedeneme
{
    public partial class MainWindow : Window
    {
        private Graph? _graph;

        public MainWindow()
        {
            InitializeComponent();
        }

        // 1) Small CSV Yükle butonu
        private void OnLoadSmallClick(object sender, RoutedEventArgs e)
        {
            LoadDatasetFromDataFolder("small.csv");
        }

        // 2) Medium CSV Yükle butonu
        private void OnLoadMediumClick(object sender, RoutedEventArgs e)
        {
            LoadDatasetFromDataFolder("medium.csv");
        }

        private void LoadDatasetFromDataFolder(string fileName)
        {
            // bin/Debug/net8.0-windows/Data/...
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", fileName);

            if (!File.Exists(path))
            {
                MessageBox.Show(
                    $"CSV bulunamadı:\n{path}\n\n" +
                    "Data klasörünü ve CSV dosyalarında 'Copy if newer' ayarını kontrol et.");
                return;
            }

            var rows = CsvNodeLoader.Load(path);

            // Graph kur + ağırlık hesapla (GraphBuilder.cs ekledikten sonra çalışır)
            var graph = GraphBuilder.BuildFromRows(rows);

            MessageBox.Show(
                $"{fileName} yüklendi.\n" +
                $"Node: {graph.Nodes.Count}\n" +
                $"Edge: {graph.Edges.Count}\n" +
                $"Örnek ağırlık: {(graph.Edges.Count > 0 ? graph.Edges[0].Weight.ToString("F4") : "YOK")}"
            );
            _graph = GraphBuilder.BuildFromRows(rows);

        }
        private void OnBfsClick(object sender, RoutedEventArgs e)
        {
            if (_graph == null)
            {
                MessageBox.Show("Önce Small/Medium CSV yüklemelisin.");
                return;
            }

            var service = new AlgorithmService(_graph);
            var (order, ms) = service.RunBfs(1);

            MessageBox.Show($"BFS (start=1)\nSüre: {ms} ms\nSıra:\n{string.Join(", ", order)}");
        }

        private void OnDfsClick(object sender, RoutedEventArgs e)
        {
            if (_graph == null)
            {
                MessageBox.Show("Önce Small/Medium CSV yüklemelisin.");
                return;
            }

            var service = new AlgorithmService(_graph);
            var (order, ms) = service.RunDfs(1);

            MessageBox.Show($"DFS (start=1)\nSüre: {ms} ms\nSıra:\n{string.Join(", ", order)}");
        }

    }
}
