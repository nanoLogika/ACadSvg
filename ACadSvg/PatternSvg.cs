#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using SvgElements;
using System.Xml.Linq;


namespace ACadSvg {

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
