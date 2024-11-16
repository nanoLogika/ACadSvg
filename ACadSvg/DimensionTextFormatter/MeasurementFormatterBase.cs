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
            },
            new string[128] {
                "",
                "1/128", "1/64", "3/128", "1/32", "5/128", "3/64", "7/128", "1/16",
                "9/128", "5/64", "11/128", "3/32", "13/128", "7/64", "15/128", "1/8",
                "17/128", "9/64", "19/128", "5/32", "21/128", "11/64", "23/128", "3/16",
                "25/128", "13/64", "27/128", "7/32", "29/128", "15/64", "31/128", "1/4",
                "33/128", "17/64", "35/128", "9/32", "37/128", "19/64", "39/128", "5/16",
                "41/128", "21/64", "43/128", "11/32", "45/128", "23/64", "47/128", "3/8",
                "49/128", "25/64", "51/128", "13/32", "53/128", "27/64", "55/128", "7/16",
                "57/128", "29/64", "59/128", "15/32", "61/128", "31/64", "63/128", "1/2",
                "65/128", "33/64", "67/128", "17/32", "69/128", "35/64", "71/128", "9/16",
                "73/128", "37/64", "75/128", "19/32", "77/128", "39/64", "79/128", "5/8",
                "81/128", "41/64", "83/128", "21/32", "85/128", "43/64", "87/128", "11/16",
                "89/128", "45/64", "91/128", "23/32", "93/128", "47/64", "95/128", "3/4",
                "97/128", "49/64", "99/128", "25/32", "101/128", "51/64", "103/128", "13/16",
                "105/128", "53/64", "107/128", "27/32", "109/128", "55/64", "111/128", "7/8",
                "113/128", "57/64", "115/128", "29/32", "117/128", "59/64", "119/128", "15/16",
                "121/128", "61/64", "123/128", "31/32", "125/128", "63/64", "127/128"
            },
            new string[256] {
                "",
                "1/256", "1/128", "3/256", "1/64", "5/256", "3/128", "7/256", "1/32",
                "9/256", "5/128", "11/256", "3/64", "13/256", "7/128", "15/256", "1/16",
                "17/256", "9/128", "19/256", "5/64", "21/256", "11/128", "23/256", "3/32",
                "25/256", "13/128", "27/256", "7/64", "29/256", "15/128", "31/256", "1/8",
                "33/256", "17/128", "35/256", "9/64", "37/256", "19/128", "39/256", "5/32",
                "41/256", "21/128", "43/256", "11/64", "45/256", "23/128", "47/256", "3/16",
                "49/256", "25/128", "51/256", "13/64", "53/256", "27/128", "55/256", "7/32",
                "57/256", "29/128", "59/256", "15/64", "61/256", "31/128", "63/256", "1/4",
                "65/256", "33/128", "67/256", "17/64", "69/256", "35/128", "71/256", "9/32",
                "73/256", "37/128", "75/256", "19/64", "77/256", "39/128", "79/256", "5/16",
                "81/256", "41/128", "83/256", "21/64", "85/256", "43/128", "87/256", "11/32",
                "89/256", "45/128", "91/256", "23/64", "93/256", "47/128", "95/256", "3/8",
                "97/256", "49/128", "99/256", "25/64", "101/256", "51/128", "103/256", "13/32",
                "105/256", "53/128", "107/256", "27/64", "109/256", "55/128", "111/256", "7/16",
                "113/256", "57/128", "115/256", "29/64", "117/256", "59/128", "119/256", "15/32",
                "121/256", "61/128", "123/256", "31/64", "125/256", "63/128", "127/256", "1/2",
                "129/256", "65/128", "131/256", "33/64", "133/256", "67/128", "135/256", "17/32",
                "137/256", "69/128", "139/256", "35/64", "141/256", "71/128", "143/256", "9/16",
                "145/256", "73/128", "147/256", "37/64", "149/256", "75/128", "151/256", "19/32",
                "153/256", "77/128", "155/256", "39/64", "157/256", "79/128", "159/256", "5/8",
                "161/256", "81/128", "163/256", "41/64", "165/256", "83/128", "167/256", "21/32",
                "169/256", "85/128", "171/256", "43/64", "173/256", "87/128", "175/256", "11/16",
                "177/256", "89/128", "179/256", "45/64", "181/256", "91/128", "183/256", "23/32",
                "185/256", "93/128", "187/256", "47/64", "189/256", "95/128", "191/256", "3/4",
                "193/256", "97/128", "195/256", "49/64", "197/256", "99/128", "199/256", "25/32",
                "201/256", "101/128", "203/256", "51/64", "205/256", "103/128", "207/256", "13/16",
                "209/256", "105/128", "211/256", "53/64", "213/256", "107/128", "215/256", "27/32",
                "217/256", "109/128", "219/256", "55/64", "221/256", "111/128", "223/256", "7/8",
                "225/256", "113/128", "227/256", "57/64", "229/256", "115/128", "231/256", "29/32",
                "233/256", "117/128", "235/256", "59/64", "237/256", "119/128", "239/256", "15/16",
                "241/256", "121/128", "243/256", "61/64", "245/256", "123/128", "247/256", "31/32",
                "249/256", "125/128", "251/256", "63/64", "253/256", "127/128", "255/256"
            }
        };


        private static string ToFraction(double fraction, short precision) {
            if (precision < 1) {
                return string.Empty;
            }
            if (precision > 8) {
                precision = 8;
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
                if (text.Contains(decimalSeparator)) {
                    return text.TrimEnd('0').TrimEnd(decimalSeparator);
                }
                else {
                    return text;
                }

            case ZeroHandling.SuppressDecimalLeadingAndTrailingZeroes:
                getSignAndNumber(text, out string number2, out string sign2);
                if (number2.Contains(decimalSeparator)) {
                    return sign2 + number2.TrimStart('0').TrimEnd('0').TrimEnd(decimalSeparator);
                }
                else {
                    return sign2 + number2.TrimStart('0');
                }
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
