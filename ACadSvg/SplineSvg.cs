#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;


namespace ACadSvg {

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
			if (_spline.ControlPoints.Count == 0) {
				return null;
			}

			return new PathElement()
				.AddPoints(Utils.VerticesToArray(_spline.ControlPoints))
				.AddPoints(false, Utils.VerticesToArray(_spline.FitPoints))
				.WithID(ID)
				.WithClass(Class)
				.WithStroke(ColorUtils.GetHtmlColor(_spline, _spline.Color))
				.WithStrokeDashArray(Utils.LineToDashArray(_spline, _spline.LineType));
		}
    }
}