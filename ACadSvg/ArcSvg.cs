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
	/// Represents an SVG <i>arc</i> converted from an ACad <see cref="Arc"/> entity.
	/// </summary>
	internal class ArcSvg : EntitySvg {

        private Arc _arc;


        public ArcSvg(Entity arc, ConversionContext ctx) {
            _arc = (Arc)arc;
			SetStandardIdAndClassIf(arc, ctx);
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			var c = _arc.Center;
			var sa = _arc.StartAngle;
			var ea = _arc.EndAngle;
			var r = _arc.Radius;

			return new PathElement()
				.AddMoveAndArc(c.X, c.Y, sa, ea, r)
				.WithID(ID)
				.WithClass(Class)
				.WithStroke(ColorUtils.GetHtmlColor(_arc, _arc.Color));
		}
    }
}