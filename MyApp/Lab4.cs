using System;
using System.Collections.Generic;
using System.IO;

namespace MyApp
{
    // Лабораторная работа №4: Построение графа. Алгоритмы обхода
    public class Lab4
    {
        // Граф хранится как словарь: узел -> список соседей
        private Dictionary<string, List<string>> graph;
        
        // Список всех узлов
        private List<string> nodes;

        public Lab4()
        {
            graph = new Dictionary<string, List<string>>();
            nodes = new List<string>();
        }

        // Загрузка графа из текстового файла
        // Формат: "Узел1 - Узел2"
        public void LoadGraph(string filename)
        {
            graph.Clear();
            nodes.Clear();

            // Читаем файл построчно
            string[] lines = File.ReadAllLines(filename);
            
            foreach (string line in lines)
            {
                // Пропускаем пустые строки
                if (string.IsNullOrWhiteSpace(line)) 
                    continue;

                // Разделяем строку по символу "-"
                string[] parts = line.Split('-');
                if (parts.Length < 2) 
                    continue;

                string from = parts[0].Trim();
                string to = parts[1].Trim();

                // Добавляем узел "from" если его еще нет
                if (!graph.ContainsKey(from))
                {
                    graph[from] = new List<string>();
                    nodes.Add(from);
                }
                
                // Добавляем узел "to" если его еще нет
                if (!graph.ContainsKey(to))
                {
                    graph[to] = new List<string>();
                    nodes.Add(to);
                }

                // Добавляем связи в обе стороны (неориентированный граф)
                graph[from].Add(to);
                graph[to].Add(from);
            }
        }

        // BFS - обход в ширину (Breadth-First Search)
        // Обходит граф уровень за уровнем, используя очередь
        public List<string> BFS(string start)
        {
            // Множество посещенных узлов
            HashSet<string> visited = new HashSet<string>();
            
            // Очередь для обхода
            Queue<string> queue = new Queue<string>();
            
            // Результат - порядок посещения узлов
            List<string> result = new List<string>();

            // Начинаем со стартового узла
            queue.Enqueue(start);
            visited.Add(start);

            // Пока очередь не пуста
            while (queue.Count > 0)
            {
                // Берем узел из начала очереди
                string current = queue.Dequeue();
                result.Add(current);

                // Проверяем всех соседей текущего узла
                if (graph.ContainsKey(current))
                {
                    foreach (string neighbor in graph[current])
                    {
                        // Если сосед еще не посещен
                        if (!visited.Contains(neighbor))
                        {
                            visited.Add(neighbor);
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            return result;
        }

        // DFS - обход в глубину (Depth-First Search)
        // Идет как можно глубже по одной ветке
        public List<string> DFS(string start)
        {
            HashSet<string> visited = new HashSet<string>();
            List<string> result = new List<string>();
            
            // Запускаем рекурсивный обход
            DFSRecursive(start, visited, result);
            
            return result;
        }

        // Вспомогательная рекурсивная функция для DFS
        private void DFSRecursive(string node, HashSet<string> visited, List<string> result)
        {
            // Отмечаем узел как посещенный
            visited.Add(node);
            result.Add(node);

            // Рекурсивно посещаем всех непосещенных соседей
            if (graph.ContainsKey(node))
            {
                foreach (string neighbor in graph[node])
                {
                    if (!visited.Contains(neighbor))
                    {
                        DFSRecursive(neighbor, visited, result);
                    }
                }
            }
        }

        // Проверка: достижима ли вершина B из вершины A
        public bool IsReachable(string from, string to)
        {
            // Используем BFS для поиска всех достижимых узлов
            List<string> reachable = BFS(from);
            return reachable.Contains(to);
        }

        // Поиск компонент связности
        // Компонента связности - группа узлов, между которыми есть пути
        public List<List<string>> FindConnectedComponents()
        {
            HashSet<string> visited = new HashSet<string>();
            List<List<string>> components = new List<List<string>>();

            // Проходим по всем узлам
            foreach (string node in nodes)
            {
                // Если узел еще не посещен, начинаем новую компоненту
                if (!visited.Contains(node))
                {
                    // BFS найдет все узлы в этой компоненте
                    List<string> component = BFS(node);
                    components.Add(component);
                    
                    // Отмечаем все узлы компоненты как посещенные
                    foreach (string n in component)
                    {
                        visited.Add(n);
                    }
                }
            }

            return components;
        }

        // Получить список всех узлов
        public List<string> GetNodes()
        {
            return nodes;
        }

        // Вывести граф на экран
        public void PrintGraph()
        {
            Console.WriteLine("Граф трубопроводной сети:");
            foreach (var node in graph)
            {
                Console.WriteLine($"{node.Key}: {string.Join(", ", node.Value)}");
            }
        }
    }
}
