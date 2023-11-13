#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Tables;
using ACadSharp.Entities;

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

        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private double _rot;
        private string _patternColor;
        private List<XElement> _elements = new List<XElement>();


        public PatternSvg(HatchPattern pattern, string patternColor) {

            switch (pattern.Name) {
            case "ANSI31":
                initAnsi31();
                break;
            case "AR-CONC":
                initConcrete();
                break;

            default:
                //  TODO Try to understand the pattern definition from AutoCAD
                foreach (var line in pattern.Lines) {

                }
                break;
            }

            _patternColor = patternColor;
        }


        /// <summary>
        /// Gets a value indicating whether the pattern has been created successfully.
        /// </summary>
        /// <value>
        /// <b>true</b>, when the pattern has been created successfully; otherwise, <b>false</b>.
        /// </value>
        public bool Valid { get; set; } = false;


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			PatternElement patternElement = new PatternElement();
			patternElement.Comment = Comment;
			patternElement.ID = ID;
			patternElement.Elements = _elements;
            patternElement.X = _x;
            patternElement.Y = _y;
			patternElement.Width = _width;
			patternElement.Height = _height;
			patternElement.AddRotate(_rot);
			patternElement.Stroke = _patternColor;
			return patternElement;
		}


        private void initAnsi31() {
            ID = "ANSI31";
            _width = 16;
            _height = 16;
            _elements.Add(XElement.Parse($"<path d=\"M -4,4 l 8,-8 M 0,16 l 16,-16 M 12,20 l 8,-8\"></path>"));
            Valid = true;
        }


        private void initConcrete() {
            ID = "AR-CONC";
            _x = -5;
            _y = -5;
            _width = 190;
            _height = 90;

            var patternElementsGroup = XElement.Parse("<g>" + Resources.concrete_pattern + "</g>");
            _elements.AddRange(patternElementsGroup.Elements());
            Valid = true;
        }
	}
}
