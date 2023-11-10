#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;


namespace ACadSvg {
    internal class PointSvg : EntitySvg {

        private Point _point;


        /// <summary>
		/// Initializes a new instance of the <see cref="PointSvg"/> class
		/// for the specified <see cref="Point"/> entity.
        /// </summary>
        /// <param name="point">The <see cref="Circle"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public PointSvg(Entity point, ConversionContext ctx) {
            _point = (Point)point;
            SetStandardIdAndClassIf(point, ctx);
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            return new CircleElement() {
                Cx = _point.Location.X,
                Cy = _point.Location.Y,
                R = 5
            }
            .WithID(ID)
            .WithClass(Class)
            .WithStroke(ColorUtils.GetHtmlColor(_point, _point.Color));
		}
    }
}