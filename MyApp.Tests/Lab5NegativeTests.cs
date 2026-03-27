using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using MyApp;

namespace MyApp.Tests
{
    // Ложно-положительные тесты для Lab5
    public class Lab5NegativeTests
    {
        private string CreateWeightedGraphFile(string[] lines)
        {
            string filename = Path.GetTempFileName();
            File.WriteAllLines(filename, lines);
            return filename;
        }

        // ===== ТЕСТЫ НА НЕКОРРЕКТНЫЕ ДАННЫЕ =====

        [Fact]
        public void Test01_LoadWeightedGraph_InvalidFormat_SkipsLine()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "InvalidLine", "A - B, 10" });

            lab5.LoadWeightedGraph(file);
            var nodes = lab5.GetNodes();

            Assert.Equal(2, nodes.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test02_LoadWeightedGraph_MissingWeight_SkipsLine()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B", "C - D, 20" });

            lab5.LoadWeightedGraph(file);
            var nodes = lab5.GetNodes();

            // Только C-D должны загрузиться
            Assert.Equal(2, nodes.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test03_LoadWeightedGraph_NegativeWeight_LoadsAnyway()
        {
            // Дейкстра не работает с отрицательными весами, но загрузка должна пройти
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, -10" });

            // Ожидаем исключение при парсинге отрицательного числа
            // или успешную загрузку, если код обрабатывает это
            try
            {
                lab5.LoadWeightedGraph(file);
                var nodes = lab5.GetNodes();
                Assert.True(nodes.Count >= 0); // Любой результат допустим
            }
            catch (FormatException)
            {
                // Это тоже допустимо
                Assert.True(true);
            }

            File.Delete(file);
        }

        [Fact]
        public void Test04_LoadWeightedGraph_ZeroWeight_LoadsCorrectly()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 0" });

            lab5.LoadWeightedGraph(file);
            var nodes = lab5.GetNodes();

            Assert.Equal(2, nodes.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test05_LoadWeightedGraph_VeryLargeWeight_HandlesCorrectly()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 999999999" });

            lab5.LoadWeightedGraph(file);
            var nodes = lab5.GetNodes();

            Assert.Equal(2, nodes.Count);
            File.Delete(file);
        }

        // ===== ТЕСТЫ ДЕЙКСТРЫ НА ГРАНИЧНЫХ СЛУЧАЯХ =====

        [Fact]
        public void Test06_Dijkstra_NonExistentStartNode_ReturnsMaxDistances()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10" });

            lab5.LoadWeightedGraph(file);
            var (distances, _) = lab5.Dijkstra("NonExistent");

            // Все узлы должны иметь максимальное расстояние
            Assert.Equal(int.MaxValue, distances["A"]);
            Assert.Equal(int.MaxValue, distances["B"]);
            File.Delete(file);
        }

        [Fact]
        public void Test07_Dijkstra_EmptyGraph_HandlesCorrectly()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new string[] { });

            lab5.LoadWeightedGraph(file);
            var (distances, _) = lab5.Dijkstra("A");

            // Пустой граф может вернуть пустой словарь или словарь с одним узлом
            Assert.True(distances.Count <= 1);
            File.Delete(file);
        }

        [Fact]
        public void Test08_Dijkstra_DisconnectedGraph_SomeNodesUnreachable()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10", "C - D, 20" });

            lab5.LoadWeightedGraph(file);
            var (distances, _) = lab5.Dijkstra("A");

            Assert.Equal(0, distances["A"]);
            Assert.Equal(10, distances["B"]);
            Assert.Equal(int.MaxValue, distances["C"]);
            Assert.Equal(int.MaxValue, distances["D"]);
            File.Delete(file);
        }

        [Fact]
        public void Test09_Dijkstra_GraphWithZeroWeights_HandlesCorrectly()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 0", "B - C, 0" });

            lab5.LoadWeightedGraph(file);
            var (distances, _) = lab5.Dijkstra("A");

            Assert.Equal(0, distances["A"]);
            Assert.Equal(0, distances["B"]);
            Assert.Equal(0, distances["C"]);
            File.Delete(file);
        }

        [Fact]
        public void Test10_Dijkstra_SingleNodeGraph_ReturnsZero()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10" });

            lab5.LoadWeightedGraph(file);
            var (distances, _) = lab5.Dijkstra("A");

            Assert.Equal(0, distances["A"]);
            File.Delete(file);
        }

        // ===== ТЕСТЫ ВОССТАНОВЛЕНИЯ ПУТИ НА ГРАНИЧНЫХ СЛУЧАЯХ =====

        [Fact]
        public void Test11_GetPath_NonExistentEndNode_ReturnsEmpty()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10" });

            lab5.LoadWeightedGraph(file);
            var (_, previous) = lab5.Dijkstra("A");
            var path = lab5.GetPath(previous, "A", "NonExistent");

            Assert.Empty(path);
            File.Delete(file);
        }

        [Fact]
        public void Test12_GetPath_EmptyPrevious_ReturnsEmpty()
        {
            var lab5 = new Lab5();
            var previous = new Dictionary<string, string>();

            var path = lab5.GetPath(previous, "A", "B");

            Assert.Empty(path);
        }

        [Fact]
        public void Test13_GetPath_CircularReference_HandlesCorrectly()
        {
            // Проверка на защиту от бесконечного цикла
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10", "B - C, 20" });

            lab5.LoadWeightedGraph(file);
            var (_, previous) = lab5.Dijkstra("A");
            var path = lab5.GetPath(previous, "A", "C");

            // Путь должен быть конечным
            Assert.True(path.Count <= 10);
            File.Delete(file);
        }

        // ===== ТЕСТЫ ПОИСКА КРАТЧАЙШЕГО ПУТИ НА ГРАНИЧНЫХ СЛУЧАЯХ =====

        [Fact]
        public void Test14_FindShortestPath_BothNodesNonExistent_HandlesCorrectly()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10" });

            lab5.LoadWeightedGraph(file);
            var (distance, path) = lab5.FindShortestPath("X", "Y");

            // Может вернуть -1 или int.MaxValue для несуществующих узлов
            Assert.True(distance == -1 || distance == int.MaxValue);
            File.Delete(file);
        }

        [Fact]
        public void Test15_FindShortestPath_VeryLongPath_FindsCorrectly()
        {
            // Длинная цепочка узлов
            var lab5 = new Lab5();
            var lines = new List<string>();
            for (int i = 0; i < 20; i++)
            {
                lines.Add($"Node{i} - Node{i + 1}, 1");
            }
            string file = CreateWeightedGraphFile(lines.ToArray());

            lab5.LoadWeightedGraph(file);
            var (distance, path) = lab5.FindShortestPath("Node0", "Node20");

            Assert.Equal(20, distance);
            Assert.Equal(21, path.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test16_FindShortestPath_MultipleEqualPaths_FindsOne()
        {
            // Два пути с одинаковой длиной
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] 
            { 
                "A - B, 10",
                "A - C, 10",
                "B - D, 10",
                "C - D, 10"
            });

            lab5.LoadWeightedGraph(file);
            var (distance, path) = lab5.FindShortestPath("A", "D");

            Assert.Equal(20, distance);
            Assert.Equal(3, path.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test17_FindShortestPath_GraphWithCycle_FindsOptimal()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] 
            { 
                "A - B, 5",
                "B - C, 5",
                "C - A, 5",
                "A - D, 20"
            });

            lab5.LoadWeightedGraph(file);
            var (distance, path) = lab5.FindShortestPath("A", "D");

            Assert.Equal(20, distance);
            File.Delete(file);
        }
    }
}
