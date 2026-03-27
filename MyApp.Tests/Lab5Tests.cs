using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using MyApp;

namespace MyApp.Tests
{
    // Тесты для Лабораторной работы №5
    public class Lab5Tests
    {
        // Вспомогательный метод для создания тестового файла с весами
        private string CreateWeightedGraphFile(string[] lines)
        {
            string filename = Path.GetTempFileName();
            File.WriteAllLines(filename, lines);
            return filename;
        }

        // ===== ТЕСТЫ ЗАГРУЗКИ ВЗВЕШЕННОГО ГРАФА =====

        [Fact]
        public void Test01_LoadWeightedGraph_SimpleGraph_LoadsCorrectly()
        {
            // Arrange
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10", "B - C, 20" });

            // Act
            lab5.LoadWeightedGraph(file);
            var nodes = lab5.GetNodes();

            // Assert
            Assert.Equal(3, nodes.Count);
            Assert.Contains("A", nodes);
            Assert.Contains("B", nodes);
            Assert.Contains("C", nodes);

            File.Delete(file);
        }

        [Fact]
        public void Test02_LoadWeightedGraph_EmptyFile_LoadsEmptyGraph()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new string[] { });

            lab5.LoadWeightedGraph(file);
            var nodes = lab5.GetNodes();

            Assert.Empty(nodes);
            File.Delete(file);
        }

        [Fact]
        public void Test03_LoadWeightedGraph_WithSpaces_ParsesCorrectly()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "  Node1  -  Node2  ,  50  " });

            lab5.LoadWeightedGraph(file);
            var nodes = lab5.GetNodes();

            Assert.Equal(2, nodes.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test04_LoadWeightedGraph_MultipleEdges_LoadsAll()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10", "B - C, 20", "C - D, 30" });

            lab5.LoadWeightedGraph(file);
            var nodes = lab5.GetNodes();

            Assert.Equal(4, nodes.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test05_LoadWeightedGraph_DifferentWeights_HandlesCorrectly()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 5", "A - C, 100" });

            lab5.LoadWeightedGraph(file);
            var nodes = lab5.GetNodes();

            Assert.Equal(3, nodes.Count);
            File.Delete(file);
        }

        // ===== ТЕСТЫ АЛГОРИТМА ДЕЙКСТРЫ =====

        [Fact]
        public void Test06_Dijkstra_SimpleGraph_CalculatesDistances()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10", "B - C, 20" });

            lab5.LoadWeightedGraph(file);
            var (distances, previous) = lab5.Dijkstra("A");

            Assert.Equal(0, distances["A"]);
            Assert.Equal(10, distances["B"]);
            Assert.Equal(30, distances["C"]);
            File.Delete(file);
        }

        [Fact]
        public void Test07_Dijkstra_StartNode_HasZeroDistance()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10" });

            lab5.LoadWeightedGraph(file);
            var (distances, _) = lab5.Dijkstra("A");

            Assert.Equal(0, distances["A"]);
            File.Delete(file);
        }

        [Fact]
        public void Test08_Dijkstra_MultiplePathsGraph_FindsShortest()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] 
            { 
                "A - B, 10", 
                "A - C, 5", 
                "C - B, 3" 
            });

            lab5.LoadWeightedGraph(file);
            var (distances, _) = lab5.Dijkstra("A");

            // Кратчайший путь A->B через C: 5 + 3 = 8
            Assert.Equal(8, distances["B"]);
            File.Delete(file);
        }

        [Fact]
        public void Test09_Dijkstra_LinearGraph_CalculatesCorrectly()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] 
            { 
                "A - B, 5", 
                "B - C, 10", 
                "C - D, 15" 
            });

            lab5.LoadWeightedGraph(file);
            var (distances, _) = lab5.Dijkstra("A");

            Assert.Equal(30, distances["D"]);
            File.Delete(file);
        }

        [Fact]
        public void Test10_Dijkstra_DisconnectedNode_HasMaxDistance()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10", "C - D, 20" });

            lab5.LoadWeightedGraph(file);
            var (distances, _) = lab5.Dijkstra("A");

            Assert.Equal(int.MaxValue, distances["C"]);
            Assert.Equal(int.MaxValue, distances["D"]);
            File.Delete(file);
        }

        // ===== ТЕСТЫ ВОССТАНОВЛЕНИЯ ПУТИ =====

        [Fact]
        public void Test11_GetPath_SimpleGraph_ReturnsCorrectPath()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10", "B - C, 20" });

            lab5.LoadWeightedGraph(file);
            var (_, previous) = lab5.Dijkstra("A");
            var path = lab5.GetPath(previous, "A", "C");

            Assert.Equal(3, path.Count);
            Assert.Equal("A", path[0]);
            Assert.Equal("B", path[1]);
            Assert.Equal("C", path[2]);
            File.Delete(file);
        }

        [Fact]
        public void Test12_GetPath_DirectConnection_ReturnsTwoNodes()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10" });

            lab5.LoadWeightedGraph(file);
            var (_, previous) = lab5.Dijkstra("A");
            var path = lab5.GetPath(previous, "A", "B");

            Assert.Equal(2, path.Count);
            Assert.Equal("A", path[0]);
            Assert.Equal("B", path[1]);
            File.Delete(file);
        }

        [Fact]
        public void Test13_GetPath_NoPath_ReturnsEmptyList()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10", "C - D, 20" });

            lab5.LoadWeightedGraph(file);
            var (_, previous) = lab5.Dijkstra("A");
            var path = lab5.GetPath(previous, "A", "D");

            Assert.Empty(path);
            File.Delete(file);
        }

        // ===== ТЕСТЫ ПОИСКА КРАТЧАЙШЕГО ПУТИ =====

        [Fact]
        public void Test14_FindShortestPath_SimpleGraph_ReturnsCorrectDistanceAndPath()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10", "B - C, 20" });

            lab5.LoadWeightedGraph(file);
            var (distance, path) = lab5.FindShortestPath("A", "C");

            Assert.Equal(30, distance);
            Assert.Equal(3, path.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test15_FindShortestPath_ComplexGraph_FindsOptimalRoute()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] 
            { 
                "A - B, 100", 
                "A - C, 50", 
                "C - D, 30",
                "D - B, 10"
            });

            lab5.LoadWeightedGraph(file);
            var (distance, path) = lab5.FindShortestPath("A", "B");

            // Оптимальный путь: A -> C -> D -> B = 90
            Assert.Equal(90, distance);
            Assert.Equal(4, path.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test16_FindShortestPath_NoPath_ReturnsMaxDistance()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10", "C - D, 20" });

            lab5.LoadWeightedGraph(file);
            var (distance, path) = lab5.FindShortestPath("A", "D");

            // Недостижимый узел имеет расстояние int.MaxValue
            Assert.Equal(int.MaxValue, distance);
            Assert.Empty(path);
            File.Delete(file);
        }

        [Fact]
        public void Test17_FindShortestPath_SameNode_ReturnsZeroDistance()
        {
            var lab5 = new Lab5();
            string file = CreateWeightedGraphFile(new[] { "A - B, 10" });

            lab5.LoadWeightedGraph(file);
            var (distance, path) = lab5.FindShortestPath("A", "A");

            Assert.Equal(0, distance);
            Assert.Single(path);
            Assert.Equal("A", path[0]);
            File.Delete(file);
        }
    }
}
