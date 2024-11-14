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
				double x = _wipeout.InsertPoint.X + (vertices[i].X * _wipeout.UVector.X);
				double y = _wipeout.InsertPoint.Y + (vertices[i].Y * _wipeout.VVector.Y * (_ctx.ConversionOptions.ReverseY ? -1 : 1));

				newVertices.Add(new XY(x, y));
			}

			path.AddPoints(Utils.VerticesToArray(newVertices));
			path.Close();

			GroupElement group = new GroupElement();

			CircleElement circle1 = new CircleElement()
				.WithID($"1st POINT")
				.WithFill("red");
			circle1.Cx = newVertices[0].X;
			circle1.Cy = newVertices[0].Y;
			circle1.R = 1;
			group.Children.Add(circle1);

			CircleElement circle2 = new CircleElement()
				.WithID($"2nd POINT")
				.WithFill("green");
			circle2.Cx = newVertices[1].X;
			circle2.Cy = newVertices[1].Y;
			circle2.R = 1;
			group.Children.Add(circle2);

			CircleElement circle3 = new CircleElement()
				.WithID($"3rd POINT")
				.WithFill("blue");
			circle3.Cx = newVertices[2].X;
			circle3.Cy = newVertices[2].Y;
			circle3.R = 1;
			group.Children.Add(circle3);

			group.Children.Add(path);

			path
				.WithID($"Size: {_wipeout.Size} | UVector: {_wipeout.UVector} | VVector: {_wipeout.VVector}")
				.WithClass(Class)
				.WithFill("skyblue");

			CircleElement circle4 = new CircleElement()
				.WithID($"INSERT POINT {_wipeout.InsertPoint}")
				.WithStroke("magenta");
			circle4.Cx = _wipeout.InsertPoint.X;
			circle4.Cy = _wipeout.InsertPoint.Y;
			circle4.R = 1;
			group.Children.Add(circle4);

			return group;
		}
    }
}