#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using ACadSharp.Tables;
using SvgElements;


namespace ACadSvg {
    internal class MTextSvg : EntitySvg {

        private MText _mText;


		/// <summary>
		/// Initializes a new instance of the <see cref="MTextSvg"/> class
		/// for the specified <see cref="MText"/> entity.
		/// </summary>
		/// <param name="mText">The <see cref="MText"/> entity to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
		public MTextSvg(Entity mText, ConversionContext ctx) {
            _mText = (MText)mText;
			SetStandardIdAndClassIf(mText, ctx);
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			var iX = _mText.InsertPoint.X;
			var iY = _mText.InsertPoint.Y;
			double textSize = TextUtils.GetTextSize(false, _mText.Height, _mText.Style, _mText.BackgroundScale);
			double rot = _mText.Rotation * 180 / Math.PI;
			TextStyle textStyle = _mText.Style;
			string textAnchor = TextUtils.HorizontalAlignmentToTextAnchor(_mText.AttachmentPoint);
			iY -= TextUtils.AlignmentToVerticalAdjustment(_mText.AttachmentPoint, textSize);

			TextUtils.StyleToValues(textStyle, textSize, out string fontFamily, out double fontSize, out bool bold, out bool italic);

			return new TextElement()
				.WithXY(iX, iY)
				.WithTextAnchor(textAnchor)
				.WithFont(fontFamily, fontSize, bold, italic)
				.WithTspans(TextUtils.ConvertMTextToHtml(iX, iY, _mText.Value, textSize, textStyle))
				.WithFill(ColorUtils.GetHtmlTextColor(_mText, _mText.Color))
				.AddRotate(rot, iX, iY)
				.ReverseY()
				.WithID(ID)
				.WithClass(Class);
		}
    }
}