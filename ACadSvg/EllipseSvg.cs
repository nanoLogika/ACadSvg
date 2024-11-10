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
		public EllipseSvg(Entity ellipse, ConversionContext ctx) : base(ctx) {
			_ellipse = (Ellipse)ellipse;
			SetStandardIdAndClassIf(ellipse, ctx);
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {

			var ec = _ellipse.Center;
			var rr = _ellipse.RadiusRatio;
			//  ep defines the length and rotation of the large axis
			var ep = _ellipse.EndPoint;
			var ep2 = new XY(ep.X, ep.Y);

			var cx = ec.X;
			var cy = ec.Y;
			double rx = ep2.GetLength();
			double ry = rx * rr;
			double rot = ep2.GetAngle();
			double sa = _ellipse.StartParameter;
			double ea = _ellipse.EndParameter;

			if (sa == ea || sa == ea - Math.PI * 2) {
				EllipseElement ellipseElement = new EllipseElement() {
					Cx = cx,
					Cy = cy,
					Rx = rx,
					Ry = ry
				};
				if (rot != 0 && ry != rx) {
                    ellipseElement.AddRotate(rot, cx, cy);
				}
				ellipseElement
					.WithID(ID)
                    .WithClass(Class)
		            .WithFill("none")
                    .WithStroke(ColorUtils.GetHtmlColor(_ellipse, _ellipse.Color))
					.WithStrokeWidth(LineUtils.GetLineWeight(_ellipse.LineWeight, _ellipse, _ctx));

				return ellipseElement;
			}
			else {
				return new PathElement()
				    .AddMoveAndArc(cx, cy, sa, ea, rx, ry, rot)
				    .WithID(ID)
				    .WithClass(Class)
                    .WithFill("none")
					.WithStroke(ColorUtils.GetHtmlColor(_ellipse, _ellipse.Color))
				    .WithStrokeWidth(LineUtils.GetLineWeight(_ellipse.LineWeight, _ellipse, _ctx));
			}
		}
	}
}