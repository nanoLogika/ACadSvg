#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using SvgElements;
using System.Xml.Linq;


namespace ACadSvg {

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="BlockRecord"/> table entry
    /// that is used as a pattern to fill a shape of a <see cref="Hatch"/> entity.
    /// The <see cref="BlockRecord"/> associated with the <see cref="Hatch"/> entity is
    /// converted into a <i>pattern</i> element.
    /// </summary>
    public class PatternSvg : GroupSvg {

		public string Width { get; set; } = "0";


		public string Height { get; set; } = "0";


		public double Rotation { get; set; } = 0;


		public List<XElement> Elements = new List<XElement>();


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			PatternElement patternElement = new PatternElement();
			patternElement.Comment = Comment;
			patternElement.ID = ID;
			patternElement.Elements = Elements;
			patternElement.Width = Width;
			patternElement.Height = Height;
			patternElement.AddRotate(Rotation);
			return patternElement;
		}
	}
}
