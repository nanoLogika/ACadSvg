#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;


namespace ACadSvg {

	/// <summary>
	/// Represents an SVG <i>circle</i> converted from an ACad <see cref="Circle"/> entity.
	/// </summary>
	internal class CircleSvg : EntitySvg {

        private Circle _circle;


		/// <summary>
		/// Initializes a new instance of the <see cref="CircleSvg"/> class
		/// for the specified <see cref="Circle"/> entity.
		/// /// </summary>
		/// <param name="circle">The <see cref="Circle"/> entity to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
		public CircleSvg(Entity circle, ConversionContext ctx) {
            _circle = (Circle)circle;
			SetStandardIdAndClassIf(circle, ctx);
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			return new CircleElement() {
				Cx = _circle.Center.X,
				Cy = _circle.Center.Y,
				R = _circle.Radius
			}
			.WithID(ID)
			.WithClass(Class)
			.WithStroke(ColorUtils.GetHtmlColor(_circle, _circle.Color));
		}
    }
}