using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Objects.Evaluations;
using ACadSharp.Tables;
using CSMath;
using System.Xml.Linq;

namespace ACadSvg.Test;

public class DrawOrderTests {
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void BlockConversionPreservesSortTableOrder(bool dynamicBlock) {
        var document = new CadDocument();
        var block = new BlockRecord("DrawOrder");
        document.BlockRecords.Add(block);
        var first = new Line { EndPoint = new XYZ(1, 0, 0) };
        var second = new Line { EndPoint = new XYZ(2, 0, 0) };
        var third = new Line { EndPoint = new XYZ(3, 0, 0) };
        block.Entities.Add(first);
        block.Entities.Add(second);
        block.Entities.Add(third);
        // Swap the first and last sort keys; leave the middle entity absent
        // from the table to exercise ACadSharp's entity-handle fallback.
        var table = block.CreateSortEntitiesTable();
        table.Add(first, third.Handle);
        table.Add(third, first.Handle);
        var expected = new Entity[] { third, second, first };
        Assert.Equal(expected, block.GetSortedEntities().ToArray());

        if (dynamicBlock) {
            var visibility = new BlockVisibilityParameter();
            visibility.Entities.AddRange(new Entity[] { first, second, third });
            for (int i = 0; i < 10; i++) {
                var state = new BlockVisibilityParameter.State { Name = "State" + i };
                // Membership order intentionally differs from drawing order.
                state.Entities.AddRange(i % 2 == 0
                    ? new Entity[] { first, second, third }
                    : new Entity[] { second, first });
                visibility.AddState(state);
            }
            var graph = new EvaluationGraph();
            graph.CreateNode().Expression = visibility;
            block.CreateExtendedDictionary().Add(EvaluationGraph.DictionaryEntryName, graph);
        }

        var converted = new BlockRecordSvg(block, new ConversionContext());
        var svg = XElement.Parse(converted.ToSvgElement().ToString());
        if (dynamicBlock) {
            var groups = svg.Elements().ToArray();
            Assert.Equal(10, groups.Length);
            for (int i = 0; i < groups.Length; i++) {
                AssertOrder(groups[i], i % 2 == 0 ? expected : new Entity[] { second, first });
            }
        }
        else {
            AssertOrder(svg, expected);
        }
    }

    [Fact]
    public void AlwaysVisibleEntitiesAreSortedAndEmittedOnlyOnce() {
        var document = new CadDocument();
        var block = new BlockRecord("WithFreeEntities");
        document.BlockRecords.Add(block);
        var first = new Line { EndPoint = new XYZ(1, 0, 0) };
        var second = new Line { EndPoint = new XYZ(2, 0, 0) };
        var stateEntity = new Line { EndPoint = new XYZ(3, 0, 0) };
        block.Entities.Add(first);
        block.Entities.Add(second);
        block.Entities.Add(stateEntity);
        var table = block.CreateSortEntitiesTable();
        table.Add(first, second.Handle);
        table.Add(second, first.Handle);
        var visibility = new BlockVisibilityParameter();
        visibility.Entities.Add(stateEntity);
        var state = new BlockVisibilityParameter.State { Name = "State" };
        state.Entities.Add(stateEntity);
        visibility.AddState(state);
        var graph = new EvaluationGraph();
        graph.CreateNode().Expression = visibility;
        block.CreateExtendedDictionary().Add(EvaluationGraph.DictionaryEntryName, graph);

        var converted = new BlockRecordSvg(block, new ConversionContext());
        var svg = XElement.Parse(converted.ToSvgElement().ToString());
        var groups = svg.Elements().ToArray();
        Assert.Equal(2, groups.Length);
        AssertOrder(groups[0], new Entity[] { second, first });
        AssertOrder(groups[1], new Entity[] { stateEntity });
    }
    private static void AssertOrder(XElement group, Entity[] expected) {
        Assert.Equal(expected.Select(e => e.Handle.ToString("X")),
            group.Elements().Select(e => (string?)e.Attribute("id")));
    }
}