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

			List<CSMath.XY> vertices = _wipeout.ClipBoundaryVertices;
			
			List<CSMath.XY> newVertices = new List<CSMath.XY>();
			foreach (var vertex in vertices) {
				newVertices.Add(new CSMath.XY(_wipeout.InsertPoint.X + vertex.X, _wipeout.InsertPoint.Y + vertex.Y));
			}

			path.AddPoints(Utils.VerticesToArray(newVertices));
			path.Close();

			CircleElement circle = new CircleElement()
				.WithID("InsertPoint")
				.WithFill("yellow");
			circle.Cx = _wipeout.InsertPoint.X;
			circle.Cy = _wipeout.InsertPoint.Y;
			circle.R = 1;

			GroupElement group = new GroupElement();
			group.Children.Add(path);
			group.Children.Add(circle);

			path
				.WithID(ID)
				.WithClass(Class)
				.WithFill("green");

			return group;
		}
    }
}