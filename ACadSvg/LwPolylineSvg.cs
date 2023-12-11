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
	/// Represents an SVG element converted from an ACad <see cref="LwPolyline"/> entity.
	/// The <see cref="LwPolyline"/> entity is converted into a <i>path</i> element.
	/// </summary>
	internal class LwPolylineSvg : EntitySvg {

        private LwPolyline _polyline;


		/// <summary>
		/// Initializes a new instance of the <see cref="LwPolylineSvg"/> class
		/// for the specified <see cref="LwPolyline"/> entity.
		/// </summary>
		/// <param name="polyline">The <see cref="LwPolyline"/> entity to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
		public LwPolylineSvg(Entity polyline, ConversionContext ctx) : base(ctx) {
            _polyline = (LwPolyline)polyline;
			SetStandardIdAndClassIf(polyline, ctx);
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			PathElement pathElement = new PathElement();

			//	-  Allways MOVE to the first vertex
			//	-  Keep the bulge for the line to the NEXT line.
			//	-  A line without bulge is a straight line
			//	-  A line with bulge is a cuircular arc

			bool first = true;
			XY lastVertexLocation = XY.Zero;
			double lastVertexBulge = 0;
			foreach (var vertex in _polyline.Vertices) {
				XY vertexLocation = vertex.Location;
				if (first) {
					pathElement.AddMove(vertexLocation.X, vertexLocation.Y);
					first = false;
				}
				else {
                    addLineOrBulge(pathElement, vertexLocation, lastVertexLocation, lastVertexBulge);
                }
                lastVertexBulge = vertex.Bulge;
                lastVertexLocation = vertex.Location;
            }

			if (_polyline.IsClosed) {
				addLineOrBulge(pathElement, _polyline.Vertices[0].Location, lastVertexLocation, lastVertexBulge);
            }

			pathElement
				.WithID(ID)
				.WithClass(Class)
				.WithStroke(ColorUtils.GetHtmlColor(_polyline, _polyline.Color))
				.WithStrokeDashArray(LineUtils.LineToDashArray(_polyline, _polyline.LineType))
				.WithStrokeWidth(LineUtils.GetLineWeight(_polyline.LineWeight, _polyline, _ctx));

			return pathElement;
		}


        private static void addLineOrBulge(PathElement pathElement, XY vertexLocation, XY lastVertexLocation, double lastVertexBulge) {
            if (lastVertexBulge != 0) {
                double l = vertexLocation.DistanceFrom(lastVertexLocation);
                double d = l / 2;
                double sagitta = d * Math.Abs(lastVertexBulge);
                double r = (Math.Pow(sagitta, 2) + Math.Pow(d, 2)) / 2 / sagitta;
                bool lf = r < sagitta;
                bool sf = lastVertexBulge > 0;

                pathElement.AddArc(r, r, 0, lf, sf, vertexLocation.X, vertexLocation.Y);
            }
            else {
                pathElement.AddLine(vertexLocation.X, vertexLocation.Y);
            }
        }
    }
}