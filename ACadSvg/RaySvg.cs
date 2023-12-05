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
    /// Represents an SVG element converted from an ACad <see cref="Ray"/> entity.
    /// The <see cref="Ray"/> entity is converted into a <i>path</i> element with
    /// a line from the specified start point to "infinity" in the specified
    /// direction.
    /// </summary>
    internal class RaySvg : EntitySvg {

        private Ray _ray;


        /// <summary>
        /// Initializes a new instance of the <see cref="RaySvg"/> class
        /// for the specified <see cref="Ray"/> entity.
        /// </summary>
        /// <param name="ray">The <see cref="Ray"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public RaySvg(Entity ray, ConversionContext ctx) : base(ctx) {
            _ray = (Ray)ray;
            SetStandardIdAndClassIf(ray, ctx);
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {

            PathElement pathElement = (PathElement)new PathElement()
                .WithID(ID)
                .WithClass(Class)
                .WithStroke(ColorUtils.GetHtmlColor(_ray, _ray.Color))
                .WithStrokeDashArray(LineUtils.LineToDashArray(_ray, _ray.LineType))
                .WithStrokeWidth(LineUtils.GetLineWeight(_ray.LineWeight, _ray, _ctx));

            double range = Utils.GetInfinity(_ray);

            XY direction = Utils.ToXY(_ray.Direction);
            XY startPoint = Utils.ToXY(_ray.StartPoint);
            XY infintiyPoint = startPoint + direction * range;

            pathElement.AddLine(startPoint.X, startPoint.Y, infintiyPoint.X, infintiyPoint.Y);

            return pathElement;
        }
    }
}
