#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Globalization;

using ACadSharp.Entities;
using ACadSharp.Tables;

using ACadSvg.Extensions;

using SvgElements;


namespace ACadSvg.DimensionTextFormatter {

    /// <summary>
    /// Base class for measurement formatters. Measurement formatters are used to create
    /// the parts of the measurement text such as measurement value with or without
    /// tolerance and optionally an alternate scaled measurement value with or without
    /// tolerance. Various formatters can be applied according to the specified format
    /// for linear dimension (see <see cref="LinearMeasurementFormatter"/>)
    /// and angular dimensions (see <see cref="AngularMeasurementFormatter"/>)
    /// respectively.
    /// </summary>
    internal abstract class MeasurementFormatterBase {

        protected readonly Dimension _dimension;
        protected readonly DimensionProperties _dimProps;
        protected readonly string _defaultPostFix;
        protected readonly double _textSize;


        /// <summary>
        /// Initializes a new instance of a measurement formatter.
        /// </summary>
        /// <param name="dimension">The <see cref="Dimension"/> object the dimension text is to be created for.</param>
        /// <param name="dimProps">A <see cref="DimensionProperties"/> object providing all
        /// dimension-style properties specufied in the <see cref="DimensionStyle"/> object
        /// associated with the dimension object possibly overriden by the dimension object's
        /// extended data.</param>
        /// <param name="defaultPostFix">A string specifying a default prefix and/or postfix
        /// used for both the primary and the alternate value if no prefix/postfix is specified
        /// by the dimension-style properties.</param>
        /// 
        protected MeasurementFormatterBase(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize) {
            _dimension = dimension;
            _dimProps = dimProps;
            _defaultPostFix = string.IsNullOrEmpty(defaultPostFix) ? "<>" : defaultPostFix;
            _textSize = textSize;
        }


        /// <summary>
        /// If implemented by a derived class converts the specified value into a string
        /// in a specific format.
        /// </summary>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="valueParameter"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="decimalPlacesParameter"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="zeroHandlingParameter"]/*'/>
        /// <returns>The converted value in the specific format.</returns>
        protected abstract string FormatValue(double value, short decimalplaces, ZeroHandling zeroHandling);


        /// <summary>
        /// Formats the (primary) measurement value. The formatted numerical value is prefixed and/or
        /// suffixed as specified by the <see cref="DimensionProperties.PostFix"/> style property.
        /// If no PostFix style property is specified the default Postfix specified for this formatter
        /// is used.
        /// </summary>
        /// <returns>A string containing formatted measurement value.</returns>
        public abstract string FormatMeasurement();


        /// <summary>
        /// Formats the tolerance value assuming that the <see cref="DimensionProperties.PlusTolreance"/>
        /// and the <see cref="DimensionProperties.MinusTolreance"/> are equal. The formatted tolerance
        /// value is prefixed with "±" and should follow the primary measurement value.
        /// </summary>
        /// <returns>A string containing the formatted tolerance value.</returns>
        public abstract string FormatMeasurementToleranceSymmetric();


        /// <summary>
        /// Formats the a tolerance value assuming that the <see cref="DimensionProperties.PlusTolreance"/>
        /// and the <see cref="DimensionProperties.MinusTolreance"/> are not equal. The values are stacked,
        /// coded with AutoCAD-MTEXT markup-commands.
        /// The <see cref="DimensionProperties.PlusTolreance"/> is placed above, prefixed with a "+" sign,
        /// the <see cref="DimensionProperties.MinusTolreance"/> is placed below, prefixed with a "-" sign.
        /// The formatted stacked tolerance values should immediately follow the primary measurement value.
        /// </summary>
        /// <returns>A string containing the formatted stacked tolerance values.</returns>
        public abstract string FormatMeasurementTolerancePlusMinus();


        /// <summary>
        /// The tolerance is to be shown as a maximum and minimum value stacked coded with AutoCAD-MTEXT
        /// markup-commands. The maximum value (measurement + <see cref="DimensionProperties.PlusTolreance"/>)
        /// is placed above the minimum value (measurement - <see cref="DimensionProperties.MinusTolreance"/>)
        /// is placed below.
        /// The text size of the stacked values is controlled by the
        /// <see cref="DimensionProperties.ToleranceScaleFactor"/> property.
        /// </summary>
        /// <param name="textSize"></param>
        /// <returns>A string containing the formatted stacked limits values.</returns>
        public abstract string FormatMeasurementLimits();


        /// <summary>
        /// Adds a prefix and/or a suffix specified by the <see cref="DimensionProperties.PostFix"/>
        /// property to the <paramref name="text"/>. If the <see cref="DimensionProperties.PostFix"/>
        /// is empty the default PostFix is used.
        /// </summary>
        /// <param name="text">A string the prefix and/or suffix is to be added to.</param>
        /// <param name="defaultPostFix">Default prefix/suffix, to be used if the
        /// <see cref="DimensionProperties.PostFix"/> is empty.</param>
        /// <returns>The <paramref name="text"/> with the prefix and/or suffix added.</returns>
        protected string GetTextWithPostFix(string text) {
            string postFix = _dimProps.PostFix;
            if (string.IsNullOrEmpty(postFix)) {
                postFix = _defaultPostFix;
            }
            else if (!postFix.Contains("<>")) {
                postFix = "<>" + postFix;
            }
            return postFix.Replace("<>", text);
        }


        /// <summary>
        /// Rounds the specified value to the specified unit.
        /// </summary>
        /// <param name="value">The value to be rounded.</param>
        /// <param name="roundingUnit"></param>
        /// <returns>The rounded value.</returns>
        protected static double RoundByUnit(double value, double roundingUnit) {
            if (roundingUnit == 0) {
                return value;
            }
            return Math.Round(value / roundingUnit, MidpointRounding.AwayFromZero) * roundingUnit;
        }


        /// <summary>
        /// Formats the specified <paramref name="value"/> as decimal number using decimal separator specified
        /// by <see cref="DimensionProperties.DecimalSeparator" /> and the specified <paramref name="decimalPlaces"/>
        /// and <paramref name="zeroHandling"/>.
        /// </summary>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="valueParameter"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="decimalPlacesParameter"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="zeroHandlingParameter"]/*'/>
        /// <returns>
        /// The formatted value as decimal number.
        /// </returns>
        protected string FormatDecimal(double value, short decimalPlaces, ZeroHandling zeroHandling) {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = _dimProps.DecimalSeparator.ToString();
            string format = "F" + decimalPlaces.ToString();
            return handleZerosDecimal(value.ToString(format, nfi), zeroHandling, _dimProps.DecimalSeparator);
        }


        /// <summary>
        /// Formats the specified <paramref name="value"/> as decimal number as exponential expression
        /// using decimal separator specified by <see cref="DimensionProperties.DecimalSeparator" />,
        /// the specified <paramref name="decimalPlaces"/> and <paramref name="zeroHandling"/>.
        /// </summary>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="valueParameter"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="decimalPlacesParameter"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="zeroHandlingParameter"]/*'/>
        /// <returns>
        /// The formatted value as decimal number as exponential expression.
        /// </returns>
        protected string FormatExponential(double value, short decimalPlaces, ZeroHandling zeroHandling) {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = _dimProps.DecimalSeparator.ToString();
            string format = "E" + decimalPlaces.ToString();
            return handleZerosDecimal(value.ToString(format, nfi), zeroHandling, _dimProps.DecimalSeparator);
        }


        /// <summary>
        /// Formats the specified value as integer part and fraction. 
        /// </summary>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="valueParameter"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="decimalPlacesParameterFraction"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="zeroHandlingParameter"]/*'/>
        /// <returns>
        /// The formatted value with integer part and fraction.
        /// </returns>
        protected static string FormatFractional(double value, short precision, ZeroHandling zeroHandling) {
            int intPart = roundToIntegerForFraction(value, precision);
            double fracPart = value - intPart;
            return $"{intPart}{ToFraction(fracPart, precision)}";
        }


        /// <summary>
        /// Formats the specified value (in inches) as feets and inches, and the fractional
        /// part of the inches as fraction.
        /// </summary> 
        /// <include file='_comments.xml' path='docTokens/docToken[@name="valueParameter"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="decimalPlacesParameterFraction"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="zeroHandlingParameter"]/*'/>
        /// <returns>
        /// The formatted value with feet, integer inches, and fraction.
        /// </returns>
        protected static string FormatInchesToFeetInchesFractional(double value, short precision, ZeroHandling zeroHandling) {
            int feet = Convert.ToInt32(Math.Truncate(value / 12));
            double fullinches = value - feet * 12;
            int inches = roundToIntegerForFraction(fullinches, precision);
            double fracInches = fullinches - inches;
            string fracInchesStr = ToFraction(fracInches, precision);

            return handleZerosFeetAndInchesFractional(feet, inches, fracInchesStr, zeroHandling);
        }


        /// <summary>
        /// Formats the specified value (in inches) as feet and inches as decimal number.
        /// </summary>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="valueParameter"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="decimalPlacesParameter"]/*'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="zeroHandlingParameter"]/*'/>
        /// <returns>
        /// The formatted value with feet and inches as decimal number.
        /// </returns>
        protected string FormatInchesToFeetInchesDecimal(double value, short decimalPlaces, ZeroHandling zeroHandling) {
            int feet = Convert.ToInt32(Math.Truncate(value / 12));
            double fullinches = value - feet * 12;
            int inches = Convert.ToInt32(Math.Truncate(fullinches));
            string fullinchesStr = FormatDecimal(fullinches, decimalPlaces, zeroHandling);

            return handleZerosFeetAndInchesDecimal(feet, inches, fullinchesStr, zeroHandling);
        }


        private static int roundToIntegerForFraction(double value, short precision) {
            double fracHalfStep = 1 / Math.Pow(2, precision) / 2;
            int inches = Convert.ToInt32(Math.Truncate(value + fracHalfStep));
            return inches;
        }


        private static List<string[]> _fractions = new List<string[]>(8) {
            new string[2] {
                "",
                "1/2"
            },
            new string[4] {
                "",
                "1/4", "1/2", "3/4"
            },
            new string[8] {
                "",
                "1/8", "1/4", "3/8", "1/2", "5/8", "3/4", "7/8"
            },
            new string[16] {
                "",
                "1/16", "1/8", "3/16", "1/4", "5/16", "3/8", "7/16", "1/2",
                "9/16", "5/8", "11/16", "3/4", "13/16", "7/8", "15/16"
            },
            new string[32] {
                "",
                "1/32", "1/16", "3/32", "1/8", "5/32", "3/16", "7/32", "1/4",
                "9/32", "5/16", "11/32", "3/8", "13/32", "7/16", "15/32", "1/2",
                "17/32", "9/16", "19/32", "5/8", "21/32", "11/16", "23/32", "3/4",
                "25/32", "13/16", "27/32", "7/8", "29/32", "15/16", "31/32"
            },
            new string[64] {
                "",
                "1/64", "1/32", "3/64", "1/16", "5/64", "3/32", "7/64", "1/8",
                "9/64", "5/32", "11/64", "3/16", "13/64", "7/32", "15/64", "1/4",
                "17/64", "9/32", "19/64", "5/16", "21/64", "11/32", "23/64", "3/8",
                "25/64", "13/32", "27/64", "7/16", "29/64", "15/32", "31/64", "1/2",
                "33/64", "17/32", "35/64", "9/16", "37/64", "19/32", "39/64", "5/8",
                "41/64", "21/32", "43/64", "11/16", "45/64", "23/32", "47/64", "3/4",
                "49/64", "25/32", "51/64", "13/16", "53/64", "27/32", "55/64", "7/8",
                "57/64", "29/32", "59/64", "15/16", "61/64", "31/32", "63/64"
            }
            //  TODO Add arrays 128, 256
        };


        private static string ToFraction(double fraction, short precision) {
            if (precision < 1) {
                return string.Empty;
            }
            if (precision > 6) {
                precision = 6;
            }
            double denominator = Math.Pow(2, precision);
            int index = Convert.ToInt32(Math.Floor(fraction * denominator));

            return _fractions[precision - 1][index];
        }


        private static void getSignAndNumber(string text, out string number, out string sign) {
            number = text;
            sign = string.Empty;
            if (text.StartsWith("+") || text.StartsWith("-")) {
                sign = text.Substring(0, 1);
                number = text.Substring(1);
            }
        }


        private static string handleZerosDecimal(string text, ZeroHandling zeroHandling, char decimalSeparator) {
            switch (zeroHandling) {
            case ZeroHandling.SuppressDecimalLeadingZeroes:
                getSignAndNumber(text, out string number, out string sign);
                return sign + number.TrimStart('0');

            case ZeroHandling.SuppressDecimalTrailingZeroes:
                return text.TrimEnd('0').TrimEnd(decimalSeparator);

            case ZeroHandling.SuppressDecimalLeadingAndTrailingZeroes:
                getSignAndNumber(text, out string number2, out string sign2);
                return sign2 + number2.TrimStart('0').TrimEnd('0').TrimEnd(decimalSeparator);
            }

            return text;
        }


		private static string handleZerosFeetAndInchesFractional(int feet, int inches, string fracInches, ZeroHandling zeroHandling) {
            bool showFeet = true;
            bool showInches = true;
            
            switch (zeroHandling) {
				case ZeroHandling.SuppressZeroFeetAndInches:
                    showFeet = feet != 0;
                    showInches = inches != 0;
                    break;
				case ZeroHandling.ShowZeroFeetAndInches:
					break;
				case ZeroHandling.ShowZeroFeetSuppressZeroInches:
                    showInches = inches != 0;
					break;
				case ZeroHandling.SuppressZeroFeetShowZeroInches:
                    showFeet = feet != 0;
					break;
			}

            string output = string.Empty;
            if (showFeet) {
                output += $"{feet}'";
            }
            if (showInches) {
                if (output.Length > 0) {
                    output += "-";
                }
                output += $"{inches} {fracInches}\"";
            }
			return output;
		}


		private static string handleZerosFeetAndInchesDecimal(int feet, int inches, string fullinchesStr, ZeroHandling zeroHandling) {
			bool showFeet = true;
			bool showInches = true;

			switch (zeroHandling) {
				case ZeroHandling.SuppressZeroFeetAndInches:
					showFeet = feet != 0;
					showInches = inches != 0;
					break;
				case ZeroHandling.ShowZeroFeetAndInches:
					break;
				case ZeroHandling.ShowZeroFeetSuppressZeroInches:
					showInches = inches != 0;
					break;
				case ZeroHandling.SuppressZeroFeetShowZeroInches:
					showFeet = feet != 0;
					break;
			}

			string output = string.Empty;
			if (showFeet) {
				output += $"{feet}'";
			}
			if (showInches) {
				if (output.Length > 0) {
					output += "-";
				}
				output += $"{fullinchesStr}\"";
			}
			return output;
		}


        /// <summary>
        /// Creates a string containing stacked values <paramref name="plus"/> and <paramref name="minus"/>
        /// coded with AutoCAD/MTEXT markup commands. Depending on the
        /// <see cref="DimensionProperties.ToleranceScaleFactor"/> the text size is set.
        /// </summary>
        /// <param name="minus">String to be placed at bottom.</param>
        /// <param name="plus">String to be placed at top.</param>
        /// <returns>
        /// A string containing the stacked-expression of <paramref name="plus"/> and <paramref name="minus"/>.
        /// </returns>
		protected string FormatToleranceStacked(string minus, string plus) {

            double toleranceScaleFactor = _dimProps.ToleranceScaleFactor;

            if (toleranceScaleFactor == 1) {
                return $"\\S{plus}^{minus};";
            }
            else {
                string brace = "{";
                string ketce = "}";
                string height = SvgElementBase.Cd(_textSize * toleranceScaleFactor / 1.5);
                return $"{brace}\\H{height};\\S{plus}^{minus}; {ketce}";
            }
        }
    }
}
