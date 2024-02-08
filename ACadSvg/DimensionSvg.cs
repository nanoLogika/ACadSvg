#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using ACadSharp.Types.Units;

using ACadSvg.DimensionTextFormatter;
using ACadSvg.Extensions;

using CSMath;

using SvgElements;


namespace ACadSvg {

    /// <summary>
    /// The base class for classes representing SVG elements converted from a AutoCAD dimension
    /// entities.
    /// </summary>
    internal abstract class DimensionSvg : EntitySvg {

        protected DimensionProperties _dimProps;
        protected Dimension _dimension;
        protected string _defaultPostFix;
        protected double? _dimensionLineWidth;
        protected string _dimensionLineColor;
        protected double? _extensionLineWidth;
        protected string _extensionLineColor;
        protected double _arrowSize;
        protected GroupElement _groupElement;


        public DimensionSvg(Entity entity, ConversionContext ctx) : base(ctx) {
            _dimension = (Dimension)entity;
            _dimProps = new DimensionProperties(_dimension, _dimension.Style);
            SetStandardIdAndClassIf(entity, ctx);

            _dimensionLineColor = ColorUtils.GetHtmlColor(_dimension, _dimProps.DimensionLineColor);
            _dimensionLineWidth = LineUtils.GetLineWeight(_dimProps.DimensionLineWeight, _dimension, _ctx);

            _extensionLineWidth = LineUtils.GetLineWeight(_dimProps.ExtensionLineWeight, _dimension, _ctx);
            _extensionLineColor = ColorUtils.GetHtmlColor(_dimension, _dimProps.ExtensionLineColor);

            _arrowSize = _dimProps.ArrowSize;
        }


        /// <summary>
        /// Creates an SVG <i>group</i> element that is to contain the liene and text elements of
        /// a dimension entity.
        /// </summary>
        protected void CreateGroupElement() {
            _groupElement = new GroupElement();
            _groupElement.WithID(ID).WithClass(Class);
        }


        /// <summary>
        /// Evaluates the rotation of the dimension text, that is to be aligned with the
        /// direction of the dimension line. Vertical (90°) text must be readable from the
        /// right side. Text with an angle larger than 90° ...
        /// </summary>
        /// <param name="dimDirRot"></param>
        /// <returns></returns>
        protected static double GetTextRot(double dimDirRot) {
            double textRot = dimDirRot;
            if (textRot > Math.PI / 2 && textRot < Math.PI * 3 / 2) {
                textRot -= Math.PI;
            }
            else if (textRot <= -Math.PI / 2 && textRot > -Math.PI * 3 / 2) {
                textRot += Math.PI;
            }

            return textRot;
        }


        protected void CreateTextElement(XY textPos, double dimDirAngle, out double textLen) {
            double textSize = TextUtils.GetTextSize(_dimProps.TextHeight) * 1.5;
            string text = GetFormattedDimensionText(textSize);
            textLen = TextUtils.GetTextLength(text, textSize);
            string textColor = ColorUtils.GetHtmlColor(_dimension, _dimProps.TextColor);
            double textRot = (GetTextRot(dimDirAngle) + _dimension.TextRotation) * 180 / Math.PI;
            var tva = _dimProps.TextVerticalAlignment;
            var td = _dimProps.TextVerticalPosition;
            double textOffset;
            switch (_dimProps.TextVerticalAlignment) {
            case DimensionTextVerticalAlignment.Centered:
                textOffset = 0;
                break;
            case DimensionTextVerticalAlignment.Above:
                textOffset = _dimProps.DimensionLineGap;
                break;
            case DimensionTextVerticalAlignment.Below:
                textOffset = -_dimProps.DimensionLineGap - textSize * 0.75;
                break;
            default:
                textOffset = 0;
                break;
            }
            string textAnchor = TextUtils.HorizontalAlignmentToTextAnchor(_dimProps.TextHorizontalAlignment);
            TextUtils.StyleToValues(_dimProps.TextStyle, textSize, out string fontFamily, out double fontSize, out bool bold, out bool italic);

            _groupElement.Children.Add(new TextElement()
                .WithXY(textPos.X, textPos.Y)
                .WithTextAnchor(textAnchor)
                .WithFont(fontFamily, fontSize, bold, italic)
                //.WithValue(text)
                .WithTspans(TextUtils.ConvertMTextToHtml(textPos.X, textPos.Y, text, fontSize, _dimProps.TextStyle))
                .WithStroke("none")
                .WithFill(textColor)
                .ReverseY(_ctx.ConversionOptions.ReverseY)
                .AddRotate(textRot, textPos.X, textPos.Y)
                .AddTranslate(0, textOffset));
        }


        protected void CreateTextElementAndLeader(XY dimMid, XY textMid, XY dimDir, double textRot) {
            double textSize = TextUtils.GetTextSize(_dimProps.TextHeight) * 1.5;
            string text = GetFormattedDimensionText(textSize);
            double textLen = TextUtils.GetTextLength(text, textSize);
            string textColor = ColorUtils.GetHtmlColor(_dimension, _dimProps.TextColor);
            string textAnchor;
            if (isCcwPath(dimMid, textMid, textMid - dimDir)) {
                textAnchor = string.Empty;
            }
            else {
                textAnchor = "end";
            }

            XY dimDir90 = Utils.Rotate(dimDir, Math.PI / 2);
            XY textPos = textMid + dimDir90 * textSize / 2;
            TextUtils.StyleToValues(_dimProps.TextStyle, textSize, out string fontFamily, out double fontSize, out bool bold, out bool italic);

            _groupElement.Children.Add(new TextElement()
                .WithXY(textPos.X, textPos.Y)
                .WithTextAnchor(textAnchor)
                .WithFont(fontFamily, fontSize, bold, italic)
                .WithValue(text)
                .WithStroke("none")
                .WithFill(textColor)
                .ReverseY(_ctx.ConversionOptions.ReverseY)
                .AddRotate(textRot, textMid.X, textMid.Y));

            XY landing = textMid + dimDir90 * (textSize / 2 + _dimProps.DimensionLineGap);
            XY landingEnd = landing - dimDir * textLen;
            _groupElement.Children.Add(new PathElement()
                    .AddLine(dimMid.X, dimMid.Y, landing.X, landing.Y)
                    .AddLine(landingEnd.X, landingEnd.Y)
                    .WithStroke(_dimensionLineColor).WithStrokeWidth(_dimensionLineWidth));

            CreateDebugPoint(textPos, "aqua");
        }


        private string GetFormattedDimensionText(double textSize) {
            string text = _dimension.Text;
            if (!string.IsNullOrEmpty(text)) {
                return text;
            }
            return CreateMeasurementText(textSize);
        }


        /// <summary>
        /// Creates the measurement text from the measurement value as specified by the properties
        /// of the associated <see cref="DimensionStyle"/> object or property values overriden
        /// by values from the <see cref="Dimension"/> object's <see cref="CadObject.ExtendedData"/>.
        /// This implementation supports <see cref="DimensionLinear">linear dimensions</see>
        /// with primary measurement and alternate-unit values with the specified respective 
        /// <see cref="LinearUnitFormat"/> with or without tolerance.
        /// <param name="textSize"></param>
        /// <remarks><para>
        /// Formatting complient with the specified <see cref="LinearUnitFormat"/> is provided
        /// by a class derived from <see cref="LinearMeasurementFormatter"/>.
        /// </para><para>
        /// The created measuement text is coded using AutoCAD "markup" as used for MTEXT entities.
        /// </para></remarks>
        /// <returns>
        /// The created measuerement text.
        /// </returns>
        /// 
        protected virtual string CreateMeasurementText(double textSize) {
            LinearMeasurementFormatter mFt = LinearMeasurementFormatter.CreateMeasurementFormatter(
                _dimProps.LinearUnitFormat, _dimension, _dimProps, _defaultPostFix, textSize);
            LinearMeasurementFormatter aFt = LinearMeasurementFormatter.CreateMeasurementFormatter(
                _dimProps.AlternateUnitFormat, _dimension, _dimProps, _defaultPostFix, textSize);

            switch (GetToleranceOption()) {
            default:
            case 0: //  Just value and optionally alternate value, no tolerance
                string m = mFt.FormatMeasurement();
                if (!_dimProps.AlternateUnitDimensioning) {
                    return m;
                }
                else {
                    string am = aFt.FormatAlternate();
                    return $"{m} [{am}]";
                }

            case 1:
                //  Symmetric: ±{PlusTolerance}
                //  -  GenerateTolerance = true
                //  -  MinusTolerance == PlusTolerance" (Exactly equal)
                string mt = $"{mFt.FormatMeasurement()}{mFt.FormatMeasurementToleranceSymmetric()}";
                if (!_dimProps.AlternateUnitDimensioning) {
                    return mt;
                }
                else {
                    string amt = $"{aFt.FormatAlternate()}{aFt.FormatAlternateToleranceSymmetric()}";
                    return $"{mt} [{amt}]";
                }

            case 2:
                //  Deviation: +{PlusTolerance}^-{MinusTolerance}, aligned
                //  -  GenerateTolerance = true
                //  -  MinusTolerance != PlusTolerance" (Difference may be below precision if, e.g., +5^-5
                string alignment = getVerticalAlingnment();
                string mtpm = $"{alignment}{mFt.FormatMeasurement()}{mFt.FormatMeasurementTolerancePlusMinus()}";
                if (!_dimProps.AlternateUnitDimensioning) {
                    return mtpm;
                }
                else {
                    string squareBra = @"{\H1.8x;[}";
                    string squareKet = @"{\H1.8x;]}";
                    string amtpm = $"{alignment}{aFt.FormatAlternate()}{aFt.FormatAlternateTolerancePlusMinus()}";
                    return $"{mtpm} {squareBra}{amtpm}{squareKet}";
                }
            case 3:
                //  Limits/oben:  +{Measurement+PlusTolerance}^-{Measurement-MinusTolerance}
                //  -  LimitsGeneration = true
                string alignmentl = getVerticalAlingnment();
                string ml = $"{alignmentl}{mFt.FormatMeasurementLimits()}";
                if (!_dimProps.AlternateUnitDimensioning) {
                    return ml;
                }
                else {
                    string squareBra = @"{\H1.8x;[}";
                    string squareKet = @"{\H1.8x;]}";
                    string aml = $"{alignmentl}{aFt.FormatAlternateLimits()}";
                    return $"{ml} {squareBra}{aml}{squareKet}";
                }
            }
        }


        /// <summary>
        /// Gets a value indicating whether the measurement contains the measurement only
        /// or with a symmetric tolerance ± or stacked +/- tolerance specification, or
        /// a stacked limits specification.
        /// </summary>
        /// <returns></returns>
        protected int GetToleranceOption() {
            if (_dimProps.GenerateTolerances) {
                if (_dimProps.MinusTolerance == _dimProps.PlusTolerance) {
                    return 1;
                }
                else {
                    return 2;
                }
            }
            else if (_dimProps.LimitsGeneration) {
                return 3;
            }
            else {
                return 0;
            }
        }


        protected string getVerticalAlingnment() {
            //  Top:    \A2;80,26{\H1.875;\S+5^ -5;} {\o\l\H4.5;[}3,160{\H1.875;\S+0,197^ -0,197;}{\o\l\H4.5;]}
            //  Middle: \A1;80,26{\H1.875;\S+5^ -5;} {\o\l\H4.5;[}3,160{\H1.875;\S+0,197^ -0,197;}{\o\l\H4.5;]}
            //  Bottom:     80,26{\H1.875;\S+5^ -5;} {\o\l\H4.5;[}3,160{\H1.875;\S+0,197^ -0,197;}{\o\l\H4.5;]}

            switch (_dimProps.ToleranceAlignment) {
            case ToleranceAlignment.Top:
                return @"\A2;";

            case ToleranceAlignment.Middle:
                return @"\A1;";

            default:
            case ToleranceAlignment.Bottom:
                return string.Empty;
            }
        }


        protected void CreateDimensionLine(XY dla, XY dle) {
            _groupElement.Children.Add(new PathElement()
                .AddLine(dla.X, dla.Y, dle.X, dle.Y)
                .WithStroke(_dimensionLineColor).WithStrokeWidth(_dimensionLineWidth));
        }


        protected void CreateExtensionLine(XY point, XY definitionPoint, XY extDir) {
            XY exta = point + _dimProps.ExtensionLineOffset * extDir;
            XY exte = definitionPoint + _dimProps.ExtensionLineExtension * extDir;

            _groupElement.Children.Add(new PathElement()
                .AddLine(exta.X, exta.Y, exte.X, exte.Y)
                .WithStroke(_extensionLineColor)
                .WithStrokeWidth(_extensionLineWidth));
        }


        protected void CreateFirstExtensionLine(XY point, XY arcPoint, XY extDir) {
            if (_dimProps.SuppressFirstExtensionLine) {
                return;
            }

            CreateExtensionLine(point, arcPoint, extDir);
        }


        protected void CreateSecondExtensionLine(XY point, XY arcPoint, XY extDir) {
            if (_dimProps.SuppressSecondExtensionLine) {
                return;
            }

            CreateExtensionLine(point, arcPoint, extDir);
        }


        protected void CreateDimensionLineExtension(XY arrowPoint, XY textPoint, XY dimDir, double arrowSize, bool textOutside, bool arrowOutside, double textLen) {
            if (arrowOutside || textOutside) {
                XY dexta = arrowOutside ? arrowPoint + dimDir * arrowSize : arrowPoint;
                XY dexte = textOutside ? textPoint + dimDir * textLen / 2 : arrowPoint + dimDir * 2 * arrowSize;
                _groupElement.Children.Add(new PathElement()
                    .AddLine(dexta.X, dexta.Y, dexte.X, dexte.Y)
                    .WithStroke(_dimensionLineColor).WithStrokeWidth(_dimensionLineWidth));
            }
        }


        protected void GetArrowsOutside(double dimensionLineLength, out bool firstArrowOutside, out bool secondArrowOutside) {
            bool arrowsOutside = dimensionLineLength < _arrowSize * 2.8;
            firstArrowOutside = arrowsOutside ^ _dimension.FlipArrow2;
            secondArrowOutside = arrowsOutside ^ _dimension.FlipArrow1;
        }


        public void CreateArrowHead(BlockRecord arrowHeadBlock, XY arrowPoint, XY arrowDirection) {
            if (arrowHeadBlock != null) {
                _groupElement.Children.Add(XElementFactory.CreateArrowheadFromBlock(arrowHeadBlock, arrowPoint, arrowDirection * _arrowSize, _arrowSize));
            }
            else {
                _groupElement.Children.Add(XElementFactory.CreateStandardArrowHead(arrowPoint, arrowDirection * _arrowSize, _dimensionLineColor));
            }
        }


        protected void CreateDebugPoint(XY point, string color) {
#if DEBUG
            _groupElement.Children.Add(new CircleElement() { Cx = point.X, Cy = point.Y, R = 0.25 }.WithStroke(color));
#endif
        }


        private bool isCcwPath(XY p0, XY p1, XY p2) {
            double a1 = (p1 - p0).GetAngle();
            if (a1 < 0) {
                a1 += Math.Tau;
            }
            double a2 = (p2 - p1).GetAngle();
            if (a2 < 0) {
                a2 += Math.Tau;
            }
            return a1 > a2;
        }


        protected XY GetMidpoint(XY point1, XY point2) {
            double midX = (point1.X + point2.X) / 2;
            double midY = (point1.Y + point2.Y) / 2;
            return new XY(midX, midY);
        }
    }
}
