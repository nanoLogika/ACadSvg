#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;


namespace ACadSvg {

    internal class LwPolylineSvg : EntitySvg {

        private LwPolyline _polyline;


		/// <summary>
		/// Initializes a new instance of the <see cref="LwPolylineSvg"/> class
		/// for the specified <see cref="LwPolyline"/> entity.
		/// </summary>
		/// <param name="polyline">The <see cref="LwPolyline"/> entity to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
		public LwPolylineSvg(Entity polyline, ConversionContext ctx) {
            _polyline = (LwPolyline)polyline;
			SetStandardIdAndClassIf(polyline, ctx);
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			return new PathElement()
				.AddPoints(Utils.VerticesToArray(_polyline.Vertices))
				.Close()
				.WithID(ID)
				.WithClass(Class)
				.WithStroke(ColorUtils.GetHtmlColor(_polyline, _polyline.Color))
				.WithStrokeDashArray(Utils.LineToDashArray(_polyline, _polyline.LineType));
		}
    }
}