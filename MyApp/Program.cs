using System;
using System.IO;

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Анализ трубопроводной сети ===\n");

            // Создаем тестовый файл с данными
            CreateTestData();

            // Лабораторная работа №4
            Console.WriteLine("--- Лабораторная работа №4: Обход графа ---");
            var lab4 = new Lab4();
            lab4.LoadGraph("pipeline_network.txt");
            lab4.PrintGraph();

            Console.WriteLine("\nBFS от узла 'НС_Московская':");
            var bfs = lab4.BFS("НС_Московская");
            Console.WriteLine(string.Join(" -> ", bfs));

            Console.WriteLine("\nDFS от узла 'НС_Московская':");
            var dfs = lab4.DFS("НС_Московская");
            Console.WriteLine(string.Join(" -> ", dfs));

            Console.WriteLine("\nДостижимость 'Северодвинский_терминал' из 'НС_Московская': " + 
                lab4.IsReachable("НС_Московская", "Северодвинский_терминал"));

            Console.WriteLine("\nКомпоненты связности:");
            var components = lab4.FindConnectedComponents();
            for (int i = 0; i < components.Count; i++)
            {
                Console.WriteLine($"Компонента {i + 1}: {string.Join(", ", components[i])}");
            }

            // Лабораторная работа №5
            Console.WriteLine("\n\n--- Лабораторная работа №5: Алгоритм Дейкстры ---");
            var lab5 = new Lab5();
            lab5.LoadWeightedGraph("pipeline_weighted.txt");

            var (distance, path) = lab5.FindShortestPath("НС_Московская", "Северодвинский_терминал");
            Console.WriteLine($"\nКратчайший путь от 'НС_Московская' до 'Северодвинский_терминал':");
            Console.WriteLine($"Расстояние: {distance}");
            Console.WriteLine($"Путь: {string.Join(" -> ", path)}");

            var (distances, _) = lab5.Dijkstra("НС_Московская");
            Console.WriteLine("\nРасстояния от 'НС_Московская' до всех узлов:");
            foreach (var kvp in distances)
            {
                if (kvp.Value != int.MaxValue)
                    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
            }

            // Лабораторная работа №6
            Console.WriteLine("\n\n--- Лабораторная работа №6: Дополнительный анализ ---");
            var graphData = GetGraphData(lab5);
            var lab6 = new Lab6(graphData.Item1, graphData.Item2);

            Console.WriteLine("\nТочки сочленения (критические узлы):");
            var artPoints = lab6.FindArticulationPoints();
            foreach (var ap in artPoints)
            {
                Console.WriteLine($"  - {ap}");
            }

            Console.WriteLine("\nМинимальное остовное дерево:");
            var mst = lab6.PrimMST();
            int totalWeight = 0;
            foreach (var (from, to, weight) in mst)
            {
                Console.WriteLine($"  {from} - {to}: {weight}");
                totalWeight += weight;
            }
            Console.WriteLine($"Суммарный вес: {totalWeight}");

            Console.WriteLine("\nОптимальный путь транспортировки (максимальная пропускная способность):");
            var (maxFlow, optPath) = lab6.FindOptimalTransportPath("НС_Московская", "Северодвинский_терминал");
            Console.WriteLine($"Максимальная пропускная способность: {maxFlow}");
            Console.WriteLine($"Путь: {string.Join(" -> ", optPath)}");

            Console.WriteLine("\n\nАнализ завершен!");
        }

        static void CreateTestData()
        {
            // Создаем файл для Lab4 (без весов)
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
            File.WriteAllLines("pipeline_network.txt", lines4);

            // Создаем файл для Lab5 (с весами - пропускная способность)
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
            File.WriteAllLines("pipeline_weighted.txt", lines5);
        }

        static (Dictionary<string, List<(string, int)>>, List<string>) GetGraphData(Lab5 lab5)
        {
            // Получаем граф и узлы из Lab5
            Dictionary<string, List<(string, int)>> graph = lab5.GetGraph();
            List<string> nodes = lab5.GetNodes();
            
            return (graph, nodes);
        }
    }
}
