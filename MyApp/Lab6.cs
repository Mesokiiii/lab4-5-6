using System;
using System.Collections.Generic;

namespace MyApp
{
    // Лабораторная работа №6: Дополнительный анализ графа
    public class Lab6
    {
        // Граф с весами
        private Dictionary<string, List<(string neighbor, int weight)>> graph;
        
        // Список всех узлов
        private List<string> nodes;

        public Lab6(Dictionary<string, List<(string, int)>> graph, List<string> nodes)
        {
            this.graph = graph;
            this.nodes = nodes;
        }

        // ===== ЗАДАЧА 1: Поиск точек сочленения =====
        // Точка сочленения - узел, удаление которого разрывает граф на части
        public List<string> FindArticulationPoints()
        {
            HashSet<string> visited = new HashSet<string>();
            
            // disc[узел] - время обнаружения узла
            Dictionary<string, int> disc = new Dictionary<string, int>();
            
            // low[узел] - минимальное время обнаружения, достижимое из поддерева узла
            Dictionary<string, int> low = new Dictionary<string, int>();
            
            // parent[узел] - родитель узла в DFS дереве
            Dictionary<string, string> parent = new Dictionary<string, string>();
            
            // Множество точек сочленения
            HashSet<string> articulationPoints = new HashSet<string>();
            
            int time = 0;

            // Запускаем DFS для каждой компоненты связности
            foreach (string node in nodes)
            {
                if (!visited.Contains(node))
                {
                    FindAPUtil(node, visited, disc, low, parent, articulationPoints, ref time);
                }
            }

            return new List<string>(articulationPoints);
        }

        // Вспомогательная функция для поиска точек сочленения
        private void FindAPUtil(string u, HashSet<string> visited, 
            Dictionary<string, int> disc, Dictionary<string, int> low,
            Dictionary<string, string> parent, HashSet<string> ap, ref int time)
        {
            int children = 0; // Количество детей в DFS дереве
            visited.Add(u);
            
            // Инициализируем время обнаружения и low
            disc[u] = low[u] = ++time;

            // Проходим по всем соседям
            if (graph.ContainsKey(u))
            {
                foreach (var (v, _) in graph[u])
                {
                    // Если v не посещен
                    if (!visited.Contains(v))
                    {
                        children++;
                        parent[v] = u;
                        
                        // Рекурсивный вызов
                        FindAPUtil(v, visited, disc, low, parent, ap, ref time);

                        // Обновляем low[u]
                        low[u] = Math.Min(low[u], low[v]);

                        // Случай 1: u - корень DFS дерева и имеет 2+ детей
                        if (!parent.ContainsKey(u) && children > 1)
                            ap.Add(u);

                        // Случай 2: u не корень и low[v] >= disc[u]
                        if (parent.ContainsKey(u) && low[v] >= disc[u])
                            ap.Add(u);
                    }
                    // Обратное ребро (не к родителю)
                    else if (parent.ContainsKey(u) && v != parent[u])
                    {
                        low[u] = Math.Min(low[u], disc[v]);
                    }
                }
            }
        }

        // ===== ЗАДАЧА 2: Минимальное остовное дерево (алгоритм Прима) =====
        // МОД - подграф, соединяющий все вершины с минимальной суммой весов
        public List<(string from, string to, int weight)> PrimMST()
        {
            if (nodes.Count == 0) 
                return new List<(string, string, int)>();

            // Результат - список ребер МОД
            List<(string, string, int)> mst = new List<(string, string, int)>();
            
            // Множество узлов, уже включенных в МОД
            HashSet<string> inMST = new HashSet<string>();
            
            // Куча (приоритетная очередь) ребер: (вес, откуда, куда)
            SortedSet<(int weight, string from, string to)> minHeap = new SortedSet<(int, string, string)>();

            // Начинаем с первого узла
            string start = nodes[0];
            inMST.Add(start);

            // Добавляем все ребра из стартового узла в кучу
            if (graph.ContainsKey(start))
            {
                foreach (var (neighbor, weight) in graph[start])
                {
                    minHeap.Add((weight, start, neighbor));
                }
            }

            // Пока не включили все узлы
            while (minHeap.Count > 0 && inMST.Count < nodes.Count)
            {
                // Берем ребро с минимальным весом
                var (weight, from, to) = minHeap.Min;
                minHeap.Remove(minHeap.Min);

                // Если узел "to" уже в МОД - пропускаем
                if (inMST.Contains(to)) 
                    continue;

                // Добавляем ребро в МОД
                mst.Add((from, to, weight));
                inMST.Add(to);

                // Добавляем все ребра из нового узла в кучу
                if (graph.ContainsKey(to))
                {
                    foreach (var (neighbor, w) in graph[to])
                    {
                        if (!inMST.Contains(neighbor))
                        {
                            minHeap.Add((w, to, neighbor));
                        }
                    }
                }
            }

            return mst;
        }

        // ===== ЗАДАЧА 3 (по варианту): Оптимальный путь транспортировки =====
        // Находим путь с максимальной пропускной способностью
        // Используем модифицированный алгоритм Дейкстры
        public (int maxFlow, List<string> path) FindOptimalTransportPath(string source, string destination)
        {
            // capacity[узел] - максимальная пропускная способность до узла
            Dictionary<string, int> capacity = new Dictionary<string, int>();
            
            // previous[узел] - предыдущий узел на пути
            Dictionary<string, string> previous = new Dictionary<string, string>();
            
            // Множество непосещенных узлов
            HashSet<string> unvisited = new HashSet<string>();

            // Инициализация: все пропускные способности = 0
            foreach (string node in nodes)
            {
                capacity[node] = 0;
                unvisited.Add(node);
            }
            
            // Пропускная способность источника = бесконечность
            capacity[source] = int.MaxValue;

            // Пока есть непосещенные узлы
            while (unvisited.Count > 0)
            {
                // Находим узел с максимальной пропускной способностью
                string current = null;
                int maxCapacity = 0;
                
                foreach (string node in unvisited)
                {
                    if (capacity[node] > maxCapacity)
                    {
                        maxCapacity = capacity[node];
                        current = node;
                    }
                }

                // Если не нашли доступных узлов - выходим
                if (current == null || capacity[current] == 0)
                    break;

                unvisited.Remove(current);

                // Обновляем пропускную способность соседей
                if (graph.ContainsKey(current))
                {
                    foreach (var (neighbor, weight) in graph[current])
                    {
                        // Пропускная способность = минимум на пути
                        int newCapacity = Math.Min(capacity[current], weight);
                        
                        // Если нашли путь с большей пропускной способностью
                        if (newCapacity > capacity[neighbor])
                        {
                            capacity[neighbor] = newCapacity;
                            previous[neighbor] = current;
                        }
                    }
                }
            }

            // Восстанавливаем путь
            List<string> path = new List<string>();
            string curr = destination;
            
            while (curr != null)
            {
                path.Add(curr);
                if (curr == source) 
                    break;
                
                if (previous.ContainsKey(curr))
                    curr = previous[curr];
                else
                    curr = null;
            }
            
            path.Reverse();

            // Проверяем что узел назначения существует
            int finalCapacity = capacity.ContainsKey(destination) ? capacity[destination] : 0;
            
            return (finalCapacity, path);
        }
    }
}
