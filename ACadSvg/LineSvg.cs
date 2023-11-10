#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;


namespace ACadSvg {

    internal class LineSvg : EntitySvg {

        private Line _line;


        /// <summary>
		/// Initializes a new instance of the <see cref="LineSvg"/> class
		/// for the specified <see cref="Line"/> entity.
        /// </summary>
        /// <param name="line">The <see cref="Line"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public LineSvg(Entity line, ConversionContext ctx) {
            _line = (Line)line;
            SetStandardIdAndClassIf(line, ctx);
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            return new PathElement()
                .AddLine(_line.StartPoint.X, _line.StartPoint.Y, _line.EndPoint.X, _line.EndPoint.Y)
                .WithID(ID)
                .WithClass(Class)
                .WithStroke(ColorUtils.GetHtmlColor(_line, _line.Color))
				.WithStrokeDashArray(Utils.LineToDashArray(_line, _line.LineType));
		}
    }
}