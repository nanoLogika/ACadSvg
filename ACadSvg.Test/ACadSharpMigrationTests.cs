using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Objects.Evaluations;
using ACadSharp.Tables;
using ACadSvg.DimensionTextFormatter;
using ACadSvg.Extensions;
using CSMath;

namespace ACadSvg.Test;

public class ACadSharpMigrationTests {
    [Theory]
    [InlineData(AngularZeroHandling.DisplayAll, "0.50")]
    [InlineData(AngularZeroHandling.SuppressLeadingZeroes, ".50")]
    [InlineData(AngularZeroHandling.SupressTrailingZeroes, "0.5")]
    [InlineData(AngularZeroHandling.SupressAll, ".5")]
    public void AngularZeroSuppressionFormatsDecimalDegrees(AngularZeroHandling handling, string expected) {
        double angle = Math.PI / 360;
        var dimension = new DimensionAngular3Pt {
            AngleVertex = XYZ.Zero,
            FirstPoint = new XYZ(1, 0, 0),
            SecondPoint = new XYZ(Math.Cos(angle), Math.Sin(angle), 0)
        };
        var style = new DimensionStyle("Test") {
            AngularZeroHandling = handling,
            AngularDecimalPlaces = 2,
            DecimalSeparator = '.',
            ScaleFactor = 1
        };
        var document = new CadDocument();
        document.Entities.Add(dimension);
        var formatter = new DecimalDegreesMeasurementFormatter(
            dimension, new DimensionProperties(dimension, style), "<>", 10);
        Assert.Equal(expected, formatter.FormatMeasurement());
    }

    [Fact]
    public void VisibilityDictionaryCreatesSeparateSvgGroups() {
        var block = new BlockRecord("DynamicBlock");
        var first = new Line { StartPoint = XYZ.Zero, EndPoint = new XYZ(10, 0, 0) };
        var second = new Line { StartPoint = XYZ.Zero, EndPoint = new XYZ(0, 10, 0) };
        block.Entities.Add(first);
        block.Entities.Add(second);
        var visibility = new BlockVisibilityParameter();
        visibility.Entities.Add(first);
        visibility.Entities.Add(second);
        var stateA = new BlockVisibilityParameter.State { Name = "StateA" };
        var stateB = new BlockVisibilityParameter.State { Name = "StateB" };
        stateA.Entities.Add(first);
        stateB.Entities.Add(second);
        visibility.AddState(stateA);
        visibility.AddState(stateB);
        var graph = new EvaluationGraph();
        graph.CreateNode().Expression = visibility;
        block.CreateExtendedDictionary().Add(EvaluationGraph.DictionaryEntryName, graph);
        var document = new CadDocument();
        document.BlockRecords.Add(block);
        var context = new ConversionContext();
        var converted = new BlockRecordSvg(block, context);
        Assert.Equal(2, converted.Children.Count);
        var groups = converted.Children.Cast<GroupSvg>().ToArray();
        Assert.Equal(context.ConversionOptions.BlockVisibilityParametersPrefix + "StateA", groups[0].ID);
        Assert.Equal(context.ConversionOptions.BlockVisibilityParametersPrefix + "StateB", groups[1].ID);
        Assert.Single(groups[0].Children);
        Assert.Single(groups[1].Children);
        Assert.Contains("M 0 0 L 10 0", groups[0].ToSvgElement().ToString());
        Assert.Contains("M 0 0 L 0 10", groups[1].ToSvgElement().ToString());
    }
}