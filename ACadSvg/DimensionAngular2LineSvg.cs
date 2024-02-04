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
    /// Represents an SVG element converted from an ACad <see cref="DimensionAngular2LineSvg"/> entity.
    /// The <see cref="DimensionAngular2LineSvg"/> entity is converted into a complex element including
    /// several <i>path</i> elements for the dimension-line arc, extension lines, filled <i>path</i>
    /// elements for the standard arrowheads, and finally a <i>text</i> element for the mesurement.
    /// </summary>
    internal class DimensionAngular2LineSvg : AngularDimensionSvg {

        private DimensionAngular2Line _ang2LineDim;


        public DimensionAngular2LineSvg(Entity entity, ConversionContext ctx) : base(entity, ctx) {
            _ang2LineDim = (DimensionAngular2Line)entity;
            _defaultPostFix = string.Empty;
        }


        public override SvgElementBase ToSvgElement() {
            CreateGroupElement();

            //  Begin and end of first and second line
            //  Arc center = intersect of first an second line
            //  radius
            XY firstLinePoint1 = Utils.ToXY(_ang2LineDim.FirstPoint);
            XY firstPoint = Utils.ToXY(_ang2LineDim.SecondPoint);
            XY secondLinePoint1 = Utils.ToXY(_ang2LineDim.AngleVertex);
            XY secondPoint = Utils.ToXY(_ang2LineDim.DefinitionPoint);
            XY arcCenter = evaluateArcCenter(firstLinePoint1, firstPoint, secondLinePoint1, secondPoint);
            //  Some point on the arc
            XY dimensionArc = Utils.ToXY(_ang2LineDim.DimensionArc);
            XY textMid = Utils.ToXY(_ang2LineDim.TextMiddlePoint);
            double r = (dimensionArc - arcCenter).GetLength();
            XY textOnDimLin = arcCenter + (textMid - arcCenter).Normalize() * r;

            XY firstExtDir = (firstPoint - firstLinePoint1).Normalize();
            XY secondExtDir = (secondPoint - secondLinePoint1).Normalize();
            double extLineExt = _dimProps.ExtensionLineExtension;
            var extLineOffset = _dimProps.ExtensionLineOffset;
            XY firstArcPoint = arcCenter + firstExtDir * r;
            XY secondArcPoint = arcCenter + secondExtDir * r;

            BlockRecord arrowBlock1 = _dimProps.ArrowHeadBlock1;
            BlockRecord arrowBlock2 = _dimProps.ArrowHeadBlock2;

            double firstAngle = firstExtDir.GetAngle();
            double secondAngle = secondExtDir.GetAngle();

            //  Debug+
            CreateDebugPoint(textOnDimLin, "blue");
            CreateDebugPoint(textMid, "blue");
            CreateDebugPoint(secondLinePoint1, "aqua");
            CreateDebugPoint(secondPoint, "pink");
            CreateDebugPoint(dimensionArc, "green");
            CreateDebugPoint(firstLinePoint1, "magenta");
            CreateDebugPoint(firstPoint, "yellow");
            CreateDebugPoint(arcCenter, "white");
            //  -Debug

            bool flipped = secondAngle < firstAngle;

            CreateDimensionLineArc(arcCenter, r, flipped ? secondAngle : firstAngle, flipped ? firstAngle : secondAngle);

            //  Lines
            CreateExtensionLine(firstPoint + extLineOffset * firstExtDir, firstArcPoint + firstExtDir * extLineExt);
            CreateExtensionLine(secondPoint + extLineOffset * secondExtDir, secondArcPoint + secondExtDir * extLineExt);

            // Arrows
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


        private XY evaluateArcCenter(XY p1a, XY p1e, XY p2a, XY p2e) {
            double m1 = (p1e.Y - p1a.Y) / (p1e.X - p1a.X);
            double m2 = (p2e.Y - p2a.Y) / (p2e.X - p2a.X);
            double x = (m1 * p1a.X - m2 * p2a.X - p1a.Y + p2a.Y) / (m1 - m2);
            double y = m1 * (x - p1a.X) + p1a.Y;

            return new XY(x, y);
        }
    }
}
