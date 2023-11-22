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
	/// Represents an SVG element converted from an ACad <see cref="Spline"/> entity.
	/// The <see cref="Spline"/> entity is converted into a <i>path</i> element.
	/// The <i>path</i> element aproximates the spline curve by a polygon. This should
	/// be improved soon.
	/// </summary>
	internal class SplineSvg : EntitySvg {

        private Spline _spline;


		/// <summary>
		/// Initializes a new instance of the <see cref="SplineSvg"/> class
		/// for the specified <see cref="Spline"/> entity.
		/// </summary>
		/// <param name="spline">The <see cref="Spline"/> entity to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
		public SplineSvg(Entity spline, ConversionContext ctx) {
            _spline = (Spline)spline;
			SetStandardIdAndClassIf(spline, ctx);
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			if (_spline.ControlPoints.Count > 0) {
				switch (_spline.ControlPoints.Count) {
				case 3:
					return new PathElement()
						.AddMoveAndQSpline(Utils.VerticesToArray(_spline.ControlPoints))
                        .WithID(ID)
                        .WithClass(Class)
                        .WithStroke(ColorUtils.GetHtmlColor(_spline, _spline.Color))
                        .WithStrokeDashArray(Utils.LineToDashArray(_spline, _spline.LineType));
				
				case 4:
                    return new PathElement()
                        .AddMoveAndCSpline(Utils.VerticesToArray(_spline.ControlPoints))
                        .WithID(ID)
                        .WithClass(Class)
                        .WithStroke(ColorUtils.GetHtmlColor(_spline, _spline.Color))
                        .WithStrokeDashArray(Utils.LineToDashArray(_spline, _spline.LineType));
                
				default:
                    return new PathElement()
                        .AddPoints(Utils.VerticesToArray(_spline.ControlPoints))
                        .WithID(ID)
                        .WithClass(Class)
                        .WithStroke(ColorUtils.GetHtmlColor(_spline, _spline.Color))
                        .WithStrokeDashArray(Utils.LineToDashArray(_spline, _spline.LineType));
                }
            }
			else if (_spline.FitPoints != null) {
				XY[] fitPoints = Utils.XYZToXYArray(_spline.FitPoints.ToArray());
				XY[] curve = SplineUtils.InterpolateXY(fitPoints, fitPoints.Length * 10);
				return new PathElement()
					.AddPoints(Utils.VerticesToArray(curve))
					.WithID(ID)
					.WithClass(Class)
					.WithStroke(ColorUtils.GetHtmlColor(_spline, _spline.Color))
					.WithStrokeDashArray(Utils.LineToDashArray(_spline, _spline.LineType));
			}

			return null;
		}
    }
}
