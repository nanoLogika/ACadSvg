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
	/// Represents an SVG element converted from an ACad <see cref="Ellipse"/> entity.
	/// The <see cref="Ellipse"/> entity is converted into a <i>ellipse</i> element.
	/// </summary>
	internal class EllipseSvg : EntitySvg {

		private Ellipse _ellipse;


		/// <summary>
		/// Initializes a new instance of the <see cref="EllipseSvg"/> class
		/// for the specified <see cref="Ellipse"/> entity.
		/// </summary>
		/// <param name="ellipse">The <see cref="Ellipse"/> entity to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
		public EllipseSvg(Entity ellipse, ConversionContext ctx) {
			_ellipse = (Ellipse)ellipse;
			SetStandardIdAndClassIf(ellipse, ctx);
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {

			var ec = _ellipse.Center;
			var rr = _ellipse.RadiusRatio;
			var ep = _ellipse.EndPoint;

			//    ep defines the length of the large axis
			//    ep.x = 0 means lage axis parallel to y
			//    ep.y = 0 means lage axis parallel to x
			//  both != 0 we need a transform in SVG, since svg ellipse only supports cx, ca, rx, rx

			var cx = ec.X;
			var cy = ec.Y;
			double rx;
			double ry;
			if (ep.X < 0.0001) {
				ry = Math.Abs(cy - ec.Y);
				rx = ry * rr;
			}
			else {
				rx = Math.Abs(cx - ec.X);
				ry = rx * rr;
			}


			return new EllipseElement() { Cx = cx, Cy = cy, Rx = rx, Ry = ry }
				.WithID(ID)
				.WithClass(Class)
				.WithStroke(ColorUtils.GetHtmlColor(_ellipse, _ellipse.Color));
		}
	}
}