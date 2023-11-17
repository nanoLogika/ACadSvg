#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Text;
using ACadSharp.Entities;
using ACadSharp.Tables;
using SvgElements;
using CSMath;


namespace ACadSvg {

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="DimensionLinear"/> entity.
    /// The <see cref="DimensionLinear"/> entity is complex element including several <i>path</i>
    /// elements for the dimension lines, extension lines, filled <i>path</i> elements for the
    /// standard arrowheads, and finally  <i>text</i> element for the mesurement.
    /// </summary>
    /// <remarks><para>
    /// The converter for the <see cref="DimensionLinear"/> entity is not yet fully implemented.
    /// Only vertical dimension lines and standard arrowheads are supported.
    /// </para></remarks>
    internal class DimensionLinearSvg : EntitySvg {

        private DimensionLinear _linDim;
        private ConversionContext _ctx;


        /// <summary>
		/// Initializes a new instance of the <see cref="DimensionLinearSvg"/> class
		/// for the specified <see cref="DimensionLinear"/> entity.
        /// </summary>
        /// <param name="linDim">The <see cref="DimensionLinear"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public DimensionLinearSvg(Entity linDim, ConversionContext ctx) {
            _linDim = (DimensionLinear)linDim;
            _ctx = ctx;

            SetStandardIdAndClassIf(linDim, ctx);
		}


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
			GroupElement groupElement = new GroupElement();
            groupElement.WithID(ID).WithClass(Class);

			var x1 = _linDim.FirstPoint.X;
            var y1 = _linDim.FirstPoint.Y;
            var x2 = _linDim.SecondPoint.X;
            var y2 = _linDim.SecondPoint.Y;

            var xd2 = _linDim.DefinitionPoint.X;
            var yd2 = _linDim.DefinitionPoint.Y;

            //  TODO Verify tht this is correct if dimension rotation is not 0, 90, 180 270
            var xd1 = _linDim.DefinitionPoint.X;
            var yd1 = _linDim.DefinitionPoint.Y - y2 + y1;

            var extLineRot = _linDim.ExtLineRotation;
            var rot = _linDim.Rotation;

            //  vertical only!
            int verticalUp = (int)Math.Round(Math.Sin(rot));

            double textMidX = _linDim.TextMiddlePoint.X;
            double textMidY = _linDim.TextMiddlePoint.Y;
            double textRot = 90; //    Cd(_linDim.TextRotation * 180 / Math.PI);

            DimensionStyle dimStyle = _linDim.Style;
            short decimalPlaces = dimStyle.DecimalPlaces;
            double arrowSize = verticalUp * dimStyle.ArrowSize * dimStyle.ScaleFactor;
            double extLineExt = dimStyle.ExtensionLineExtension * dimStyle.ScaleFactor;
            double extLineOffset = dimStyle.ExtensionLineOffset;
            double d = Math.Round(x1 - xd1);
            if (d == 0) {
                extLineExt = 0;
            }
            else if (d > 0) {
                extLineExt = -extLineExt;
            }

            string text = Cd(Math.Round(_linDim.Measurement, decimalPlaces));

            double textSize = TextUtils.GetTextSize(false, dimStyle.TextHeight, dimStyle.Style, dimStyle.ScaleFactor);
            string textAnchor = TextUtils.HorizontalAlignmentToTextAnchor(dimStyle.TextHorizontalAlignment);

            //sb.AppendLine($"<circle cx=\"{Cd(x1)}\" cy=\"{Cd(y1)}\" r=\"10\" stroke=\"magenta\" stroke-width=\"2\"></circle>");
            //sb.AppendLine($"<circle cx=\"{Cd(x2)}\" cy=\"{Cd(y2)}\" r=\"10\" stroke=\"magenta\" stroke-width=\"2\"></circle>");
            //sb.AppendLine($"<circle cx=\"{Cd(xd1)}\" cy=\"{Cd(yd1)}\" r=\"10\" stroke=\"green\" stroke-width=\"2\"></circle>");
            //sb.AppendLine($"<circle cx=\"{Cd(xd2)}\" cy=\"{Cd(yd2)}\" r=\"10\" stroke=\"magenta\" stroke-width=\"2\"></circle>");

            string extensionLineColor = ColorUtils.GetHtmlColor(_linDim, _linDim.Style.ExtensionLineColor);
            string dimensionLineColor = ColorUtils.GetHtmlColor(_linDim, _linDim.Style.DimensionLineColor);
            string arrowColor = dimensionLineColor;
            string textColor = ColorUtils.GetHtmlTextColor(_linDim, _linDim.Style.TextColor);
            double extensionLineWidth = 1.5;
            double dimensionLineWidth = 1.5;
            double arrowLineWidth = 1.5;

			//  Extension Lines
			groupElement.Children.Add(new PathElement().AddLine(x1, y1, xd1 + extLineExt, yd1).WithStroke(extensionLineColor, extensionLineWidth));
			groupElement.Children.Add(new PathElement().AddLine(x2, y2, xd2 + extLineExt, yd2).WithStroke(extensionLineColor, extensionLineWidth));

			//  Dimension Line
			groupElement.Children.Add(new PathElement().AddLine(xd1, yd1, xd2, yd2).WithStroke(dimensionLineColor, dimensionLineWidth));

			//  Arrows
			groupElement.Children.Add(new PathElement()
                .AddLine(xd1, yd1, xd1, yd1 - 1.6 * arrowSize)
                .WithStroke(arrowColor, arrowLineWidth));
			groupElement.Children.Add(new PathElement()
                .AddLine(xd2, yd2, xd2, yd2 + 1.6 * arrowSize)
                .WithStroke(arrowColor, arrowLineWidth));

            XYZ arrow1Direction = new XYZ(0, arrowSize, 0);
            XYZ arrow2Direction = new XYZ(0, -arrowSize, 0);
			groupElement.Children.Add(XElementFactory.CreateStandardArrowHead(new XYZ(xd1, yd1, 0), arrow1Direction, arrowColor));
			groupElement.Children.Add(XElementFactory.CreateStandardArrowHead(new XYZ(xd2, yd2, 0), arrow2Direction, arrowColor));

            //  Measurement Text
            TextUtils.StyleToValues(dimStyle.Style, textSize, out string fontFamily, out double fontSize, out bool bold, out bool italic);

			groupElement.Children.Add(new TextElement()
                .WithXY(textMidX, textMidY)
                .WithTextAnchor(textAnchor)
                .WithFont(fontFamily, fontSize, bold, italic)
                .WithValue(text)
                .WithStroke("none")
                .WithFill(textColor)
                .ReverseY(_ctx.ConversionOptions.ReverseY)
                .AddRotate(textRot, textMidX, textMidY));

            return groupElement;
        }
    }
}