#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using ACadSvg.SplineUtils;
using CSMath;

using SvgElements;


namespace ACadSvg
{

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
		public SplineSvg(Entity spline, ConversionContext ctx) : base(ctx) {
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
	                    .WithStrokeWidth(LineUtils.GetLineWeight(_spline.LineWeight, _spline, _ctx))
                        .WithStrokeDashArray(LineUtils.LineToDashArray(_spline, _spline.LineType));

				case 4:
                    return new PathElement()
                        .AddMoveAndCSpline(Utils.VerticesToArray(_spline.ControlPoints))
                        .WithID(ID)
                        .WithClass(Class)
                        .WithStroke(ColorUtils.GetHtmlColor(_spline, _spline.Color))
                        .WithStrokeWidth(LineUtils.GetLineWeight(_spline.LineWeight, _spline, _ctx))
                        .WithStrokeDashArray(LineUtils.LineToDashArray(_spline, _spline.LineType));
						
				default:
					IList<XY> points = NURBS.CreateBSplineCurve(_spline.Degree, _spline.ControlPoints, _spline.Knots);

                    var pathElement = new PathElement()
                        .AddPoints(Utils.VerticesToArray(points))
                        .WithID(ID)
                        .WithClass(Class)
                        .WithStroke(ColorUtils.GetHtmlColor(_spline, _spline.Color))
                        .WithStrokeWidth(LineUtils.GetLineWeight(_spline.LineWeight, _spline, _ctx))
                        .WithStrokeDashArray(LineUtils.LineToDashArray(_spline, _spline.LineType));

                    return pathElement;
     
                    //GroupElement g = new GroupElement();
                    //g.Children.Add(pathElement);
                    //foreach (var point in _spline.ControlPoints) {
                    //    g.Children.Add(new CircleElement() { Cx = point.X, Cy = point.Y, R = 0.5 }.WithStroke("red"));
                    //}
					//return g;
                }
            }
			else if (_spline.FitPoints != null) {
                Utils.XYZToDoubles(_spline.FitPoints, out double[] xs, out double[] ys);
                getTangentDxDy(_spline.StartTangent, out double firstDx, out double firstDy);
                getTangentDxDy(_spline.EndTangent, out double lastDx, out double lastDy);

                CubicSpline.FitParametric(
                    xs, ys, xs.Length * 10,
                    out double[] curveXs, out double[] curveYs,
                    firstDx, firstDy, lastDx, lastDy);

                XY[] curve = Utils.DoublesToXYx(curveXs, curveYs);

                return new PathElement()
                    .AddPoints(Utils.VerticesToArray(curve))
                    .WithID(ID)
                    .WithClass(Class)
                    .WithStroke(ColorUtils.GetHtmlColor(_spline, _spline.Color))
                    .WithStrokeWidth(LineUtils.GetLineWeight(_spline.LineWeight, _spline, _ctx))
                    .WithStrokeDashArray(LineUtils.LineToDashArray(_spline, _spline.LineType));
            }

            return null;
		}


        private void getTangentDxDy(XYZ et, out double dx, out double dy) {
            dx = double.NaN;
            dy = double.NaN;
            if (et != XYZ.Zero) {
                dx = et.X;
                dy = et.Y;
            }
        }
    }
}
