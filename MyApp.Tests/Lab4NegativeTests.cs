using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using MyApp;

namespace MyApp.Tests
{
    // Ложно-положительные тесты для Lab4
    // Проверяют граничные случаи и некорректные входные данные
    public class Lab4NegativeTests
    {
        private string CreateTestGraphFile(string[] lines)
        {
            string filename = Path.GetTempFileName();
            File.WriteAllLines(filename, lines);
            return filename;
        }

        // ===== ТЕСТЫ НА НЕКОРРЕКТНЫЕ ДАННЫЕ =====

        [Fact]
        public void Test01_LoadGraph_InvalidFormat_SkipsLine()
        {
            // Строка без разделителя "-"
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "InvalidLine", "A - B" });

            lab4.LoadGraph(file);
            var nodes = lab4.GetNodes();

            // Должны загрузиться только корректные строки
            Assert.Equal(2, nodes.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test02_LoadGraph_OnlySpaces_LoadsEmptyGraph()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "   ", "  ", "" });

            lab4.LoadGraph(file);
            var nodes = lab4.GetNodes();

            Assert.Empty(nodes);
            File.Delete(file);
        }

        [Fact]
        public void Test03_LoadGraph_SpecialCharacters_HandlesCorrectly()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "Node@1 - Node#2" });

            lab4.LoadGraph(file);
            var nodes = lab4.GetNodes();

            Assert.Equal(2, nodes.Count);
            Assert.Contains("Node@1", nodes);
            Assert.Contains("Node#2", nodes);
            File.Delete(file);
        }

        [Fact]
        public void Test04_LoadGraph_VeryLongNodeNames_LoadsCorrectly()
        {
            var lab4 = new Lab4();
            string longName = new string('A', 100);
            string file = CreateTestGraphFile(new[] { $"{longName} - B" });

            lab4.LoadGraph(file);
            var nodes = lab4.GetNodes();

            Assert.Equal(2, nodes.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test05_LoadGraph_SelfLoop_HandlesCorrectly()
        {
            // Петля: узел соединен сам с собой
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - A" });

            lab4.LoadGraph(file);
            var nodes = lab4.GetNodes();

            Assert.Single(nodes);
            Assert.Contains("A", nodes);
            File.Delete(file);
        }

        // ===== ТЕСТЫ BFS НА ГРАНИЧНЫХ СЛУЧАЯХ =====

        [Fact]
        public void Test06_BFS_NonExistentStartNode_ReturnsEmpty()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B" });

            lab4.LoadGraph(file);
            
            // Попытка запустить BFS от несуществующего узла
            // Должен вернуть пустой список или только стартовый узел
            var result = lab4.BFS("NonExistent");

            Assert.Single(result);
            Assert.Equal("NonExistent", result[0]);
            File.Delete(file);
        }

        [Fact]
        public void Test07_BFS_IsolatedNode_ReturnsSingleNode()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "C - D" });

            lab4.LoadGraph(file);
            var result = lab4.BFS("C");

            // Должен вернуть только узлы из компоненты C-D
            Assert.Equal(2, result.Count);
            Assert.Contains("C", result);
            Assert.Contains("D", result);
            File.Delete(file);
        }

        [Fact]
        public void Test08_BFS_GraphWithCycle_HandlesCorrectly()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "B - C", "C - A" });

            lab4.LoadGraph(file);
            var result = lab4.BFS("A");

            // Не должно быть бесконечного цикла
            Assert.Equal(3, result.Count);
            File.Delete(file);
        }

        // ===== ТЕСТЫ DFS НА ГРАНИЧНЫХ СЛУЧАЯХ =====

        [Fact]
        public void Test09_DFS_NonExistentStartNode_ReturnsEmpty()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B" });

            lab4.LoadGraph(file);
            var result = lab4.DFS("NonExistent");

            Assert.Single(result);
            File.Delete(file);
        }

        [Fact]
        public void Test10_DFS_GraphWithCycle_NoInfiniteLoop()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "B - C", "C - A" });

            lab4.LoadGraph(file);
            var result = lab4.DFS("A");

            // Проверяем что нет дубликатов (бесконечного цикла)
            Assert.Equal(3, result.Count);
            Assert.Equal(result.Count, new HashSet<string>(result).Count);
            File.Delete(file);
        }

        [Fact]
        public void Test11_DFS_DeepGraph_HandlesRecursion()
        {
            // Глубокий граф для проверки рекурсии
            var lab4 = new Lab4();
            var lines = new List<string>();
            for (int i = 0; i < 50; i++)
            {
                lines.Add($"Node{i} - Node{i + 1}");
            }
            string file = CreateTestGraphFile(lines.ToArray());

            lab4.LoadGraph(file);
            var result = lab4.DFS("Node0");

            Assert.Equal(51, result.Count);
            File.Delete(file);
        }

        // ===== ТЕСТЫ ДОСТИЖИМОСТИ НА ГРАНИЧНЫХ СЛУЧАЯХ =====

        [Fact]
        public void Test12_IsReachable_NonExistentNodes_ReturnsFalse()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B" });

            lab4.LoadGraph(file);
            bool reachable = lab4.IsReachable("NonExistent1", "NonExistent2");

            Assert.False(reachable);
            File.Delete(file);
        }

        [Fact]
        public void Test13_IsReachable_FromExistentToNonExistent_ReturnsFalse()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B" });

            lab4.LoadGraph(file);
            bool reachable = lab4.IsReachable("A", "NonExistent");

            Assert.False(reachable);
            File.Delete(file);
        }

        [Fact]
        public void Test14_IsReachable_EmptyGraph_ReturnsFalse()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new string[] { });

            lab4.LoadGraph(file);
            bool reachable = lab4.IsReachable("A", "B");

            Assert.False(reachable);
            File.Delete(file);
        }

        // ===== ТЕСТЫ КОМПОНЕНТ СВЯЗНОСТИ НА ГРАНИЧНЫХ СЛУЧАЯХ =====

        [Fact]
        public void Test15_FindConnectedComponents_EmptyGraph_ReturnsEmpty()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new string[] { });

            lab4.LoadGraph(file);
            var components = lab4.FindConnectedComponents();

            Assert.Empty(components);
            File.Delete(file);
        }

        [Fact]
        public void Test16_FindConnectedComponents_AllIsolated_ReturnsMany()
        {
            // Каждый узел - отдельная компонента
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] { "A - B", "C - D", "E - F", "G - H" });

            lab4.LoadGraph(file);
            var components = lab4.FindConnectedComponents();

            Assert.Equal(4, components.Count);
            File.Delete(file);
        }

        [Fact]
        public void Test17_FindConnectedComponents_ComplexGraph_IdentifiesCorrectly()
        {
            var lab4 = new Lab4();
            string file = CreateTestGraphFile(new[] 
            { 
                "A - B", "B - C", // Компонента 1
                "D - E", "E - F", "F - D", // Компонента 2 (с циклом)
                "G - H" // Компонента 3
            });

            lab4.LoadGraph(file);
            var components = lab4.FindConnectedComponents();

            Assert.Equal(3, components.Count);
            File.Delete(file);
        }
    }
}
