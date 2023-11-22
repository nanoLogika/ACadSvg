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
			var ep2 = new XY(ep.X, ep.Y);

			//  ep defines the length and rolation of the large axis

			var cx = ec.X;
			var cy = ec.Y;
			double rx = ep2.GetLength();
			double ry = rx * rr;
			double rot = ep2.GetAngle();
			double sa = _ellipse.StartParameter;
			double ea = _ellipse.EndParameter;

			if (sa == ea || sa == ea - Math.PI * 2) {
				EllipseElement ellipse = new EllipseElement() {
					Cx = cx,
					Cy = cy,
					Rx = rx,
					Ry = ry
				};
				if (rot != 0 && ry != rx) {
					ellipse.AddRotate(rot, cx, cy);
				}
				return ellipse
					.WithID(ID)
                    .WithClass(Class)
                    .WithStroke(ColorUtils.GetHtmlColor(_ellipse, _ellipse.Color));
            }
			else {
				return new PathElement()
					.AddMoveAndArc(cx, cy, sa, ea, rx, ry, rot)
					.WithID(ID)
					.WithClass(Class)
					.WithStroke(ColorUtils.GetHtmlColor(_ellipse, _ellipse.Color));
			}
		}
	}
}