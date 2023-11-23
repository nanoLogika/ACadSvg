#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;
using CSMath;


namespace ACadSvg {

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="DimensionAngular3Pt"/> entity.
    /// The <see cref="DimensionAngular3Pt"/> entity is converted into a complex element including
    /// several <i>path</i> elements for the dimension-line arc, extension lines, filled <i>path</i>
    /// elements for the standard arrowheads, and finally a <i>text</i> element for the mesurement.
    /// </summary>
    /// <remarks><para>
    /// The converter for the <see cref="DimensionAngular3Pt"/> entity is not yet fully implemented.
    /// Currently no arrowheads other than teh standard arrow are supported.
    /// </para></remarks>
    internal class DimensionAngular3PtSvg : EntitySvg {

        private DimensionAngular3Pt _ang3PtDim;


        /// <summary>
		/// Initializes a new instance of the <see cref="DimensionAngular3PtSvg"/> class
		/// for the specified <see cref="DimensionAngular3Pt"/> entity.
        /// </summary>
        /// <param name="ang3PtDim">The <see cref="DimensionAngular3Pt"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public DimensionAngular3PtSvg(Entity ang3PtDim, ConversionContext ctx) : base(ctx) {
            _ang3PtDim = (DimensionAngular3Pt)ang3PtDim;
            SetStandardIdAndClassIf(ang3PtDim, ctx);
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
			GroupElement groupElement = new GroupElement();
            groupElement.WithID(ID).WithClass(Class);

			var angleVertex = _ang3PtDim.AngleVertex;
            var definitionPoint = _ang3PtDim.DefinitionPoint;
            var firstPoint = _ang3PtDim.FirstPoint;
            var secondPoint = _ang3PtDim.SecondPoint;

            var dimStyle = _ang3PtDim.Style;
            var decimalPlaces = dimStyle.DecimalPlaces;
            var arrowSize = dimStyle.ArrowSize * dimStyle.ScaleFactor;
            var extLineExt = dimStyle.ExtensionLineExtension * dimStyle.ScaleFactor;
            var extLineOffset = dimStyle.ExtensionLineOffset;

            double firstAngle = Math.Atan2(firstPoint.Y - angleVertex.Y, firstPoint.X - angleVertex.X);
            double secondAngle = Math.Atan2(secondPoint.Y - angleVertex.Y, secondPoint.X - angleVertex.X);
            bool flipped = secondAngle < firstAngle;
            if (flipped) {
                var p = firstPoint;
                firstPoint = secondPoint;
                secondPoint = p;
                var a = firstAngle;
                firstAngle = secondAngle;
                secondAngle = a;
            }

            double r = Math.Sqrt(Math.Pow(definitionPoint.X - angleVertex.X, 2) + Math.Pow(definitionPoint.Y - angleVertex.Y, 2));
            XYZ firstPointExt = angleVertex + new XYZ(r * Math.Cos(firstAngle), r * Math.Sin(firstAngle), 0);
            XYZ secondPointExt = angleVertex + new XYZ(r * Math.Cos(secondAngle), r * Math.Sin(secondAngle), 0);

            double extlineLength = r + extLineExt;
            XYZ firstPointExtExt = angleVertex + new XYZ(extlineLength * Math.Cos(firstAngle), extlineLength * Math.Sin(firstAngle), 0);
            XYZ secondPointExtExt = angleVertex + new XYZ(extlineLength * Math.Cos(secondAngle), extlineLength * Math.Sin(secondAngle), 0);

            string extensionLineColor = ColorUtils.GetHtmlColor(_ang3PtDim, dimStyle.ExtensionLineColor);
            string dimensionLineColor = ColorUtils.GetHtmlColor(_ang3PtDim, dimStyle.DimensionLineColor);
            string arrowColor = dimensionLineColor;
            string textColor = ColorUtils.GetHtmlTextColor(_ang3PtDim, dimStyle.TextColor);
            double? extensionLineWidth = LineUtils.GetLineWeight(dimStyle.ExtensionLineWeight, _ang3PtDim, _ctx);
            double? dimensionLineWidth = LineUtils.GetLineWeight(dimStyle.DimensionLineWeight, _ang3PtDim, _ctx);
            //  ?? double arrowLineWidth = 0.1;

            //  Extension lines
            var extensionLinePath = new PathElement()
                .AddLine(firstPoint.X, firstPoint.Y, firstPointExtExt.X, firstPointExtExt.Y)
                .AddLine(secondPoint.X, secondPoint.Y, secondPointExtExt.X, secondPointExtExt.Y)
                .WithStroke(extensionLineColor)
                .WithStrokeWidth(extensionLineWidth);
			groupElement.Children.Add(extensionLinePath);

            //  Arc dimension line
            var dimensionLinePath = new PathElement()
                .AddMoveAndArc(angleVertex.X, angleVertex.Y, firstAngle, secondAngle, r)
                .WithStroke(dimensionLineColor)
                .WithStrokeWidth(dimensionLineWidth);
            groupElement.Children.Add(dimensionLinePath);

            //  Arrows
            //  Sehne - kreis - winkel: s = 2 r sin(a/2)
            //  sin(a/2) = s / 2 / r
            double alpha = Math.Asin(arrowSize / 2 / r);
            double firstAngleAlpha = firstAngle + alpha;
            XYZ firstArrowDirection = new XYZ(arrowSize * Math.Sin(firstAngleAlpha), -arrowSize * Math.Cos(firstAngleAlpha), 0);
            double secondAngleAlpha = secondAngle - alpha;
            XYZ secondArrowDirection = new XYZ(-arrowSize * Math.Sin(secondAngleAlpha), arrowSize * Math.Cos(secondAngleAlpha), 0);

			groupElement.Children.Add(XElementFactory.CreateStandardArrowHead(firstPointExt, firstArrowDirection, arrowColor));
			groupElement.Children.Add(XElementFactory.CreateStandardArrowHead(secondPointExt, secondArrowDirection, arrowColor));

            //  Measurement text  
            double textSize = TextUtils.GetTextSize(false, dimStyle.TextHeight, dimStyle.Style, dimStyle.ScaleFactor);
            string angle = Cd(Math.Round(_ang3PtDim.Measurement * 180 / Math.PI, decimalPlaces)) + "°";
            string textAnchor = TextUtils.HorizontalAlignmentToTextAnchor(dimStyle.TextHorizontalAlignment);
            double midX = _ang3PtDim.TextMiddlePoint.X;
            double midY = _ang3PtDim.TextMiddlePoint.Y;
            //  TODO: flipped may not be the right criterion
            double rot = flipped ? -(firstAngle + secondAngle) / 2 * 180 / Math.PI : (firstAngle + secondAngle) / 2 * 180 / Math.PI - 90;

            TextUtils.StyleToValues(dimStyle.Style, textSize, out string fontFamily, out double fontSize, out bool bold, out bool italic);

            groupElement.Children.Add(new TextElement()
                .WithXY(midX, midY)
                .WithAlignmentBaseline("middle")
                .WithTextAnchor(textAnchor)
                .WithFont(fontFamily, fontSize, bold, italic)
                .WithValue(angle)
                .WithStroke("none")
                .WithStrokeWidth(LineUtils.GetLineWeight(_ang3PtDim.LineWeight, _ang3PtDim, _ctx))
                .WithFill(textColor)
                .WithComment("DIMENSION_ANG_3_Pt")
                .ReverseY(_ctx.ConversionOptions.ReverseY)
                .AddRotate(rot, midX, midY));

            return groupElement;
        }
    }
}