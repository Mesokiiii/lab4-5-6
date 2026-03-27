using System;
using System.Collections.Generic;
using Xunit;
using MyApp;

namespace MyApp.Tests
{
    // Тесты для Лабораторной работы №6
    public class Lab6Tests
    {
        // Вспомогательный метод для создания простого графа
        private (Dictionary<string, List<(string, int)>>, List<string>) CreateSimpleGraph()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10), ("C", 5) },
                ["B"] = new List<(string, int)> { ("A", 10), ("D", 15) },
                ["C"] = new List<(string, int)> { ("A", 5), ("D", 20) },
                ["D"] = new List<(string, int)> { ("B", 15), ("C", 20) }
            };
            var nodes = new List<string> { "A", "B", "C", "D" };
            return (graph, nodes);
        }

        // Граф с точкой сочленения
        private (Dictionary<string, List<(string, int)>>, List<string>) CreateGraphWithArticulationPoint()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10) },
                ["B"] = new List<(string, int)> { ("A", 10), ("C", 5) },
                ["C"] = new List<(string, int)> { ("B", 5), ("D", 15) },
                ["D"] = new List<(string, int)> { ("C", 15) }
            };
            var nodes = new List<string> { "A", "B", "C", "D" };
            return (graph, nodes);
        }

        // ===== ТЕСТЫ ПОИСКА ТОЧЕК СОЧЛЕНЕНИЯ =====

        [Fact]
        public void Test01_FindArticulationPoints_SimpleGraph_FindsNone()
        {
            // Полносвязный граф не имеет точек сочленения
            var (graph, nodes) = CreateSimpleGraph();
            var lab6 = new Lab6(graph, nodes);

            var ap = lab6.FindArticulationPoints();

            Assert.Empty(ap);
        }

        [Fact]
        public void Test02_FindArticulationPoints_LinearGraph_FindsMiddleNodes()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10) },
                ["B"] = new List<(string, int)> { ("A", 10), ("C", 5) },
                ["C"] = new List<(string, int)> { ("B", 5) }
            };
            var nodes = new List<string> { "A", "B", "C" };
            var lab6 = new Lab6(graph, nodes);

            var ap = lab6.FindArticulationPoints();

            Assert.Contains("B", ap);
        }

        [Fact]
        public void Test03_FindArticulationPoints_BridgeNode_FindsIt()
        {
            var (graph, nodes) = CreateGraphWithArticulationPoint();
            var lab6 = new Lab6(graph, nodes);

            var ap = lab6.FindArticulationPoints();

            // B и C - точки сочленения
            Assert.True(ap.Count >= 1);
        }

        [Fact]
        public void Test04_FindArticulationPoints_TwoComponents_FindsNone()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10) },
                ["B"] = new List<(string, int)> { ("A", 10) },
                ["C"] = new List<(string, int)> { ("D", 5) },
                ["D"] = new List<(string, int)> { ("C", 5) }
            };
            var nodes = new List<string> { "A", "B", "C", "D" };
            var lab6 = new Lab6(graph, nodes);

            var ap = lab6.FindArticulationPoints();

            Assert.Empty(ap);
        }

        [Fact]
        public void Test05_FindArticulationPoints_StarGraph_FindsCenter()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["Center"] = new List<(string, int)> { ("A", 10), ("B", 5), ("C", 15) },
                ["A"] = new List<(string, int)> { ("Center", 10) },
                ["B"] = new List<(string, int)> { ("Center", 5) },
                ["C"] = new List<(string, int)> { ("Center", 15) }
            };
            var nodes = new List<string> { "Center", "A", "B", "C" };
            var lab6 = new Lab6(graph, nodes);

            var ap = lab6.FindArticulationPoints();

            Assert.Contains("Center", ap);
        }

        // ===== ТЕСТЫ МИНИМАЛЬНОГО ОСТОВНОГО ДЕРЕВА =====

        [Fact]
        public void Test06_PrimMST_SimpleGraph_ReturnsCorrectEdges()
        {
            var (graph, nodes) = CreateSimpleGraph();
            var lab6 = new Lab6(graph, nodes);

            var mst = lab6.PrimMST();

            // МОД должно содержать n-1 ребер для n узлов
            Assert.Equal(3, mst.Count);
        }

        [Fact]
        public void Test07_PrimMST_SimpleGraph_HasMinimalWeight()
        {
            var (graph, nodes) = CreateSimpleGraph();
            var lab6 = new Lab6(graph, nodes);

            var mst = lab6.PrimMST();
            int totalWeight = 0;
            foreach (var (_, _, weight) in mst)
            {
                totalWeight += weight;
            }

            // Минимальный вес: A-C(5) + C-A(уже есть) + B-A(10) + B-D(15) = 30
            Assert.Equal(30, totalWeight);
        }

        [Fact]
        public void Test08_PrimMST_LinearGraph_ReturnsAllEdges()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10) },
                ["B"] = new List<(string, int)> { ("A", 10), ("C", 20) },
                ["C"] = new List<(string, int)> { ("B", 20) }
            };
            var nodes = new List<string> { "A", "B", "C" };
            var lab6 = new Lab6(graph, nodes);

            var mst = lab6.PrimMST();

            Assert.Equal(2, mst.Count);
        }

        [Fact]
        public void Test09_PrimMST_EmptyGraph_ReturnsEmpty()
        {
            var graph = new Dictionary<string, List<(string, int)>>();
            var nodes = new List<string>();
            var lab6 = new Lab6(graph, nodes);

            var mst = lab6.PrimMST();

            Assert.Empty(mst);
        }

        [Fact]
        public void Test10_PrimMST_SingleNode_ReturnsEmpty()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)>()
            };
            var nodes = new List<string> { "A" };
            var lab6 = new Lab6(graph, nodes);

            var mst = lab6.PrimMST();

            Assert.Empty(mst);
        }

        // ===== ТЕСТЫ ОПТИМАЛЬНОГО ПУТИ ТРАНСПОРТИРОВКИ =====

        [Fact]
        public void Test11_FindOptimalTransportPath_SimpleGraph_FindsMaxCapacity()
        {
            var (graph, nodes) = CreateSimpleGraph();
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "D");

            // Максимальная пропускная способность ограничена минимумом на пути
            Assert.True(maxFlow > 0);
            Assert.NotEmpty(path);
        }

        [Fact]
        public void Test12_FindOptimalTransportPath_DirectConnection_ReturnsDirectWeight()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 100) },
                ["B"] = new List<(string, int)> { ("A", 100) }
            };
            var nodes = new List<string> { "A", "B" };
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "B");

            Assert.Equal(100, maxFlow);
            Assert.Equal(2, path.Count);
        }

        [Fact]
        public void Test13_FindOptimalTransportPath_BottleneckPath_ReturnsMinimum()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 100) },
                ["B"] = new List<(string, int)> { ("A", 100), ("C", 10) },
                ["C"] = new List<(string, int)> { ("B", 10), ("D", 50) },
                ["D"] = new List<(string, int)> { ("C", 50) }
            };
            var nodes = new List<string> { "A", "B", "C", "D" };
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "D");

            // Узкое место - ребро B-C с весом 10
            Assert.Equal(10, maxFlow);
        }

        [Fact]
        public void Test14_FindOptimalTransportPath_NoPath_ReturnsZero()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10) },
                ["B"] = new List<(string, int)> { ("A", 10) },
                ["C"] = new List<(string, int)> { ("D", 20) },
                ["D"] = new List<(string, int)> { ("C", 20) }
            };
            var nodes = new List<string> { "A", "B", "C", "D" };
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "D");

            Assert.Equal(0, maxFlow);
        }

        [Fact]
        public void Test15_FindOptimalTransportPath_SameNode_ReturnsInfinity()
        {
            var (graph, nodes) = CreateSimpleGraph();
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "A");

            Assert.Equal(int.MaxValue, maxFlow);
            Assert.Single(path);
        }

        [Fact]
        public void Test16_FindOptimalTransportPath_MultipleRoutes_ChoosesBest()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 50), ("C", 100) },
                ["B"] = new List<(string, int)> { ("A", 50), ("D", 60) },
                ["C"] = new List<(string, int)> { ("A", 100), ("D", 80) },
                ["D"] = new List<(string, int)> { ("B", 60), ("C", 80) }
            };
            var nodes = new List<string> { "A", "B", "C", "D" };
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "D");

            // Лучший путь: A -> C -> D с минимумом 80
            Assert.Equal(80, maxFlow);
        }

        [Fact]
        public void Test17_FindOptimalTransportPath_ComplexGraph_FindsOptimal()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["Start"] = new List<(string, int)> { ("A", 100), ("B", 50) },
                ["A"] = new List<(string, int)> { ("Start", 100), ("End", 30) },
                ["B"] = new List<(string, int)> { ("Start", 50), ("C", 40) },
                ["C"] = new List<(string, int)> { ("B", 40), ("End", 60) },
                ["End"] = new List<(string, int)> { ("A", 30), ("C", 60) }
            };
            var nodes = new List<string> { "Start", "A", "B", "C", "End" };
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("Start", "End");

            // Оптимальный путь: Start -> B -> C -> End с минимумом 40
            Assert.Equal(40, maxFlow);
            Assert.True(path.Count >= 2);
        }
    }
}
