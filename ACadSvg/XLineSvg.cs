#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;

using CSMath;

using SvgElements;


namespace ACadSvg {

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="XLine"/> entity.
    /// The <see cref="XLine"/> entity is converted into a <i>path</i> element with
    /// a line from the negative "infinity" to positive "infinity" in the specified
    /// direction.
    /// </summary>
    internal class XLineSvg : EntitySvg {

        private XLine _xLine;


        /// <summary>
		/// Initializes a new instance of the <see cref="XLineSvg"/> class
		/// for the specified <see cref="XLine"/> entity.
        /// </summary>
        /// <param name="xLine">The <see cref="XLine"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public XLineSvg(Entity xLine, ConversionContext ctx) : base(ctx) {
            _xLine = (XLine)xLine;
            SetStandardIdAndClassIf(xLine, ctx);
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            PathElement pathElement = (PathElement)new PathElement()
                .WithID(ID)
                .WithClass(Class)
                .WithStroke(ColorUtils.GetHtmlColor(_xLine, _xLine.Color))
                .WithStrokeDashArray(LineUtils.LineToDashArray(_xLine, _xLine.LineType))
                .WithStrokeWidth(LineUtils.GetLineWeight(_xLine.LineWeight, _xLine, _ctx));

            double range = Utils.GetInfinity(_xLine);

            XY direction = _xLine.Direction.ToXY();
            XY firstPoint = _xLine.FirstPoint.ToXY();
            XY negInfintiyPoint = firstPoint - direction * range;
            XY posInfintiyPoint = firstPoint + direction * range;

            pathElement.AddLine(negInfintiyPoint.X, negInfintiyPoint.Y, posInfintiyPoint.X, posInfintiyPoint.Y);

            return pathElement;
        }
    }
}
