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
	/// Represents an SVG element converted from an ACad <see cref="Solid"/> entity.
	/// The <see cref="Solid"/> entity is converted into a closed <i>path</i> element.
	/// </summary>
	internal class SolidSvg : EntitySvg {

        private Solid _solid;


		/// <summary>
		/// Initializes a new instance of the <see cref="SolidSvg"/> class
		/// for the specified <see cref="Solid"/> entity.
		/// 
		/// </summary>
		/// <param name="solid">The <see cref="Solid"/> entity to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
        public SolidSvg(Entity solid, ConversionContext ctx) {
            _solid = (Solid)solid;
			SetStandardIdAndClassIf(solid, ctx);
		}

		//	TODO Color?
		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			return new PathElement()
				.AddMove(_solid.FirstCorner.X, _solid.FirstCorner.Y)
				.AddLine(_solid.SecondCorner.X, _solid.SecondCorner.Y)
				.AddLine(_solid.ThirdCorner.X, _solid.ThirdCorner.Y)
				.AddLine(_solid.FirstCorner.X, _solid.FirstCorner.Y)
				.WithID(ID)
				.WithClass(Class)
				.WithFill("white");
		}
    }
}
