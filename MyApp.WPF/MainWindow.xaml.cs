using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using MyApp;
using IOPath = System.IO.Path;

namespace MyApp.WPF
{
    public partial class MainWindow : Window
    {
        // Экземпляры лабораторных работ
        private Lab4 lab4;
        private Lab5 lab5;
        private Lab6? lab6;
        
        // Для эксперимента
        private Lab5 lab5Large;
        private Lab4 lab4Large;

        // Пути к файлам данных
        private string? graphFilePath;
        private string? weightedGraphFilePath;
        private string? largeGraphFilePath;

        public MainWindow()
        {
            InitializeComponent();
            
            // Инициализация
            lab4 = new Lab4();
            lab5 = new Lab5();
            lab5Large = new Lab5();
            lab4Large = new Lab4();
            
            // Создаем тестовые файлы при запуске
            CreateTestDataFiles();
            
            // Автоматически загружаем графы при запуске
            try
            {
                // Загружаем обычный граф для Lab4
                lab4.LoadGraph(graphFilePath);
                
                // Загружаем взвешенный граф для Lab5 и Lab6
                lab5.LoadWeightedGraph(weightedGraphFilePath);
                lab6 = new Lab6(lab5.GetGraph(), lab5.GetNodes());
                
                // Заполняем ComboBox'ы
                var nodes = lab5.GetNodes();
                if (nodes != null && nodes.Count > 0)
                {
                    var nodeList = new List<string>(nodes);
                    
                    Lab5FromNode.ItemsSource = nodeList;
                    Lab5ToNode.ItemsSource = nodeList;
                    Lab6FromNode.ItemsSource = nodeList;
                    Lab6ToNode.ItemsSource = nodeList;
                    
                    // Устанавливаем значения по умолчанию
                    if (nodes.Contains("НС_Московская"))
                    {
                        Lab5FromNode.SelectedItem = "НС_Московская";
                        Lab6FromNode.SelectedItem = "НС_Московская";
                    }
                    else
                    {
                        Lab5FromNode.SelectedIndex = 0;
                        Lab6FromNode.SelectedIndex = 0;
                    }
                    
                    if (nodes.Contains("Северодвинский_терминал"))
                    {
                        Lab5ToNode.SelectedItem = "Северодвинский_терминал";
                        Lab6ToNode.SelectedItem = "Северодвинский_терминал";
                    }
                    else
                    {
                        Lab5ToNode.SelectedIndex = nodes.Count > 1 ? nodes.Count - 1 : 0;
                        Lab6ToNode.SelectedIndex = nodes.Count > 1 ? nodes.Count - 1 : 0;
                    }
                }
                
                Lab4Results.Text = "✓ Граф загружен. Выберите операцию.";
                Lab5Results.Text = "✓ Взвешенный граф загружен. Выберите операцию.";
                Lab6Results.Text = "✓ Граф загружен. Выберите операцию.";
            }
            catch (Exception ex)
            {
                Lab4Results.Text = "Нажмите 'Загрузить граф' для начала работы.";
                Lab5Results.Text = "Нажмите 'Загрузить взвешенный граф' для начала работы.";
                Lab6Results.Text = "Сначала загрузите взвешенный граф в Лабораторной №5.";
                
                MessageBox.Show($"Ошибка автозагрузки: {ex.Message}", "Предупреждение", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // ===== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ =====

        private void CreateTestDataFiles()
        {
            // Создаем файл для Lab4
            graphFilePath = IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "pipeline_network.txt");
            var lines4 = new[]
            {
                "НС_Московская - Тверской_трубопровод",
                "НС_Московская - Рязанский_трубопровод",
                "Тверской_трубопровод - Калининский_узел",
                "Тверской_трубопровод - Волжский_узел",
                "Рязанский_трубопровод - Тульский_узел",
                "Рязанский_трубопровод - Коломенский_узел",
                "Калининский_узел - НС_Северная",
                "Волжский_узел - Ярославский_узел",
                "Тульский_узел - Орловский_узел",
                "Коломенский_узел - Владимирский_узел",
                "НС_Северная - Вологодский_узел",
                "Ярославский_узел - Вологодский_узел",
                "Орловский_узел - Курский_узел",
                "Владимирский_узел - Курский_узел",
                "Вологодский_узел - НС_Архангельская",
                "Курский_узел - НС_Архангельская",
                "НС_Архангельская - Северодвинский_терминал",
                "НС_Северная - Новгородский_узел",
                "Новгородский_узел - Псковский_узел",
                "Псковский_узел - Северодвинский_терминал"
            };
            File.WriteAllLines(graphFilePath, lines4);

            // Создаем файл для Lab5
            weightedGraphFilePath = IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "pipeline_weighted.txt");
            var lines5 = new[]
            {
                "НС_Московская - Тверской_трубопровод, 100",
                "НС_Московская - Рязанский_трубопровод, 80",
                "Тверской_трубопровод - Калининский_узел, 90",
                "Тверской_трубопровод - Волжский_узел, 70",
                "Рязанский_трубопровод - Тульский_узел, 85",
                "Рязанский_трубопровод - Коломенский_узел, 75",
                "Калининский_узел - НС_Северная, 95",
                "Волжский_узел - Ярославский_узел, 60",
                "Тульский_узел - Орловский_узел, 80",
                "Коломенский_узел - Владимирский_узел, 70",
                "НС_Северная - Вологодский_узел, 100",
                "Ярославский_узел - Вологодский_узел, 65",
                "Орловский_узел - Курский_узел, 75",
                "Владимирский_узел - Курский_узел, 80",
                "Вологодский_узел - НС_Архангельская, 90",
                "Курский_узел - НС_Архангельская, 85",
                "НС_Архангельская - Северодвинский_терминал, 95",
                "НС_Северная - Новгородский_узел, 50",
                "Новгородский_узел - Псковский_узел, 55",
                "Псковский_узел - Северодвинский_терминал, 60"
            };
            File.WriteAllLines(weightedGraphFilePath, lines5);
            
            // Путь к большому графу
            largeGraphFilePath = IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "pipeline_large.txt");
        }

        // ===== ЛАБОРАТОРНАЯ РАБОТА №4 =====

        private void LoadGraph_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Выберите файл графа",
                    Filter = "Текстовые файлы (*.txt)|*.txt|CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                    DefaultExt = ".txt"
                };

                if (dialog.ShowDialog() == true)
                {
                    graphFilePath = dialog.FileName;
                    lab4.LoadGraph(graphFilePath);
                    
                    var sb = new StringBuilder();
                    sb.AppendLine("✓ Граф успешно загружен!");
                    sb.AppendLine($"Файл: {IOPath.GetFileName(graphFilePath)}");
                    sb.AppendLine($"Количество узлов: {lab4.GetNodes().Count}");
                    sb.AppendLine();
                    sb.AppendLine("Структура графа:");
                    
                    // Выводим первые 10 узлов
                    var nodes = lab4.GetNodes();
                    for (int i = 0; i < Math.Min(10, nodes.Count); i++)
                    {
                        sb.AppendLine($"  • {nodes[i]}");
                    }
                    if (nodes.Count > 10)
                        sb.AppendLine($"  ... и еще {nodes.Count - 10} узлов");

                    Lab4Results.Text = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки графа: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private Dictionary<string, List<string>> GetGraphEdges()
        {
            var edges = new Dictionary<string, List<string>>();
            var lines = File.ReadAllLines(graphFilePath);
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split('-');
                if (parts.Length < 2) continue;
                
                string from = parts[0].Trim();
                string to = parts[1].Trim();
                
                if (!edges.ContainsKey(from))
                    edges[from] = new List<string>();
                if (!edges.ContainsKey(to))
                    edges[to] = new List<string>();
                    
                edges[from].Add(to);
            }
            
            return edges;
        }

        private void RunBFS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lab4.GetNodes().Count == 0)
                {
                    MessageBox.Show("Сначала загрузите граф!", "Предупреждение", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string startNode = "НС_Московская";
                
                // Замер времени выполнения
                var stopwatch = Stopwatch.StartNew();
                var result = lab4.BFS(startNode);
                stopwatch.Stop();

                var sb = new StringBuilder();
                sb.AppendLine($"BFS (обход в ширину) от узла '{startNode}':");
                sb.AppendLine();
                sb.AppendLine("Порядок посещения узлов:");
                for (int i = 0; i < result.Count; i++)
                {
                    sb.Append($"{i + 1}. {result[i]}");
                    if (i < result.Count - 1)
                        sb.Append(" → ");
                    if ((i + 1) % 3 == 0)
                        sb.AppendLine();
                }
                sb.AppendLine();
                sb.AppendLine($"\nВсего посещено узлов: {result.Count}");

                Lab4Results.Text = sb.ToString();
                Lab4Timing.Text = $"{stopwatch.Elapsed.TotalMilliseconds:F4} мс";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения BFS: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RunDFS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lab4.GetNodes().Count == 0)
                {
                    MessageBox.Show("Сначала загрузите граф!", "Предупреждение", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string startNode = "НС_Московская";
                
                // Замер времени выполнения
                var stopwatch = Stopwatch.StartNew();
                var result = lab4.DFS(startNode);
                stopwatch.Stop();

                var sb = new StringBuilder();
                sb.AppendLine($"DFS (обход в глубину) от узла '{startNode}':");
                sb.AppendLine();
                sb.AppendLine("Порядок посещения узлов:");
                for (int i = 0; i < result.Count; i++)
                {
                    sb.Append($"{i + 1}. {result[i]}");
                    if (i < result.Count - 1)
                        sb.Append(" → ");
                    if ((i + 1) % 3 == 0)
                        sb.AppendLine();
                }
                sb.AppendLine();
                sb.AppendLine($"\nВсего посещено узлов: {result.Count}");

                Lab4Results.Text = sb.ToString();
                Lab4Timing.Text = $"{stopwatch.Elapsed.TotalMilliseconds:F4} мс";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения DFS: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FindComponents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lab4.GetNodes().Count == 0)
                {
                    MessageBox.Show("Сначала загрузите граф!", "Предупреждение", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Замер времени выполнения
                var stopwatch = Stopwatch.StartNew();
                var components = lab4.FindConnectedComponents();
                stopwatch.Stop();

                var sb = new StringBuilder();
                sb.AppendLine("Компоненты связности:");
                sb.AppendLine();
                
                for (int i = 0; i < components.Count; i++)
                {
                    sb.AppendLine($"Компонента {i + 1} ({components[i].Count} узлов):");
                    foreach (var node in components[i])
                    {
                        sb.AppendLine($"  • {node}");
                    }
                    sb.AppendLine();
                }

                sb.AppendLine($"Всего компонент: {components.Count}");

                Lab4Results.Text = sb.ToString();
                Lab4Timing.Text = $"{stopwatch.Elapsed.TotalMilliseconds:F4} мс";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска компонент: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== ЛАБОРАТОРНАЯ РАБОТА №5 =====

        private void LoadWeightedGraph_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Выберите файл взвешенного графа",
                    Filter = "Текстовые файлы (*.txt)|*.txt|CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                    DefaultExt = ".txt"
                };

                if (dialog.ShowDialog() == true)
                {
                    weightedGraphFilePath = dialog.FileName;
                    lab5.LoadWeightedGraph(weightedGraphFilePath);
                    
                    // Создаем Lab6 с загруженными данными
                    lab6 = new Lab6(lab5.GetGraph(), lab5.GetNodes());

                    var sb = new StringBuilder();
                    sb.AppendLine("✓ Взвешенный граф успешно загружен!");
                    sb.AppendLine($"Файл: {IOPath.GetFileName(weightedGraphFilePath)}");
                    sb.AppendLine($"Количество узлов: {lab5.GetNodes().Count}");
                    sb.AppendLine();
                    
                    // Заполняем ComboBox списком узлов
                    var nodes = lab5.GetNodes();
                    
                    if (nodes != null && nodes.Count > 0)
                    {
                        var nodeList = new List<string>(nodes);
                        
                        Lab5FromNode.ItemsSource = nodeList;
                        Lab5ToNode.ItemsSource = nodeList;
                        Lab6FromNode.ItemsSource = nodeList;
                        Lab6ToNode.ItemsSource = nodeList;
                        
                        // Устанавливаем значения по умолчанию
                        Lab5FromNode.SelectedIndex = 0;
                        Lab6FromNode.SelectedIndex = 0;
                        Lab5ToNode.SelectedIndex = nodes.Count > 1 ? nodes.Count - 1 : 0;
                        Lab6ToNode.SelectedIndex = nodes.Count > 1 ? nodes.Count - 1 : 0;
                        
                        sb.AppendLine($"✓ ComboBox'ы заполнены {nodeList.Count} узлами.");
                        sb.AppendLine();
                        sb.AppendLine("Теперь доступны все функции Лабораторных №5 и №6");
                    }
                    else
                    {
                        sb.AppendLine("⚠ Предупреждение: список узлов пуст!");
                    }

                    Lab5Results.Text = sb.ToString();
                    Lab6Results.Text = "Граф загружен. Выберите операцию.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки графа: {ex.Message}\n\nStack trace:\n{ex.StackTrace}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RunDijkstra_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lab5.GetNodes().Count == 0)
                {
                    MessageBox.Show("Сначала загрузите взвешенный граф!", "Предупреждение", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string startNode = "НС_Московская";
                
                // Замер времени выполнения
                var stopwatch = Stopwatch.StartNew();
                var (distances, _) = lab5.Dijkstra(startNode);
                stopwatch.Stop();

                var sb = new StringBuilder();
                sb.AppendLine($"Алгоритм Дейкстры от узла '{startNode}':");
                sb.AppendLine();
                sb.AppendLine("Кратчайшие расстояния до всех узлов:");
                sb.AppendLine();

                foreach (var kvp in distances)
                {
                    if (kvp.Value == int.MaxValue)
                        sb.AppendLine($"  {kvp.Key}: недостижим");
                    else
                        sb.AppendLine($"  {kvp.Key}: {kvp.Value}");
                }

                Lab5Results.Text = sb.ToString();
                Lab5Timing.Text = $"{stopwatch.Elapsed.TotalMilliseconds:F4} мс";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения Дейкстры: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FindPath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lab5.GetNodes().Count == 0)
                {
                    MessageBox.Show("Сначала загрузите взвешенный граф!", "Предупреждение", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string from = Lab5FromNode.Text?.Trim() ?? "";
                string to = Lab5ToNode.Text?.Trim() ?? "";

                if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                {
                    MessageBox.Show("Выберите начальный и конечный узлы!", "Предупреждение", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Замер времени выполнения
                var stopwatch = Stopwatch.StartNew();
                var (distance, path) = lab5.FindShortestPath(from, to);
                stopwatch.Stop();

                var sb = new StringBuilder();
                sb.AppendLine($"Кратчайший путь от '{from}' до '{to}':");
                sb.AppendLine();

                if (path.Count == 0 || distance == int.MaxValue)
                {
                    sb.AppendLine("❌ Путь не найден (узлы не связаны)");
                }
                else
                {
                    sb.AppendLine($"✓ Расстояние: {distance}");
                    sb.AppendLine();
                    sb.AppendLine("Маршрут:");
                    sb.Append("  ");
                    for (int i = 0; i < path.Count; i++)
                    {
                        sb.Append(path[i]);
                        if (i < path.Count - 1)
                            sb.Append(" → ");
                    }
                    sb.AppendLine();
                }

                Lab5Results.Text = sb.ToString();
                Lab5Timing.Text = $"{stopwatch.Elapsed.TotalMilliseconds:F4} мс";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска пути: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== ЛАБОРАТОРНАЯ РАБОТА №6 =====

        private void FindArticulationPoints_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lab6 == null || lab5.GetNodes().Count == 0)
                {
                    MessageBox.Show("Сначала загрузите взвешенный граф в Лабораторной №5!", 
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Замер времени выполнения
                var stopwatch = Stopwatch.StartNew();
                var artPoints = lab6.FindArticulationPoints();
                stopwatch.Stop();

                var sb = new StringBuilder();
                sb.AppendLine("Точки сочленения (критические узлы):");
                sb.AppendLine();

                if (artPoints.Count == 0)
                {
                    sb.AppendLine("✓ Точек сочленения не найдено.");
                    sb.AppendLine("Граф устойчив - нет узлов, удаление которых разорвет сеть.");
                }
                else
                {
                    sb.AppendLine($"Найдено критических узлов: {artPoints.Count}");
                    sb.AppendLine();
                    foreach (var ap in artPoints)
                    {
                        sb.AppendLine($"  ⚠ {ap}");
                    }
                    sb.AppendLine();
                    sb.AppendLine("Удаление этих узлов приведет к разрыву сети!");
                }

                Lab6Results.Text = sb.ToString();
                Lab6Timing.Text = $"{stopwatch.Elapsed.TotalMilliseconds:F4} мс";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска точек сочленения: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BuildMST_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lab6 == null || lab5.GetNodes().Count == 0)
                {
                    MessageBox.Show("Сначала загрузите взвешенный граф в Лабораторной №5!", 
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Замер времени выполнения
                var stopwatch = Stopwatch.StartNew();
                var mst = lab6.PrimMST();
                stopwatch.Stop();

                var sb = new StringBuilder();
                sb.AppendLine("Минимальное остовное дерево (алгоритм Прима):");
                sb.AppendLine();

                int totalWeight = 0;
                sb.AppendLine("Рёбра МОД:");
                foreach (var (from, to, weight) in mst)
                {
                    sb.AppendLine($"  {from} ↔ {to} : {weight}");
                    totalWeight += weight;
                }

                sb.AppendLine();
                sb.AppendLine($"✓ Суммарный вес МОД: {totalWeight}");
                sb.AppendLine($"✓ Количество рёбер: {mst.Count}");

                Lab6Results.Text = sb.ToString();
                Lab6Timing.Text = $"{stopwatch.Elapsed.TotalMilliseconds:F4} мс";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка построения МОД: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FindOptimalPath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lab6 == null || lab5.GetNodes().Count == 0)
                {
                    MessageBox.Show("Сначала загрузите взвешенный граф в Лабораторной №5!", 
                        "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string from = Lab6FromNode.Text?.Trim() ?? "";
                string to = Lab6ToNode.Text?.Trim() ?? "";

                if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
                {
                    MessageBox.Show("Выберите узлы источника и назначения!", "Предупреждение", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Замер времени выполнения
                var stopwatch = Stopwatch.StartNew();
                var (maxFlow, path) = lab6.FindOptimalTransportPath(from, to);
                stopwatch.Stop();

                var sb = new StringBuilder();
                sb.AppendLine($"Оптимальный путь транспортировки от '{from}' до '{to}':");
                sb.AppendLine();

                if (maxFlow == 0 || path.Count == 0)
                {
                    sb.AppendLine("❌ Путь не найден");
                }
                else
                {
                    sb.AppendLine($"✓ Максимальная пропускная способность: {maxFlow}");
                    sb.AppendLine();
                    sb.AppendLine("Маршрут:");
                    sb.Append("  ");
                    for (int i = 0; i < path.Count; i++)
                    {
                        sb.Append(path[i]);
                        if (i < path.Count - 1)
                            sb.Append(" → ");
                    }
                    sb.AppendLine();
                }

                Lab6Results.Text = sb.ToString();
                Lab6Timing.Text = $"{stopwatch.Elapsed.TotalMilliseconds:F4} мс";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка поиска оптимального пути: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===== ЭКСПЕРИМЕНТ =====

        private void LoadLargeGraph_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    Title = "Выберите файл большого графа для эксперимента",
                    Filter = "Текстовые файлы (*.txt)|*.txt|CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*",
                    DefaultExt = ".txt"
                };

                if (dialog.ShowDialog() == true)
                {
                    largeGraphFilePath = dialog.FileName;

                    // Загружаем взвешенный граф
                    lab5Large.LoadWeightedGraph(largeGraphFilePath);
                    
                    // Создаем невзвешенную версию для lab4Large
                    var lines = File.ReadAllLines(largeGraphFilePath);
                    var unweightedLines = new List<string>();
                    foreach (var line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        var parts = line.Split(',');
                        if (parts.Length > 0)
                        {
                            unweightedLines.Add(parts[0].Trim());
                        }
                    }
                    
                    var tempFile = IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "pipeline_large_unweighted.txt");
                    File.WriteAllLines(tempFile, unweightedLines);
                    lab4Large.LoadGraph(tempFile);
                    
                    var sb = new StringBuilder();
                    sb.AppendLine("✓ Большой граф успешно загружен!");
                    sb.AppendLine($"Файл: {IOPath.GetFileName(largeGraphFilePath)}");
                    sb.AppendLine($"Количество узлов: {lab5Large.GetNodes().Count}");
                    sb.AppendLine();
                    sb.AppendLine("Граф готов для эксперимента.");
                    sb.AppendLine("Нажмите 'Запустить эксперимент' для замера времени выполнения алгоритмов.");

                    ExperimentResults.Text = sb.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки большого графа: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RunExperiment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lab5Large.GetNodes().Count == 0)
                {
                    MessageBox.Show("Сначала загрузите большой граф!", "Предупреждение", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine("ЭКСПЕРИМЕНТ: Анализ производительности алгоритмов");
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine();
                sb.AppendLine($"Граф: {lab5Large.GetNodes().Count} вершин");
                sb.AppendLine();

                var totalStopwatch = Stopwatch.StartNew();
                var nodes = lab5Large.GetNodes();
                string startNode = nodes.Count > 0 ? nodes[0] : "НС_Московская";

                // Тест 1: BFS
                sb.AppendLine("1. BFS (обход в ширину)");
                var sw1 = Stopwatch.StartNew();
                var bfsResult = lab4Large.BFS(startNode);
                sw1.Stop();
                sb.AppendLine($"   Время: {sw1.Elapsed.TotalMilliseconds:F4} мс");
                sb.AppendLine($"   Посещено узлов: {bfsResult.Count}");
                sb.AppendLine();

                // Тест 2: DFS
                sb.AppendLine("2. DFS (обход в глубину)");
                var sw2 = Stopwatch.StartNew();
                var dfsResult = lab4Large.DFS(startNode);
                sw2.Stop();
                sb.AppendLine($"   Время: {sw2.Elapsed.TotalMilliseconds:F4} мс");
                sb.AppendLine($"   Посещено узлов: {dfsResult.Count}");
                sb.AppendLine();

                // Тест 3: Дейкстра
                sb.AppendLine("3. Алгоритм Дейкстры");
                var sw3 = Stopwatch.StartNew();
                var (distances, _) = lab5Large.Dijkstra(startNode);
                sw3.Stop();
                sb.AppendLine($"   Время: {sw3.Elapsed.TotalMilliseconds:F4} мс");
                sb.AppendLine($"   Обработано узлов: {distances.Count}");
                sb.AppendLine();

                // Тест 4: Поиск пути
                if (nodes.Count >= 2)
                {
                    string endNode = nodes[nodes.Count - 1];
                    sb.AppendLine($"4. Поиск кратчайшего пути ({startNode} → {endNode})");
                    var sw4 = Stopwatch.StartNew();
                    var (distance, path) = lab5Large.FindShortestPath(startNode, endNode);
                    sw4.Stop();
                    sb.AppendLine($"   Время: {sw4.Elapsed.TotalMilliseconds:F4} мс");
                    sb.AppendLine($"   Расстояние: {distance}");
                    sb.AppendLine($"   Длина пути: {path.Count} узлов");
                    sb.AppendLine();
                }

                totalStopwatch.Stop();
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine($"Общее время эксперимента: {totalStopwatch.Elapsed.TotalMilliseconds:F4} мс");
                sb.AppendLine("═══════════════════════════════════════════════════════");

                ExperimentResults.Text = sb.ToString();
                ExperimentTiming.Text = $"{totalStopwatch.Elapsed.TotalMilliseconds:F4} мс";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения эксперимента: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CompareGraphs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lab5.GetNodes().Count == 0 || lab5Large.GetNodes().Count == 0)
                {
                    MessageBox.Show("Загрузите оба графа перед сравнением!", "Предупреждение", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine("СРАВНЕНИЕ ПРОИЗВОДИТЕЛЬНОСТИ");
                sb.AppendLine("═══════════════════════════════════════════════════════");
                sb.AppendLine();

                // Малый граф
                sb.AppendLine("МАЛЫЙ ГРАФ:");
                sb.AppendLine($"  Вершин: {lab5.GetNodes().Count}");
                var smallNodes = lab5.GetNodes();
                string smallStart = smallNodes.Count > 0 ? smallNodes[0] : "НС_Московская";

                var sw1 = Stopwatch.StartNew();
                lab4.BFS(smallStart);
                sw1.Stop();
                double smallBFS = sw1.Elapsed.TotalMilliseconds;

                var sw2 = Stopwatch.StartNew();
                lab4.DFS(smallStart);
                sw2.Stop();
                double smallDFS = sw2.Elapsed.TotalMilliseconds;

                var sw3 = Stopwatch.StartNew();
                lab5.Dijkstra(smallStart);
                sw3.Stop();
                double smallDijkstra = sw3.Elapsed.TotalMilliseconds;

                sb.AppendLine($"  BFS: {smallBFS:F4} мс");
                sb.AppendLine($"  DFS: {smallDFS:F4} мс");
                sb.AppendLine($"  Дейкстра: {smallDijkstra:F4} мс");
                sb.AppendLine();

                // Большой граф
                sb.AppendLine("БОЛЬШОЙ ГРАФ:");
                sb.AppendLine($"  Вершин: {lab5Large.GetNodes().Count}");
                var largeNodes = lab5Large.GetNodes();
                string largeStart = largeNodes.Count > 0 ? largeNodes[0] : "НС_Московская";

                var sw4 = Stopwatch.StartNew();
                lab4Large.BFS(largeStart);
                sw4.Stop();
                double largeBFS = sw4.Elapsed.TotalMilliseconds;

                var sw5 = Stopwatch.StartNew();
                lab4Large.DFS(largeStart);
                sw5.Stop();
                double largeDFS = sw5.Elapsed.TotalMilliseconds;

                var sw6 = Stopwatch.StartNew();
                lab5Large.Dijkstra(largeStart);
                sw6.Stop();
                double largeDijkstra = sw6.Elapsed.TotalMilliseconds;

                sb.AppendLine($"  BFS: {largeBFS:F4} мс");
                sb.AppendLine($"  DFS: {largeDFS:F4} мс");
                sb.AppendLine($"  Дейкстра: {largeDijkstra:F4} мс");
                sb.AppendLine();

                // Сравнение
                sb.AppendLine("РОСТ ВРЕМЕНИ ВЫПОЛНЕНИЯ:");
                sb.AppendLine($"  BFS: {(largeBFS / smallBFS):F2}x медленнее");
                sb.AppendLine($"  DFS: {(largeDFS / smallDFS):F2}x медленнее");
                sb.AppendLine($"  Дейкстра: {(largeDijkstra / smallDijkstra):F2}x медленнее");
                sb.AppendLine();
                sb.AppendLine("═══════════════════════════════════════════════════════");

                ExperimentResults.Text = sb.ToString();
                ExperimentTiming.Text = $"Сравнение завершено";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сравнения: {ex.Message}", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
