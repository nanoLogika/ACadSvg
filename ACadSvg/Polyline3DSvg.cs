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
	/// Represents an SVG element converted from an ACad <see cref="Polyline3D"/> entity.
	/// The <see cref="Polyline3D"/> entity is converted into a <i>path</i> element.
	/// </summary>
	internal class Polyline3DSvg : EntitySvg {

        private Polyline3D _polyline;


		/// <summary>
		/// Initializes a new instance of the <see cref="Polyline3DSvg"/> class
		/// for the specified <see cref="Polyline3D"/> entity.
		/// </summary>
		/// <param name="polyline">The <see cref="Polyline3D"/> entity to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
		public Polyline3DSvg(Entity polyline, ConversionContext ctx) : base(ctx) {
            _polyline = (Polyline3D)polyline;
			SetStandardIdAndClassIf(polyline, ctx);
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			var path = new PathElement();
			var vertices = _polyline.Vertices.ToList();
			path.AddPoints(Utils.VerticesToArray(_polyline.Vertices.ToList()));

			if (_polyline.IsClosed) {
				path.Close();
			}

			return path
				.WithID(ID)
				.WithClass(Class)
                .WithFill("none")
                .WithStroke(ColorUtils.GetHtmlColor(_polyline, _polyline.Color))
				.WithStrokeDashArray(LineUtils.LineToDashArray(_polyline, _polyline.LineType))
				.WithStrokeWidth(LineUtils.GetLineWeight(_polyline.LineWeight, _polyline, _ctx));
		}
    }
}