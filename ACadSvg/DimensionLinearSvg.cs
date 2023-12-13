#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;

using CSMath;

using SvgElements;


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
        private DimensionStyle _dimStyle;


        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionLinearSvg"/> class
        /// for the specified <see cref="DimensionLinear"/> entity.
        /// </summary>
        /// <param name="linDim">The <see cref="DimensionLinear"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public DimensionLinearSvg(Entity linDim, ConversionContext ctx) : base(ctx) {
            _linDim = (DimensionLinear)linDim;
            _dimStyle = _linDim.Style;
            _ctx = ctx;

            SetStandardIdAndClassIf(linDim, ctx);
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            GroupElement groupElement = new GroupElement();
            groupElement.WithID(ID).WithClass(Class);

            XY p1 = Utils.ToXY(_linDim.FirstPoint);
            XY p2 = Utils.ToXY(_linDim.SecondPoint);
            XY dp2 = Utils.ToXY(_linDim.DefinitionPoint);

            double dirp1p2 = (p2 - p1).GetAngle();
            double dirp1dp2 = (dp2 - p1).GetAngle();
            bool ccw = Math.Sin(dirp1dp2 - dirp1p2) > 0;

            XYZ n = XYZ.Cross(_linDim.SecondPoint - _linDim.FirstPoint, _linDim.DefinitionPoint - _linDim.FirstPoint);
            ccw = n.Z > 0;

            //  TODO Find out what these properties are for
            var dimRot = _linDim.Rotation;
            var dimHor = _linDim.HorizontalDirection;
            var extLineRot = _linDim.ExtLineRotation;

            double extLineExt = _dimStyle.ExtensionLineExtension * _dimStyle.ScaleFactor;
            double extLineOffset = _dimStyle.ExtensionLineOffset * _dimStyle.ScaleFactor;

            XY dext2 = dp2 - p2;
            double dext2el = dext2.GetLength() + extLineExt;
            double dext2al = extLineOffset;
            XY dextn = dext2.Normalize();
            XY dext2e = p2 + dext2el * dextn;
            XY dext2a = p2 + dext2al * dextn;

            XY dimDir = ccw ? new XY(-dextn.Y, dextn.X) : new XY(dextn.Y, -dextn.X);
            XY dp1 = dp2 + dimDir * _linDim.Measurement;
            XY dext1 = dp1 - p1;
            double dext1el = dext1.GetLength() + extLineExt;
            double dext1al = extLineOffset;
            XY dext1n = dext1.Normalize();
            XY dext1e = p1 + dext1el * dext1n;
            XY dext1a = p1 + dext1al * dext1n;

            string extensionLineColor = ColorUtils.GetHtmlColor(_linDim, getExtensionLineColor());
            string dimensionLineColor = ColorUtils.GetHtmlColor(_linDim, getDimensionLineColor());
            string arrowColor = dimensionLineColor;
            string textColor = ColorUtils.GetHtmlTextColor(_linDim, getTextColor());
            double? extensionLineWidth = LineUtils.GetLineWeight(_dimStyle.ExtensionLineWeight, _linDim, _ctx);
            double? dimensionLineWidth = LineUtils.GetLineWeight(_dimStyle.DimensionLineWeight, _linDim, _ctx);
            double? arrowLineWidth = dimensionLineWidth;

            var arr1Block = getArrowHeadBlock1();
            var arr2Block = getArrowHeadBlock2();
            
            //  Extension Lines
            groupElement.Children.Add(new PathElement().AddLine(dext2a.X, dext2a.Y, dext2e.X, dext2e.Y).WithStroke(extensionLineColor).WithStrokeWidth(extensionLineWidth));
            groupElement.Children.Add(new PathElement().AddLine(dext1a.X, dext1a.Y, dext1e.X, dext1e.Y).WithStroke(extensionLineColor).WithStrokeWidth(extensionLineWidth));

            //  Arrows
            double arrowSize = _dimStyle.ArrowSize * _dimStyle.ScaleFactor;
            var outside = 2.8 * arrowSize > _linDim.Measurement ? -1 : 1;
            int arrow1Outside = outside * (_linDim.FlipArrow1 ? -1 : 1);
            int arrow2Outside = outside * (_linDim.FlipArrow2 ? -1 : 1);

            XYZ arrowPointDp1 = new XYZ(dp1.X, dp1.Y, 0);
            XYZ arrowPointDp2 = new XYZ(dp2.X, dp2.Y, 0);
            XYZ arrow1Direction = new XYZ(dimDir.X, dimDir.Y, 0) * arrowSize * arrow1Outside;
            XYZ arrow2Direction = new XYZ(-dimDir.X, -dimDir.Y, 0) * arrowSize * arrow2Outside;

            //  Measurement Text
            XY textMid = Utils.ToXY(_linDim.TextMiddlePoint);
            XY txtOnDimLin = dp1 + dimDir * dimDir.Dot(textMid - dp1);
            double textSize = TextUtils.GetTextSize(_dimStyle.TextHeight) * _dimStyle.ScaleFactor;
            double textRot = (getTextRot(dimDir.GetAngle()) + _linDim.TextRotation) * 180 / Math.PI;
            string text = _linDim.Text;
            double textLen = GetTextLength(text, textSize);
            bool textInside = (dp2 - dp1).GetLength() > (txtOnDimLin - dp1).GetLength();
            bool withLeader = (textMid - txtOnDimLin).GetLength() > textSize * 1.4 && getTextMovement() == TextMovement.AddLeaderWhenTextMoved;
            bool dim1ext = arrow1Outside == -1;
            bool dim2ext = (arrow2Outside == -1) || (!textInside && !withLeader);
            bool flipArrows = _linDim.FlipArrow1 && _linDim.FlipArrow2;

            // Arrow Extension get start- and endpoint
            double dimLineExtFactor = outside == -1 ? arrowSize * 2 : arrowSize * 0;
            XY dimExtDp1Start = flipArrows ? dp1 - dimDir * (dimLineExtFactor - arrowSize) : dp1 + dimDir * (dimLineExtFactor - arrowSize);
            XY dimExtDp1End = dimExtDp1Start + dimDir * arrowSize;
            XY dimExtDp2Start = flipArrows ? dp2 + dimDir * (dimLineExtFactor - arrowSize) : dp2 - dimDir * (dimLineExtFactor - arrowSize);
            XY dimExtDp2End = textInside ? dimExtDp2Start - dimDir * arrowSize : txtOnDimLin - dimDir * textLen * 0.5;  // + Halbe Textbreite

            //  Do not draw dimension line through arrow head
            //  TODO this is not 
            XY dl1 = arrow1Outside == -1 ? dp1 : dp1 - dimDir * arrowSize;
            XY dl2 = arrow2Outside == -1 ? dp2 : dp2 + dimDir * arrowSize;

            groupElement.Children.Add(new PathElement().AddLine(dl1.X, dl1.Y, dl2.X, dl2.Y).WithStroke(dimensionLineColor).WithStrokeWidth(dimensionLineWidth));

            if (arr1Block != null) {
                groupElement.Children.Add(XElementFactory.CreateArrowhead(arr1Block, arrowPointDp1, arrowSize, arrow1Direction));
            }
            else {
                groupElement.Children.Add(XElementFactory.CreateStandardArrowHead(new XYZ(dp1.X, dp1.Y, 0), arrow1Direction, arrowColor));
            }

            if (arr2Block != null) {
                groupElement.Children.Add(XElementFactory.CreateArrowhead(arr2Block, arrowPointDp2, arrowSize, arrow2Direction));
            }
            else {
                groupElement.Children.Add(XElementFactory.CreateStandardArrowHead(new XYZ(dp2.X, dp2.Y, 0), arrow2Direction, arrowColor));
            }

            string textAnchor;
            if (withLeader) {
                XY dimMid = GetMidpoint(dp1, dp2);
                XY textEnd = textMid - dimDir * textLen;
                groupElement.Children.Add(new PathElement()
                    .AddLine(dimMid.X, dimMid.Y, textMid.X, textMid.Y)
                    .AddLine(textEnd.X, textEnd.Y)
                    .WithStroke(dimensionLineColor).WithStrokeWidth(dimensionLineWidth));

                if (isCcwPath(dimMid, textMid, textEnd)) {
                    textAnchor = string.Empty;
                }
                else {
                    textAnchor = "end";
                }
            }
            else {
                textAnchor = TextUtils.HorizontalAlignmentToTextAnchor(_dimStyle.TextHorizontalAlignment);
            }

            // Dimension line extension 1
            if (dim1ext) {
                groupElement.Children.Add(new PathElement().AddLine(dimExtDp1Start.X, dimExtDp1Start.Y, dimExtDp1End.X, dimExtDp1End.Y).WithStroke(dimensionLineColor).WithStrokeWidth(dimensionLineWidth));
            }

            // Dimension line extension 2 
            if (dim2ext) {
                groupElement.Children.Add(new PathElement().AddLine(dimExtDp2Start.X, dimExtDp2Start.Y, dimExtDp2End.X, dimExtDp2End.Y).WithStroke(dimensionLineColor).WithStrokeWidth(dimensionLineWidth));
            }

            var attachmentPoint = _linDim.AttachmentPoint;
            var verticalAlignment = _dimStyle.TextVerticalAlignment;
            var verticalPosition = _dimStyle.TextVerticalPosition;
            
            TextUtils.StyleToValues(_dimStyle.Style, textSize, out string fontFamily, out double fontSize, out bool bold, out bool italic);

            groupElement.Children.Add(new TextElement()
                .WithXY(textMid.X, textMid.Y)
                .WithTextAnchor(textAnchor)
                .WithFont(fontFamily, fontSize, bold, italic)
                .WithValue(text)
                .WithStroke("none")
                .WithFill(textColor)
                .ReverseY(_ctx.ConversionOptions.ReverseY)
                .AddRotate(textRot, textMid.X, textMid.Y));

            return groupElement;
        }

        private static bool isCcwPath(XY p0, XY p1, XY p2) {
            double a1 = (p1 - p0).GetAngle();
            if (a1 < 0) {
                a1 += Math.Tau;
            }
            double a2 = (p2 - p1).GetAngle();
            if (a2 < 0) {
                a2 += Math.Tau;
            }
            return a1 > a2;
        }


        private TextMovement getTextMovement() {
            var rec = GetExtendedDataRecord(_linDim, "ACAD", "DSTYLE", 279);
            if (rec != null) {
                return (TextMovement)rec.Value;
            }
            return _dimStyle.TextMovement;
        }


        private Color getDimensionLineColor() {
            if (getExtendedDataColor(176, out Color color)) {
                return color;
            }
            return _dimStyle.DimensionLineColor;
        }


        private Color getExtensionLineColor() {
            if (getExtendedDataColor(177, out Color color)) {
                return color;
            }
            return _dimStyle.ExtensionLineColor;
        }


        private Color getTextColor() {
            if (getExtendedDataColor(178, out Color color)) {
                return color;
            }
            return _linDim.Style.TextColor;
        }


        private bool getExtendedDataColor(short field, out Color color) {
            var recTc = GetExtendedDataRecord(_linDim, "Acad_TC", "DSTYLE_TC", field);
            if (recTc != null) {
                byte[] bytes = recTc.Value as byte[];
                var r = bytes[10];
                var g = bytes[9];
                var b = bytes[8];
                color = new Color(r, g, b);
                return true;
            }
            var rec = GetExtendedDataRecord(_linDim, "ACAD", "DSTYLE", field);
            if (rec != null) {
                color = new Color((short)rec.Value);
                return true;
            }
            color = Color.ByBlock;
            return false;
        }


        private BlockRecord getArrowHeadBlock1() {
            BlockRecord arrBlock = getArrowHeadBlock(343);
            if (arrBlock != null) {
                return arrBlock;
            }
            return _dimStyle.DimArrow1;
        }
        

        private BlockRecord getArrowHeadBlock2() {
            BlockRecord arrBlock = getArrowHeadBlock(344);
            if (arrBlock != null) {
                return arrBlock;
            }
            return _dimStyle.DimArrow2;
        }


        private BlockRecord getArrowHeadBlock(short field) {
            var rec = GetExtendedDataRecord(_linDim, "ACAD", "DSTYLE", field);
            if (rec == null) {
                return null;
            }
            var arrHandle = ReverseBytes((ulong)rec.Value);
            if (!_linDim.Document.TryGetCadObject<BlockRecord>(arrHandle, out BlockRecord arrBlock)) {
                return null;
            }
            return arrBlock;
        }


        private static double getTextRot(double dimDirRot) {
            double textRot = dimDirRot;
            if (textRot > Math.PI / 2 && textRot < Math.PI * 3 / 2) {
                textRot -= Math.PI;
            }
            else if (textRot <= -Math.PI / 2 && textRot > -Math.PI * 3 / 2) {
                textRot += Math.PI;
            }

            return textRot;
        }


        public static ulong ReverseBytes(ulong value) {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }


        private static XY GetMidpoint(XY point1, XY point2) {
            double midX = (point1.X + point2.X) / 2;
            double midY = (point1.Y + point2.Y) / 2;
            return new XY(midX, midY);
        }


        private static (XY startPoint, XY endPoint) GetTextElementEndpoints(XY textMidpoint, double textRotation, double textSize, string text) {

            double textRot = textRotation * Math.PI / 180;
            double textLength = GetTextLength(text, textSize);
            double offsetX = (textLength / 2) * Math.Cos(textRot);
            double offsetY = (textLength / 2) * Math.Sin(textRot);

            XY offsetVector = new XY(offsetX, offsetY);
            XY startPoint = textMidpoint - offsetVector; 
            XY endPoint = textMidpoint + offsetVector;  

            return (startPoint, endPoint);
        }


        private static double GetTextLength(string text, double textSize) {
            int characterCount = text.Length;
            double textLength = characterCount * textSize * 0.5;
            return textLength;
        }
    }
}