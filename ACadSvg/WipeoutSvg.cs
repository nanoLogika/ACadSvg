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
            _insertAtTopOfTheParentGroup = true;
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {

            List<XY> vertices = _wipeout.ClipBoundaryVertices;
            double reverseY = _ctx.ConversionOptions.ReverseY ? -1 : 1;

            XY offset = new XY(
				_wipeout.InsertPoint.X + _wipeout.UVector.X / 2,
				_wipeout.InsertPoint.Y + _wipeout.VVector.Y / 2);

            List <XY> newVertices = new List<XY>();
			for (int i = 0; i < vertices.Count; i++) {
				XY vertex = new XY(
					offset.X + vertices[i].X * _wipeout.UVector.X,
					offset.Y + vertices[i].Y * _wipeout.VVector.Y * reverseY);

                newVertices.Add(vertex);
			}

			PathElement path = new PathElement()
				.AddPoints(Utils.VerticesToArray(newVertices))
				.Close()
				.WithID(ID)
				.WithClass(Class)
				.WithStroke("none");

            return path;
		}
    }
}