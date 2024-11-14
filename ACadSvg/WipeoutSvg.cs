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
    /// Represents an SVG element converted from an ACad <see cref="Wipeout"/> entity.
    /// </summary>
    internal class WipeoutSvg : EntitySvg {

        private Wipeout _wipeout;


		/// <summary>
		/// Initializes a new instance of the <see cref="WipeoutSvg"/> class
		/// for the specified <see cref="Wipeout"/> entity.
		/// </summary>
		/// <param name="wipeout">The <see cref="Wipeout"/> entity to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
		public WipeoutSvg(Entity wipeout, ConversionContext ctx) : base(ctx) {
            _wipeout = (Wipeout)wipeout;
            SetStandardIdAndClassIf(wipeout, ctx);
        }


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			var path = new PathElement();

			List<XY> vertices = _wipeout.ClipBoundaryVertices;

			List<XY> newVertices = new List<XY>();
			for (int i = 0; i < vertices.Count; i++) {
				BoundingBox bb = _wipeout.GetBoundingBox();

				double x = _wipeout.InsertPoint.X + ((bb.Width * _wipeout.UVector.X) / 2) + (vertices[i].X * _wipeout.UVector.X);
				double y = _wipeout.InsertPoint.Y + ((bb.Height * _wipeout.VVector.Y) / 2) + (vertices[i].Y * _wipeout.VVector.Y * (_ctx.ConversionOptions.ReverseY ? -1 : 1));

				newVertices.Add(new XY(x, y));
			}

			path.AddPoints(Utils.VerticesToArray(newVertices));
			path.Close();

			path.WithFill("green");

			return path;
		}
    }
}