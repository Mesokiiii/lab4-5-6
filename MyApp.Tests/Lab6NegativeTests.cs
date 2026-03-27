using System;
using System.Collections.Generic;
using Xunit;
using MyApp;

namespace MyApp.Tests
{
    // Ложно-положительные тесты для Lab6
    public class Lab6NegativeTests
    {
        // ===== ТЕСТЫ ТОЧЕК СОЧЛЕНЕНИЯ НА ГРАНИЧНЫХ СЛУЧАЯХ =====

        [Fact]
        public void Test01_FindArticulationPoints_EmptyGraph_ReturnsEmpty()
        {
            var graph = new Dictionary<string, List<(string, int)>>();
            var nodes = new List<string>();
            var lab6 = new Lab6(graph, nodes);

            var ap = lab6.FindArticulationPoints();

            Assert.Empty(ap);
        }

        [Fact]
        public void Test02_FindArticulationPoints_SingleNode_ReturnsEmpty()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)>()
            };
            var nodes = new List<string> { "A" };
            var lab6 = new Lab6(graph, nodes);

            var ap = lab6.FindArticulationPoints();

            Assert.Empty(ap);
        }

        [Fact]
        public void Test03_FindArticulationPoints_TwoNodesConnected_ReturnsEmpty()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10) },
                ["B"] = new List<(string, int)> { ("A", 10) }
            };
            var nodes = new List<string> { "A", "B" };
            var lab6 = new Lab6(graph, nodes);

            var ap = lab6.FindArticulationPoints();

            Assert.Empty(ap);
        }

        [Fact]
        public void Test04_FindArticulationPoints_CompleteGraph_ReturnsEmpty()
        {
            // Полносвязный граф - каждый узел соединен с каждым
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10), ("C", 10), ("D", 10) },
                ["B"] = new List<(string, int)> { ("A", 10), ("C", 10), ("D", 10) },
                ["C"] = new List<(string, int)> { ("A", 10), ("B", 10), ("D", 10) },
                ["D"] = new List<(string, int)> { ("A", 10), ("B", 10), ("C", 10) }
            };
            var nodes = new List<string> { "A", "B", "C", "D" };
            var lab6 = new Lab6(graph, nodes);

            var ap = lab6.FindArticulationPoints();

            Assert.Empty(ap);
        }

        [Fact]
        public void Test05_FindArticulationPoints_GraphWithSelfLoop_HandlesCorrectly()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("A", 10), ("B", 5) },
                ["B"] = new List<(string, int)> { ("A", 5) }
            };
            var nodes = new List<string> { "A", "B" };
            var lab6 = new Lab6(graph, nodes);

            var ap = lab6.FindArticulationPoints();

            // Не должно быть ошибок
            Assert.NotNull(ap);
        }

        // ===== ТЕСТЫ МОД НА ГРАНИЧНЫХ СЛУЧАЯХ =====

        [Fact]
        public void Test06_PrimMST_TwoNodesConnected_ReturnsOneEdge()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10) },
                ["B"] = new List<(string, int)> { ("A", 10) }
            };
            var nodes = new List<string> { "A", "B" };
            var lab6 = new Lab6(graph, nodes);

            var mst = lab6.PrimMST();

            Assert.Single(mst);
            Assert.Equal(10, mst[0].weight);
        }

        [Fact]
        public void Test07_PrimMST_DisconnectedGraph_ReturnsPartialMST()
        {
            // Несвязный граф - МОД будет только для одной компоненты
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10) },
                ["B"] = new List<(string, int)> { ("A", 10) },
                ["C"] = new List<(string, int)> { ("D", 20) },
                ["D"] = new List<(string, int)> { ("C", 20) }
            };
            var nodes = new List<string> { "A", "B", "C", "D" };
            var lab6 = new Lab6(graph, nodes);

            var mst = lab6.PrimMST();

            // МОД будет только для компоненты, содержащей стартовый узел
            Assert.True(mst.Count < 3);
        }

        [Fact]
        public void Test08_PrimMST_AllEqualWeights_ReturnsValidMST()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10), ("C", 10) },
                ["B"] = new List<(string, int)> { ("A", 10), ("C", 10) },
                ["C"] = new List<(string, int)> { ("A", 10), ("B", 10) }
            };
            var nodes = new List<string> { "A", "B", "C" };
            var lab6 = new Lab6(graph, nodes);

            var mst = lab6.PrimMST();

            Assert.Equal(2, mst.Count);
            int totalWeight = 0;
            foreach (var (_, _, weight) in mst)
            {
                totalWeight += weight;
            }
            Assert.Equal(20, totalWeight);
        }

        [Fact]
        public void Test09_PrimMST_VeryLargeWeights_HandlesCorrectly()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", int.MaxValue / 2) },
                ["B"] = new List<(string, int)> { ("A", int.MaxValue / 2) }
            };
            var nodes = new List<string> { "A", "B" };
            var lab6 = new Lab6(graph, nodes);

            var mst = lab6.PrimMST();

            Assert.Single(mst);
        }

        [Fact]
        public void Test10_PrimMST_GraphWithCycle_ReturnsTreeWithoutCycle()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 1), ("C", 3) },
                ["B"] = new List<(string, int)> { ("A", 1), ("C", 2) },
                ["C"] = new List<(string, int)> { ("A", 3), ("B", 2) }
            };
            var nodes = new List<string> { "A", "B", "C" };
            var lab6 = new Lab6(graph, nodes);

            var mst = lab6.PrimMST();

            // МОД для 3 узлов должно иметь 2 ребра
            Assert.Equal(2, mst.Count);
        }

        // ===== ТЕСТЫ ОПТИМАЛЬНОГО ПУТИ НА ГРАНИЧНЫХ СЛУЧАЯХ =====

        [Fact]
        public void Test11_FindOptimalTransportPath_EmptyGraph_ReturnsZero()
        {
            var graph = new Dictionary<string, List<(string, int)>>();
            var nodes = new List<string>();
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "B");

            Assert.Equal(0, maxFlow);
        }

        [Fact]
        public void Test12_FindOptimalTransportPath_NonExistentNodes_ReturnsZero()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 10) },
                ["B"] = new List<(string, int)> { ("A", 10) }
            };
            var nodes = new List<string> { "A", "B" };
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("X", "Y");

            Assert.Equal(0, maxFlow);
        }

        [Fact]
        public void Test13_FindOptimalTransportPath_ZeroWeightEdge_ReturnsZero()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 0) },
                ["B"] = new List<(string, int)> { ("A", 0) }
            };
            var nodes = new List<string> { "A", "B" };
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "B");

            Assert.Equal(0, maxFlow);
        }

        [Fact]
        public void Test14_FindOptimalTransportPath_AllPathsBlocked_ReturnsZero()
        {
            // Все пути имеют нулевую пропускную способность
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 0), ("C", 0) },
                ["B"] = new List<(string, int)> { ("A", 0), ("D", 0) },
                ["C"] = new List<(string, int)> { ("A", 0), ("D", 0) },
                ["D"] = new List<(string, int)> { ("B", 0), ("C", 0) }
            };
            var nodes = new List<string> { "A", "B", "C", "D" };
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "D");

            Assert.Equal(0, maxFlow);
        }

        [Fact]
        public void Test15_FindOptimalTransportPath_VeryLargeCapacity_HandlesCorrectly()
        {
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", int.MaxValue / 2) },
                ["B"] = new List<(string, int)> { ("A", int.MaxValue / 2) }
            };
            var nodes = new List<string> { "A", "B" };
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "B");

            Assert.True(maxFlow > 0);
        }

        [Fact]
        public void Test16_FindOptimalTransportPath_ComplexBottleneck_FindsCorrectly()
        {
            // Сложный граф с несколькими узкими местами
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 100), ("C", 50) },
                ["B"] = new List<(string, int)> { ("A", 100), ("D", 5) },
                ["C"] = new List<(string, int)> { ("A", 50), ("D", 60) },
                ["D"] = new List<(string, int)> { ("B", 5), ("C", 60) }
            };
            var nodes = new List<string> { "A", "B", "C", "D" };
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "D");

            // Лучший путь через C с пропускной способностью 50
            Assert.Equal(50, maxFlow);
        }

        [Fact]
        public void Test17_FindOptimalTransportPath_LongChainWithBottleneck_FindsMinimum()
        {
            // Длинная цепочка с одним узким местом
            var graph = new Dictionary<string, List<(string, int)>>
            {
                ["A"] = new List<(string, int)> { ("B", 100) },
                ["B"] = new List<(string, int)> { ("A", 100), ("C", 5) },
                ["C"] = new List<(string, int)> { ("B", 5), ("D", 100) },
                ["D"] = new List<(string, int)> { ("C", 100), ("E", 100) },
                ["E"] = new List<(string, int)> { ("D", 100) }
            };
            var nodes = new List<string> { "A", "B", "C", "D", "E" };
            var lab6 = new Lab6(graph, nodes);

            var (maxFlow, path) = lab6.FindOptimalTransportPath("A", "E");

            // Узкое место B-C с пропускной способностью 5
            Assert.Equal(5, maxFlow);
        }
    }
}
