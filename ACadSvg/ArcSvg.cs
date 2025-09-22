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
	/// Represents an SVG element converted from an ACad <see cref="Arc"/> entity.
	/// The <see cref="Arc"/> entity is converted into a <i>path</i> element.
	/// </summary>
	internal class ArcSvg : EntitySvg {

        private Arc _arc;


		/// <summary>
		/// Initializes a new instance of the <see cref="ArcSvg"/> class
		/// for the specified <see cref="Arc"/> entity.
		/// </summary>
		/// <param name="arc">The <see cref="Arc"/> entity to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
		public ArcSvg(Entity arc, ConversionContext ctx) : base(ctx) {
            _arc = (Arc)arc;
			SetStandardIdAndClassIf(arc, ctx);
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {

            PathElement path = new PathElement()
				.WithID(ID)
				.WithFill("none")
				.WithClass(Class)
				.WithStroke(ColorUtils.GetHtmlColor(_arc, _arc.Color))
				.WithStrokeWidth(LineUtils.GetLineWeight(_arc.LineWeight, _arc, _ctx));

			Utils.ArcToPath(path, true, _arc.Center.ToXY(), _arc.Radius, _arc.StartAngle, _arc.EndAngle);

			return path;
		}
    }
}