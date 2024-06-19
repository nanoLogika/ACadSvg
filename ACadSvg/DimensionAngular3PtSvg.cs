#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;
using CSMath;
using ACadSharp.Tables;

namespace ACadSvg {

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="DimensionAngular3Pt"/> entity.
    /// The <see cref="DimensionAngular3Pt"/> entity is converted into a complex element including
    /// several <i>path</i> elements for the dimension-line arc, extension lines, filled <i>path</i>
    /// elements for the standard arrowheads, and finally a <i>text</i> element for the mesurement.
    /// </summary>
    internal class DimensionAngular3PtSvg : AngularDimensionSvg {

        private DimensionAngular3Pt _ang3PtDim;


        /// <summary>
		/// Initializes a new instance of the <see cref="DimensionAngular3PtSvg"/> class
		/// for the specified <see cref="DimensionAngular3Pt"/> entity.
        /// </summary>
        /// <param name="ang3PtDim">The <see cref="DimensionAngular3Pt"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public DimensionAngular3PtSvg(Entity ang3PtDim, ConversionContext ctx) : base(ang3PtDim, ctx) {
            _ang3PtDim = (DimensionAngular3Pt)ang3PtDim;
            _defaultPostFix = string.Empty;
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            CreateGroupElement();

            XY arcCenter = _ang3PtDim.AngleVertex.ToXY();
            XY definitionPoint = _ang3PtDim.DefinitionPoint.ToXY();
            XY firstPoint = _ang3PtDim.FirstPoint.ToXY();
            XY secondPoint = _ang3PtDim.SecondPoint.ToXY();
            XY textMid = _ang3PtDim.TextMiddlePoint.ToXY();
            double r = (definitionPoint - arcCenter).GetLength();
            XY textOnDimLin = arcCenter + (textMid - arcCenter).Normalize() * r;

            XY firstExtDir = (firstPoint - arcCenter).Normalize();
            XY secondExtDir = (secondPoint - arcCenter).Normalize();

            XY firstArcPoint = arcCenter + firstExtDir * r;
            XY secondArcPoint = arcCenter + secondExtDir * r;

            BlockRecord arrowBlock1 = _dimProps.ArrowHeadBlock1;
            BlockRecord arrowBlock2 = _dimProps.ArrowHeadBlock2;

            //  Debug+
            CreateDebugPoint(textOnDimLin, "blue");
            CreateDebugPoint(textMid, "blue");
            CreateDebugPoint(definitionPoint, "red");
            CreateDebugPoint(arcCenter, "aqua");
            //  -Debug

            //  Extension lines
            CreateFirstExtensionLine(firstPoint, firstArcPoint, firstExtDir);
            CreateSecondExtensionLine(secondPoint, secondArcPoint, secondExtDir);

            double firstAngle = firstExtDir.GetAngle();
            double secondAngle = secondExtDir.GetAngle();
            bool flipped = secondAngle < firstAngle;

            CreateDimensionLineArc(arcCenter, r, flipped ? secondAngle : firstAngle, flipped ? firstAngle : secondAngle);

            //  Arrows
            GetArrowsOutside(r * (secondAngle - firstAngle), out bool firstArrowOutside, out bool secondArrowOutside);
            GetArrorwsDirection(r, firstAngle, secondAngle, firstArrowOutside, secondArrowOutside, out double alpha, out XY firstArrowDirection, out XY secondArrowDirection);

            CreateArrowHead(arrowBlock1, firstArcPoint, firstArrowDirection);
            if (firstArrowOutside) {
                CreateDimensionLineArc(arcCenter, r, firstAngle - 4 * alpha, firstAngle - 2 * alpha);
            }

            CreateArrowHead(arrowBlock2, secondArcPoint, secondArrowDirection);
            if (secondArrowOutside) {
                CreateDimensionLineArc(arcCenter, r, secondAngle + 2 * alpha, secondAngle + 4 * alpha);
            }

            //  Measurement text  
            double rot = (textMid - arcCenter).GetAngle() + Math.PI / 2;
            CreateTextElement(textOnDimLin, rot, out double textLen);

            return _groupElement;
        }
    }
}