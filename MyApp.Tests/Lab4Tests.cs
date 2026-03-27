using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using MyApp;

namespace MyApp.Tests
{
    // Тесты для Лабораторной работы №4
    public class Lab4Tests
    {
        // Вспомогательный метод для создания тестового файла
        private string CreateTestGraphFile(string[] lines)
        {
            string filename = Path.GetTempFileName();
            File.WriteAllLines(filename, lines);
            return filename;
        }

        // ===== ТЕСТЫ ЗАГРУЗКИ ГРАФА =====

        [Fact]
        public void Test01_LoadGraph_SimpleGraph_LoadsCorrectly()
        {
            // Arrange - подготовка
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "B - C" });

            // Act - действие
            lab4.LoadGraph(file);
            var nodes = lab4.GetNodes();

            // Assert - проверка
            Assert.Equal(3, nodes.Count);
            Assert.Contains("A", nodes);
            Assert.Contains("B", nodes);
            Assert.Contains("C", nodes);

            File.Delete(file);
        }

        [Fact]
        public void Test02_LoadGraph_EmptyFile_LoadsEmptyGraph()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new string[] { });

            lab4.LoadGraph(file);
            var nodes = lab4.GetNodes();

            Assert.Empty(nodes);
            File.Delete(file);
        }

        [Fact]
        public void Test03_LoadGraph_WithSpaces_TrimsCorrectly()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "  Node1  -  Node2  " });

            lab4.LoadGraph(file);
            var nodes = lab4.GetNodes();

            Assert.Contains("Node1", nodes);
            Assert.Contains("Node2", nodes);
            File.Delete(file);
        }

        [Fact]
        public void Test04_LoadGraph_MultipleEdges_CreatesConnections()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "B - C", "C - D" });

            lab4.LoadGraph(file);
            var nodes = lab4.GetNodes();

            Assert.Equal(4, nodes.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test05_LoadGraph_DuplicateEdges_HandlesCorrectly()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "A - B" });

            lab4.LoadGraph(file);
            var nodes = lab4.GetNodes();

            Assert.Equal(2, nodes.Count);
            File.Delete(file);
        }

        // ===== ТЕСТЫ BFS =====

        [Fact]
        public void Test06_BFS_SimpleGraph_ReturnsCorrectOrder()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "A - C", "B - D" });

            lab4.LoadGraph(file);
            var result = lab4.BFS("A");

            Assert.Equal("A", result[0]);
            Assert.Contains("B", result);
            Assert.Contains("C", result);
            Assert.Contains("D", result);
            File.Delete(file);
        }

        [Fact]
        public void Test07_BFS_SingleNode_ReturnsSingleNode()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B" });

            lab4.LoadGraph(file);
            var result = lab4.BFS("A");

            Assert.Equal(2, result.Count);
            Assert.Equal("A", result[0]);
            File.Delete(file);
        }

        [Fact]
        public void Test08_BFS_LinearGraph_ReturnsLinearOrder()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "B - C", "C - D" });

            lab4.LoadGraph(file);
            var result = lab4.BFS("A");

            Assert.Equal(4, result.Count);
            Assert.Equal("A", result[0]);
            File.Delete(file);
        }

        // ===== ТЕСТЫ DFS =====

        [Fact]
        public void Test09_DFS_SimpleGraph_VisitsAllNodes()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "A - C", "B - D" });

            lab4.LoadGraph(file);
            var result = lab4.DFS("A");

            Assert.Equal(4, result.Count);
            Assert.Equal("A", result[0]);
            File.Delete(file);
        }

        [Fact]
        public void Test10_DFS_LinearGraph_VisitsInDepthOrder()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "B - C", "C - D" });

            lab4.LoadGraph(file);
            var result = lab4.DFS("A");

            Assert.Equal(4, result.Count);
            Assert.Contains("A", result);
            Assert.Contains("D", result);
            File.Delete(file);
        }

        [Fact]
        public void Test11_DFS_TreeGraph_VisitsAllBranches()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "Root - Left", "Root - Right", "Left - LL" });

            lab4.LoadGraph(file);
            var result = lab4.DFS("Root");

            Assert.Equal(4, result.Count);
            File.Delete(file);
        }

        // ===== ТЕСТЫ ДОСТИЖИМОСТИ =====

        [Fact]
        public void Test12_IsReachable_ConnectedNodes_ReturnsTrue()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "B - C" });

            lab4.LoadGraph(file);
            bool reachable = lab4.IsReachable("A", "C");

            Assert.True(reachable);
            File.Delete(file);
        }

        [Fact]
        public void Test13_IsReachable_DisconnectedNodes_ReturnsFalse()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "C - D" });

            lab4.LoadGraph(file);
            bool reachable = lab4.IsReachable("A", "D");

            Assert.False(reachable);
            File.Delete(file);
        }

        [Fact]
        public void Test14_IsReachable_SameNode_ReturnsTrue()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B" });

            lab4.LoadGraph(file);
            bool reachable = lab4.IsReachable("A", "A");

            Assert.True(reachable);
            File.Delete(file);
        }

        // ===== ТЕСТЫ КОМПОНЕНТ СВЯЗНОСТИ =====

        [Fact]
        public void Test15_FindConnectedComponents_SingleComponent_ReturnsOne()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "B - C", "C - D" });

            lab4.LoadGraph(file);
            var components = lab4.FindConnectedComponents();

            Assert.Single(components);
            Assert.Equal(4, components[0].Count);
            File.Delete(file);
        }

        [Fact]
        public void Test16_FindConnectedComponents_TwoComponents_ReturnsTwo()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "C - D" });

            lab4.LoadGraph(file);
            var components = lab4.FindConnectedComponents();

            Assert.Equal(2, components.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test17_FindConnectedComponents_ThreeComponents_ReturnsThree()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "C - D", "E - F" });

            lab4.LoadGraph(file);
            var components = lab4.FindConnectedComponents();

            Assert.Equal(3, components.Count);
            File.Delete(file);
        }
    }
}
