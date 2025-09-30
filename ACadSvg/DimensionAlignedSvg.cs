#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using ACadSharp.Tables;

using CSMath;

using SvgElements;


namespace ACadSvg {

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="DimensionAligned"/> entity.
    /// The <see cref="DimensionAligned"/> entity is a complex element including dimension lines,
    /// extension lines, arrowheads, and a text element for the measurement.
    /// </summary>
    internal class DimensionAlignedSvg : DimensionSvg {

        protected DimensionAligned _aliDim;

        public DimensionAlignedSvg(Entity aliDim, ConversionContext ctx) : base(aliDim, ctx) {
            _aliDim = (DimensionAligned)aliDim;
            _defaultPostFix = "<>";
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            CreateGroupElement();

            // Get points for measurement
            XY p1 = _aliDim.FirstPoint.ToXY();
            XY p2 = _aliDim.SecondPoint.ToXY();
            XY dp2 = _aliDim.DefinitionPoint.ToXY();

            //  TODO Find out what these properties are for
            double dimHor = _aliDim.HorizontalDirection;
            double extLineRot = _aliDim.ExtLineRotation;

            CalculateDirections(
                p1, p2, dp2,
                out XY dp1, out XY dimDir, out XY dext1Dir, out XY dext2Dir);

            //  Get arrow position, direction
            GetArrowsOutside(_aliDim.Measurement, out bool firstArrowOutside, out bool secondArrowOutside);
            XY arrow1Direction = dimDir * (firstArrowOutside ? -1 : 1);
            XY arrow2Direction = -dimDir * (secondArrowOutside ? -1 : 1);

            //  Get individual arrowheads
            var arr1Block = _dimProps.ArrowHeadBlock1;
            var arr2Block = _dimProps.ArrowHeadBlock2;

            //  Text
            double textSize = TextUtils.GetTextSize(_dimProps.TextHeight);
            double textRot = (GetTextRot(dimDir.GetAngle()) + _aliDim.TextRotation) * 180 / Math.PI;
            const double noTextLen = 0;

            XY dimMid = GetMidpoint(dp1, dp2);
            XY textMid = _aliDim.TextMiddlePoint.ToXY();
            XY textOnDimLin;
            if (_aliDim.IsTextUserDefinedLocation) {
                textOnDimLin = dp1 + dimDir * dimDir.Dot(textMid - dp1);
            }
            else {
                textOnDimLin = dimMid;
            }
            bool withLeader = (textMid - textOnDimLin).GetLength() > textSize * 1.4 &&
                              _dimProps.TextMovement == TextMovement.AddLeaderWhenTextMoved;

            double textLen = noTextLen;
            if (withLeader) {
                //  Draw measuring text and leader element
                CreateTextElementAndLeader(dimMid, textMid, dimDir, textRot);
            }
            else {
                //  Draw measuring mext
                XY textPos = textOnDimLin;
                CreateTextElement(textPos, dimDir.GetAngle(), out textLen);
            }

            bool textInside = (dp2 - dp1).GetLength() > (textOnDimLin - dp1).GetLength();

            //  Calculate dimension line length
            XY dl1 = CalculateDimensionLineEnd(dimDir, dp1, firstArrowOutside, arr1Block, _dimProps.DimensionLineExtension);
            XY dl2 = CalculateDimensionLineEnd(-dimDir, dp2, secondArrowOutside, arr2Block, _dimProps.DimensionLineExtension);

            //  Draw lines
            CreateFirstExtensionLine(p1, dp1, dext1Dir);
            CreateSecondExtensionLine(p2, dp2, dext2Dir);
            CreateDimensionLine(dl1, dl2);

            //  Draw dimension line extensions
            CreateDimensionLineExtension(
                dp1, dp1 + dimDir * 2 * _arrowSize, dimDir, _arrowSize,
                false, firstArrowOutside, noTextLen);
            CreateDimensionLineExtension(
                dp2, textOnDimLin, -dimDir, _arrowSize,
                !textInside, secondArrowOutside, textLen);

            //  Draw arrow heads
            CreateArrowHead(arr1Block, dp1, arrow1Direction);
            CreateArrowHead(arr2Block, dp2, arrow2Direction);

            //  TODO
            var attachmentPoint = _aliDim.AttachmentPoint;
            var verticalAlignment = _dimProps.TextVerticalAlignment;
            var verticalPosition = _dimProps.TextVerticalPosition;

            CreateDebugPoint(p1, "aqua");
            CreateDebugPoint(p2, "aqua");
            CreateDebugPoint(dp1, "green");
            CreateDebugPoint(dp2, "red");
            CreateDebugPoint(textMid, "blue");

            //  Return group containg all drawing elements 
            return _groupElement;
        }


        protected virtual void CalculateDirections(
            XY p1, XY p2, XY dp2,
            out XY dp1, out XY dimDir, out XY dext1Dir, out XY dext2Dir) {

            //  p1 --> p2
            XY dirVec = (p2 - p1).Normalize();
            XY ortho = dirVec.Perpendicular();

            // Dimension line direction (parallel zu p1 --> p2, durch DefinitionPoint verschoben)
            dimDir = -dirVec;
            dp1 = dp2 + dimDir * _aliDim.Measurement;

            // Extension line directions
            dext1Dir = (dp1 - p1).Normalize();
            dext2Dir = (dp2 - p2).Normalize();
        }


        protected XY CalculateDimensionLineEnd(XY dimDir, XY dp, bool arrowOutside, BlockRecord? arrBlock, double dimensionLineExtension) {
            if (arrBlock == null) {
                //  Standard arrow
                //  Dimension line shortened when arrows are inside
                return arrowOutside ? dp : dp - dimDir * _arrowSize;
            }
            else {
                //  Shorten lines for well-known standard arrows/end marks
                switch (arrBlock.Name) {
                case "_Oblique":
                case "_ArchTick":
                    //  Dimension extends beyond the extension line only with oblique strokes
                    return dp + dimDir * dimensionLineExtension;

                case "_ClosedBlank":
                    //  Dimension line shortened when arrows are inside
                    return arrowOutside ? dp : dp - dimDir * _arrowSize;

                default:
                    //  No extension/shortening
                    return dp;
                }
            }
        }
    }
}
