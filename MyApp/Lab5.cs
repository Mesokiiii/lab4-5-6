using System;
using System.Collections.Generic;
using System.IO;

namespace MyApp
{
    // Лабораторная работа №5: Взвешенный граф. Алгоритм Дейкстры
    public class Lab5
    {
        // Граф с весами: узел -> список (сосед, вес ребра)
        private Dictionary<string, List<(string neighbor, int weight)>> graph;
        
        // Список всех узлов
        private List<string> nodes;

        public Lab5()
        {
            graph = new Dictionary<string, List<(string, int)>>();
            nodes = new List<string>();
        }

        // Загрузка взвешенного графа из файла
        // Формат: "Узел1 - Узел2, 100"
        public void LoadWeightedGraph(string filename)
        {
            graph.Clear();
            nodes.Clear();

            string[] lines = File.ReadAllLines(filename);
            
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) 
                    continue;

                // Разделяем по "-" и ","
                string[] parts = line.Split(new[] { '-', ',' });
                if (parts.Length < 3) 
                    continue;

                string from = parts[0].Trim();
                string to = parts[1].Trim();
                int weight = int.Parse(parts[2].Trim());

                // Добавляем узлы если их нет
                if (!graph.ContainsKey(from))
                {
                    graph[from] = new List<(string, int)>();
                    nodes.Add(from);
                }
                if (!graph.ContainsKey(to))
                {
                    graph[to] = new List<(string, int)>();
                    nodes.Add(to);
                }

                // Добавляем ребра в обе стороны (неориентированный граф)
                graph[from].Add((to, weight));
                graph[to].Add((from, weight));
            }
        }

        // Алгоритм Дейкстры - поиск кратчайших путей от одной вершины до всех остальных
        // Возвращает: расстояния до всех узлов и предыдущие узлы для восстановления пути
        public (Dictionary<string, int> distances, Dictionary<string, string> previous) Dijkstra(string start)
        {
            // Расстояния от стартового узла до всех остальных
            Dictionary<string, int> distances = new Dictionary<string, int>();
            
            // Предыдущий узел на кратчайшем пути (для восстановления маршрута)
            Dictionary<string, string> previous = new Dictionary<string, string>();
            
            // Множество непосещенных узлов
            HashSet<string> unvisited = new HashSet<string>();

            // Инициализация: все расстояния = бесконечность
            foreach (string node in nodes)
            {
                distances[node] = int.MaxValue;
                unvisited.Add(node);
            }
            
            // Расстояние до стартового узла = 0
            distances[start] = 0;

            // Пока есть непосещенные узлы
            while (unvisited.Count > 0)
            {
                // Шаг 1: Найти непосещенный узел с минимальным расстоянием
                string current = null;
                int minDistance = int.MaxValue;
                
                foreach (string node in unvisited)
                {
                    if (distances[node] < minDistance)
                    {
                        minDistance = distances[node];
                        current = node;
                    }
                }

                // Если не нашли доступных узлов - выходим
                if (current == null || distances[current] == int.MaxValue)
                    break;

                // Шаг 2: Помечаем текущий узел как посещенный
                unvisited.Remove(current);

                // Шаг 3: Обновляем расстояния до соседей
                if (graph.ContainsKey(current))
                {
                    foreach (var (neighbor, weight) in graph[current])
                    {
                        // Новое возможное расстояние через текущий узел
                        int newDistance = distances[current] + weight;
                        
                        // Если нашли более короткий путь - обновляем
                        if (newDistance < distances[neighbor])
                        {
                            distances[neighbor] = newDistance;
                            previous[neighbor] = current;
                        }
                    }
                }
            }

            return (distances, previous);
        }

        // Восстановление пути от start до end
        public List<string> GetPath(Dictionary<string, string> previous, string start, string end)
        {
            List<string> path = new List<string>();
            string current = end;

            // Идем от конца к началу по цепочке предыдущих узлов
            while (current != null)
            {
                path.Add(current);
                
                // Если дошли до начала - останавливаемся
                if (current == start) 
                    break;
                
                // Переходим к предыдущему узлу
                if (previous.ContainsKey(current))
                    current = previous[current];
                else
                    current = null;
            }

            // Разворачиваем путь (был от конца к началу)
            path.Reverse();
            
            // Проверяем что путь начинается с start
            if (path.Count > 0 && path[0] == start)
                return path;
            else
                return new List<string>(); // Путь не найден
        }

        // Найти кратчайший путь между двумя узлами
        public (int distance, List<string> path) FindShortestPath(string from, string to)
        {
            // Запускаем алгоритм Дейкстры
            var (distances, previous) = Dijkstra(from);
            
            // Восстанавливаем путь
            List<string> path = GetPath(previous, from, to);
            
            // Получаем расстояние
            int distance = distances.ContainsKey(to) ? distances[to] : -1;
            
            return (distance, path);
        }

        // Получить список всех узлов
        public List<string> GetNodes()
        {
            return nodes;
        }

        // Вывести взвешенный граф на экран
        public void PrintWeightedGraph()
        {
            Console.WriteLine("Взвешенный граф трубопроводной сети:");
            foreach (var node in graph)
            {
                Console.WriteLine($"{node.Key}:");
                foreach (var (neighbor, weight) in node.Value)
                {
                    Console.WriteLine($"  -> {neighbor} (пропускная способность: {weight})");
                }
            }
        }

        // Получить граф (для Lab6)
        public Dictionary<string, List<(string, int)>> GetGraph()
        {
            return graph;
        }
    }
}
