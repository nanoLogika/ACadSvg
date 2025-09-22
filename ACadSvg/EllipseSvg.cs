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

			XY center = _ellipse.Center.ToXY();
			double rr = _ellipse.RadiusRatio;
			//  ep defines the length and rotation of the major axis
			XY majorAxisEndPoint = _ellipse.MajorAxisEndPoint.ToXY();

			var cx = center.X;
			var cy = center.Y;
			double rx = majorAxisEndPoint.GetLength();
			double ry = rx * rr;
			double rot = majorAxisEndPoint.GetAngle();	//	in radians
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
                //	Elliptic arc
				PathElement path = new PathElement()
				    .WithID(ID)
				    .WithClass(Class)
                    .WithFill("none")
					.WithStroke(ColorUtils.GetHtmlColor(_ellipse, _ellipse.Color))
					.WithStrokeWidth(LineUtils.GetLineWeight(_ellipse.LineWeight, _ellipse, _ctx));

				Utils.EllipseArcToPath(
					path, true,
					_ellipse.Center.ToXY(), _ellipse.MajorAxisEndPoint.ToXY(),
					_ellipse.RadiusRatio, _ellipse.StartParameter, _ellipse.EndParameter);

				return path;
			}
		}
	}
}