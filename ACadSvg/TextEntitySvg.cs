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
	/// Represents an SVG element converted from an ACad <see cref="TextEntity"/>.
	/// The <see cref="TextEntity"/> is converted into a <i>text</i> element, somtime
	/// containing one <i>tspan</i> element.
	/// </summary>
    internal class TextEntitySvg : EntitySvg {

        private TextEntity _text;
		private ConversionContext _ctx;


		/// <summary>
		/// Initializes a new instance of the <see cref="TextEntitySvg"/> class
		/// for the specified <see cref="TextEntity"/>.
		/// </summary>
		/// <param name="text">The <see cref="TextEntity"/> to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
		public TextEntitySvg(Entity text, ConversionContext ctx) {
            _text = (TextEntity)text;
			_ctx = ctx;

			SetStandardIdAndClassIf(text, ctx);
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			double textSize = TextUtils.GetTextSize(false, _text.Height, _text.Style, 1);
			TextUtils.StyleToValues(_text.Style, textSize, out string fontFamily, out double fontSize, out bool bold, out bool italic);

			return new TextElement()
				.WithXY(_text.InsertPoint.X, _text.InsertPoint.Y)
				.WithTextAnchor(TextUtils.HorizontalAlignmentToTextAnchor(_text.HorizontalAlignment))
				.WithFont(fontFamily, fontSize, bold, italic)
				.WithValue(_text.Value)
				.WithStroke("none")
				.WithFill(ColorUtils.GetHtmlTextColor(_text, _text.Color))
				.ReverseY(_ctx.ConversionOptions.ReverseY)
				.WithID(ID)
				.WithClass(Class);
		}
    }
}