#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;


namespace ACadSvg {

    // TODO Should the point radius be configurable or evaluated from the drawing size?

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="Point"/> entity.
    /// The <see cref="Point"/> entity is converted into a <i>circle</i> element.
    /// </summary>
    internal class PointSvg : EntitySvg {

		private enum PointStyle {
			Dot = 0,
			Empty = 1,
			Plus = 2,
			X = 3,
			LineUp = 4,
		}


        [Flags]
        private enum PointDecoration {
            Circle = 32,
            Square = 64
        }


        private Point _point;
        private PointStyle _pointStyle;
        private PointDecoration _pointDecoration;

        private double _pointDisplaySize;


		/// <summary>
		/// Initializes a new instance of the <see cref="PointSvg"/> class
		/// for the specified <see cref="Point"/> entity.
		/// </summary>
		/// <param name="point">The <see cref="Circle"/> entity to be converted.</param>
		/// <param name="ctx">This parameter is not used in this class.</param>
		public PointSvg(Entity point, ConversionContext ctx) : base(ctx) {
            _point = (Point)point;
            SetStandardIdAndClassIf(point, ctx);

            short pdMode = point.Document.Header.PointDisplayMode;
            _pointStyle = (PointStyle)(pdMode & 0x7);
            _pointDecoration = (PointDecoration)(pdMode & 0xFFF0);

			_pointDisplaySize = point.Document.Header.PointDisplaySize;
            if (_pointDisplaySize == 0) {
                _pointDisplaySize = 5;
			}
		}


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            if (_pointStyle == PointStyle.Empty) {
                return null;
            }

            List<SvgElementBase> svgElements = new List<SvgElementBase>();

            switch (_pointStyle) {
            case PointStyle.Dot:
                addCircle(svgElements, _pointDisplaySize / 10, true);
                break;
            case PointStyle.Plus:
                addPlus(svgElements, _pointDisplaySize);
                break;
            case PointStyle.X:
                addX(svgElements, _pointDisplaySize);
                break;
            case PointStyle.LineUp:
                addLineUp(svgElements, _pointDisplaySize);
                break;
            }

            if (_pointDecoration.HasFlag(PointDecoration.Circle)) {
                addCircle(svgElements, _pointDisplaySize, false);
            }

            if (_pointDecoration.HasFlag(PointDecoration.Square)) {
                addSquare(svgElements, _pointDisplaySize);
            }

            if (svgElements.Count == 1) {
                SvgElementBase svgElement = svgElements[0];
                setDefaultProperties(svgElement);
				return svgElement;
			}

			if (svgElements.Count > 1) {
				GroupElement groupElement = new GroupElement();
				setDefaultProperties(groupElement);
				groupElement.Children.AddRange(svgElements);
				return groupElement;
			}

			return null;
		}


        private void setDefaultProperties(SvgElementBase svgElement) {
            svgElement.ID = ID;
            svgElement.Class = Class;
            svgElement.Stroke = ColorUtils.GetHtmlColor(_point, _point.Color);
            svgElement.StrokeWidth = LineUtils.GetLineWeight(_point.LineWeight, _point, _ctx);
		}


        private void addCircle(List<SvgElementBase> svgElements, double size, bool fill) {
            CircleElement circleElement = new CircleElement() {
				Cx = _point.Location.X,
				Cy = _point.Location.Y,
				R = size / 2
			};

            if (fill) {
                circleElement.WithFill(ColorUtils.GetHtmlColor(_point, _point.Color));
            }

            svgElements.Add(circleElement);
		}


        private void addPlus(List<SvgElementBase> svgElements, double size) {
            double x = _point.Location.X;
            double y = _point.Location.Y;
            double halfSize = size / 2;
            double margin = size / 10;

            PathElement horizontalLine = new PathElement();
			horizontalLine.AddMove(x - halfSize - margin, y);
            horizontalLine.AddLine(x + halfSize + margin, y);
			svgElements.Add(horizontalLine);

            PathElement verticalLine = new PathElement();
            verticalLine.AddMove(x, y - halfSize - margin);
            verticalLine.AddLine(x, y + halfSize + margin);
            svgElements.Add(verticalLine);
        }


        private void addX(List<SvgElementBase> svgElements, double size) {
			double x = _point.Location.X;
			double y = _point.Location.Y;
            double halfSize = size / 2;
			double margin = size / 10;

			PathElement diagonalLine1 = new PathElement();
            diagonalLine1.AddMove(x - halfSize - margin, y - halfSize - margin);
            diagonalLine1.AddLine(x + halfSize + margin, y + halfSize + margin);
            svgElements.Add(diagonalLine1);

			PathElement diagonalLine2 = new PathElement();
			diagonalLine2.AddMove(x + halfSize + margin, y - halfSize - margin);
			diagonalLine2.AddLine(x - halfSize - margin, y + halfSize + margin);
			svgElements.Add(diagonalLine2);
		}


        private void addLineUp(List<SvgElementBase> svgElements, double size) {
			double x = _point.Location.X;
			double y = _point.Location.Y;

            PathElement lineUp = new PathElement();
            lineUp.AddMove(x, y + (size / 2));
            lineUp.AddLine(x, y);
            svgElements.Add(lineUp);
		}


        private void addSquare(List<SvgElementBase> svgElements, double size) {
            double x = _point.Location.X;
            double y = _point.Location.Y;
            double halfSize = size / 2;

            RectangleElement squareElement = new RectangleElement() {
                X = x - halfSize,
                Y = y - halfSize,
                Width = size,
                Height = size
            };
            svgElements.Add(squareElement);
        }
    }
}