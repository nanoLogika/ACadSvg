#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using ACadSharp.Tables;
using ACadSharp.Types.Units;

using ACadSvg.Extensions;

namespace ACadSvg.DimensionTextFormatter {

    /// <summary>
    /// The base class for linear formatters used by linear dimension entities.
    /// The text created contains a value part with or without tolerance and optionally an
    /// alternate-unit value with or without tolerance. Various formatters according to the
    /// specified <see cref="DimensionProperties.LinearUnitFormat"/> can be created with the
    /// for linear dimension <see cref="CreateMeasurementFormatter"/> method.
    /// </summary>
    internal abstract class LinearMeasurementFormatter : MeasurementFormatterBase {

        /// <summary>
        /// Initializes a new instance of an linear measurement formatter.
        /// </summary>
        /// <param name="dimension">The<see cref="Dimension"/> object the dimension text is to be created for.</param>
        /// <param name="dimProps">A <see cref="DimensionProperties"/> object providing all
        /// dimension-style properties specified in the <see cref="DimensionStyle"/> object
        /// associated with the dimension object possibly overriden by the dimension object's
        /// extended data.</param>
        /// <param name="defaultPostFix">A string specifying a default prefix and/or postfix
        /// used for both the primary and the alternate value if no prefix/postfix is specified
        /// by the dimension-style properties.</param>
        /// <remarks>
        /// </remarks>
        protected LinearMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
            : base(dimension, dimProps, defaultPostFix, textSize) {
        }


        /// <summary>
        /// Creates a new instance of an linear measurement formatter as specified by the
        /// <paramref name="format"/> parameter. 
        /// </summary>
        /// <param name="dimension">The<see cref="Dimension"/> object the dimension text is to be created for.</param>
        /// <param name="dimProps">A <see cref="DimensionProperties"/> object providing all
        /// dimension-style properties specufied in the <see cref="DimensionStyle"/> object
        /// associated with the dimension object possibly overriden by the dimension object's
        /// extended data.</param>
        /// <param name="defaultPostFix">A string specifying a default prefix and/or postfix
        /// used for both the primary and the alternate value if no prefix/postfix is specified
        /// by the dimension-style properties.</param>
        /// <returns>
        /// An angular measurement formatter as specified by the <paramref name="format"/>
        /// parameter. The returned formatter can be a <see cref="DecimalMeasurementFormatter"/>,
        /// <see cref="EngineeringMeasurementFormatter"/>, <see cref="ScientificMeasurementFormatter"/>,
        /// <see cref="FractionalMeasurementFormatter"/>, <see cref="ArchitecturalMeasurementFormatter"/>,
        /// or <see cref="WindowsDesktopMeasurementFormatter"/>.
        /// </returns>
        public static LinearMeasurementFormatter CreateMeasurementFormatter(
            LinearUnitFormat format,
            Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize) {

            switch (format) {
            default:
            case LinearUnitFormat.Decimal:
                return new DecimalMeasurementFormatter(dimension, dimProps, defaultPostFix, textSize);

            case LinearUnitFormat.Engineering:
                return new EngineeringMeasurementFormatter(dimension, dimProps, defaultPostFix, textSize);

            case LinearUnitFormat.Scientific:
                return new ScientificMeasurementFormatter(dimension, dimProps, defaultPostFix, textSize);

            case LinearUnitFormat.Fractional:
                return new FractionalMeasurementFormatter(dimension, dimProps, defaultPostFix, textSize);

            case LinearUnitFormat.Architectural:
                return new ArchitecturalMeasurementFormatter(dimension, dimProps, defaultPostFix, textSize);

            case LinearUnitFormat.WindowsDesktop:
                return new WindowsDesktopMeasurementFormatter(dimension, dimProps, defaultPostFix, textSize);
            }
        }


        /// <inheritdoc/>
        /// <remarks>
        /// Measuement
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearScaledAndRoundedTo"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearFormattedAs"]'/>
        /// <see cref="DimensionProperties.DecimalPlaces"/>
        /// <see cref="DimensionProperties.ZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="primaryPostFix"]'/>
        /// </remarks>
        public override string FormatMeasurement() {
            double measurement = GetValueScaledAndRounded(_dimension.Measurement);
            string m = FormatValue(measurement, _dimProps.DecimalPlaces, _dimProps.ZeroHandling);
            return GetTextWithPostFix(m);
        }


        /// <summary>
        /// Formats the alternate-unit measurement value The formatted numerical value is prefixed and/or
        /// suffixed as specified by the <see cref="DimensionProperties.PostFix"/> style property.
        /// If no PostFix style property is specified the default Postfix specified for this formatter
        /// is used.
        /// <returns>A string containing the formatted alternate-unit measurement value.</returns>
        /// <remarks>
        /// Measuement
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearAlternateScaledAndRoundedTo"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearFormattedAs"]'/>
        /// <see cref="DimensionProperties.AlternateUnitDecimalPlaces"/>
        /// <see cref="DimensionProperties.AlternateUnitZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="alternatePostFix"]'/>
        /// </remarks>
        public virtual string FormatAlternate() {
            double alternateMeasurement = GetAlternateValueScaledAndRounded(_dimension.Measurement);
            string m = FormatValue(alternateMeasurement, _dimProps.AlternateUnitDecimalPlaces, _dimProps.AlternateUnitZeroHandling);
            return GetAlternateTextWithPostFix(m);
        }


        /// <inheritdoc />
        /// <remarks>
        /// <see cref="DimensionProperties.PlusTolerance" />
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearScaledAndRoundedTo"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearFormattedAs"]'/>
        /// <see cref="DimensionProperties.ToleranceDecimalPlaces"/>
        /// <see cref="DimensionProperties.ToleranceZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="primaryPostFix"]'/>
        /// </remarks>
        public override string FormatMeasurementToleranceSymmetric() {
            double tolerance = GetValueScaledAndRounded(_dimProps.PlusTolerance);
            string m = FormatValue(tolerance, _dimProps.ToleranceDecimalPlaces, _dimProps.ToleranceZeroHandling);
            return "±" + GetTextWithPostFix(m);

        }


        /// <summary>
        /// Formats the alternate-unit tolerance value assuming that the <see cref="DimensionProperties.PlusTolreance"/>
        /// and the <see cref="DimensionProperties.MinusTolreance"/> are equal. The formatted tolerance
        /// value is prefixed with "±" and should follow the alternate-unit measurement value.
        /// </summary>
        /// <returns>A string containing the formatted alternate-unit tolerance value.</returns>
        /// <remarks>
        /// <see cref="DimensionProperties.PlusTolerance" />
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearAlternateScaledAndRoundedTo"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearFormattedAs"]'/>
        /// <see cref="DimensionProperties.AlternateUnitToleranceDecimalPlaces"/>
        /// <see cref="DimensionProperties.AlternateUnitToleranceZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="alternatePostFix"]'/>
        /// </remarks>
        public virtual string FormatAlternateToleranceSymmetric() {
            double alternateTolerance = GetAlternateValueScaledAndRounded(_dimProps.PlusTolerance);
            string m = FormatValue(alternateTolerance, _dimProps.AlternateUnitToleranceDecimalPlaces, _dimProps.AlternateUnitToleranceZeroHandling);
            return "±" + GetAlternateTextWithPostFix(m);
        }


        /// <inheritdoc />
        /// <remarks>
        /// <see cref="DimensionProperties.MinusTolerance" />, <see cref="DimensionProperties.PlusTolerance" />
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearScaledAndRoundedToPlural"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearFormattedAs"]'/>
        /// <see cref="DimensionProperties.ToleranceDecimalPlaces"/>
        /// <see cref="DimensionProperties.ToleranceZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="primaryPostFix"]'/>
        /// </remarks>
        public override string FormatMeasurementTolerancePlusMinus() {
            double minusTolerance = GetValueScaledAndRounded(_dimProps.MinusTolerance);
            double plusTolerance = GetValueScaledAndRounded(_dimProps.PlusTolerance);

            string minusToleranceFormatted = FormatValue(minusTolerance, _dimProps.DecimalPlaces, _dimProps.ZeroHandling);
            string plusToleranceFormatted = FormatValue(plusTolerance, _dimProps.DecimalPlaces, _dimProps.ZeroHandling);

            return FormatToleranceStacked(
                "-" + GetTextWithPostFix(minusToleranceFormatted),
                "+" + GetTextWithPostFix(plusToleranceFormatted));
        }


        /// <summary>
        /// Formats the alternate-unit tolerance value assuming that the
        /// <see cref="DimensionProperties.PlusTolreance"/> and the <see cref="DimensionProperties.MinusTolreance"/>
        /// are not equal. The values are stacked, coded with AutoCAD-MTEXT markup-commands.
        /// The <see cref="DimensionProperties.PlusTolreance"/> is placed above, prefixed with a "+" sign,
        /// the <see cref="DimensionProperties.MinusTolreance"/> is placed below, prefixed with a "-" sign.
        /// The formatted stacked alternate tolerance values should immediately follow the alternate value.
        /// The text size of the stacked values is controlled by the
        /// <see cref="DimensionProperties.ToleranceScaleFactor"/> property.
        /// </summary>
        /// <returns>A string containing the formatted stacked alternate-unit tolerance values.</returns>
        /// <remarks>
        /// <see cref="DimensionProperties.MinusTolerance" />, <see cref="DimensionProperties.PlusTolerance" />
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearAlternateScaledAndRoundedToPlural"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearFormattedAs"]'/>
        /// <see cref="DimensionProperties.AlternateUnitToleranceDecimalPlaces"/>
        /// <see cref="DimensionProperties.AlternateUnitToleranceZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="alternatePostFix"]'/>
        /// </remarks>
        public virtual string FormatAlternateTolerancePlusMinus() {
            double minusTolerance = GetAlternateValueScaledAndRounded(_dimProps.MinusTolerance);
            double plusTolerance = GetAlternateValueScaledAndRounded(_dimProps.PlusTolerance);

            string minusToleranceFormatted = FormatValue(minusTolerance, _dimProps.AlternateUnitDecimalPlaces, _dimProps.AlternateUnitZeroHandling);
            string plusToleranceFormatted = FormatValue(plusTolerance, _dimProps.AlternateUnitDecimalPlaces, _dimProps.AlternateUnitZeroHandling);

            return FormatToleranceStacked(
                "-" + GetAlternateTextWithPostFix(minusToleranceFormatted),
                "+" + GetAlternateTextWithPostFix(plusToleranceFormatted));
        }


        /// <inheritdoc />
        /// <remarks>
        /// The limits
        /// (Measurement - <see cref="DimensionProperties.MinusTolerance" /> and
        /// Measurement + <see cref="DimensionProperties.PlusTolerance" />)
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearScaledAndRoundedToPlural"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearFormattedAs"]'/>
        /// <see cref="DimensionProperties.DecimalPlaces"/>
        /// <see cref="DimensionProperties.ZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="primaryPostFix"]'/>
        /// </remarks>
        public override string FormatMeasurementLimits() {
            double minLimit = GetValueScaledAndRounded(_dimension.Measurement - _dimProps.MinusTolerance);
            double maxLimit = GetValueScaledAndRounded(_dimension.Measurement + _dimProps.PlusTolerance);

            short decimalPlaces = _dimProps.DecimalPlaces;
            ZeroHandling zeroHandling = _dimProps.ZeroHandling;
            string minLimitFormatted = FormatValue(minLimit, decimalPlaces, zeroHandling);
            string maxLimitFormatted = FormatValue(maxLimit, decimalPlaces, zeroHandling);

            return FormatToleranceStacked(
                GetTextWithPostFix(minLimitFormatted),
                GetTextWithPostFix(maxLimitFormatted));
        }


        /// <summary>
        /// The alternate-unit tolerance is to be shown as a maximum and minimum value stacked, coded with AutoCAD-MTEXT
        /// markup-commands. The maximum value (alternate-unit measurement + <see cref="DimensionProperties.PlusTolreance"/>)
        /// is placed above the minimum value (alternate-unit measurement - <see cref="DimensionProperties.MinusTolreance"/>)
        /// is placed below.
        /// The text size of the stacked values is controlled by the
        /// <see cref="DimensionProperties.ToleranceScaleFactor"/> property.
        /// </summary>
        /// <returns>A string containing the formatted stacked alternate-unit limits values.</returns>
        /// <remarks>
        /// The limits
        /// Measurement - <see cref="DimensionProperties.MinusTolerance" />,
        /// measurement + <see cref="DimensionProperties.PlusTolerance" />
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearAlternateScaledAndRoundedToPlural"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="linearFormattedAs"]'/>
        /// <see cref="DimensionProperties.AlternateUniteDecimalPlaces"/> and
        /// <see cref="DimensionProperties.AlternateUnitZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="alternatePostFix"]'/>
        /// </remarks>
        public virtual string FormatAlternateLimits() {
            double minAlternateLimit = GetAlternateValueScaledAndRounded(_dimension.Measurement - _dimProps.MinusTolerance);
            double maxAlternateLimit = GetAlternateValueScaledAndRounded(_dimension.Measurement + _dimProps.PlusTolerance);

            short decimalPlaces = _dimProps.AlternateUnitDecimalPlaces;
            ZeroHandling zeroHandling = _dimProps.AlternateUnitZeroHandling;
            string minAlternateLimitFormatted = FormatValue(minAlternateLimit, decimalPlaces, zeroHandling);
            string maxAlternateLimitFormatted = FormatValue(maxAlternateLimit, decimalPlaces, zeroHandling);

            return FormatToleranceStacked(
                GetAlternateTextWithPostFix(minAlternateLimitFormatted),
                GetAlternateTextWithPostFix(maxAlternateLimitFormatted));
        }


        /// <summary>
        /// Adds a prefix and/or a suffix specified by the <see cref="DimensionProperties.AlternateDimensioningSuffix"/>
        /// property to the <paramref name="text"/>. If the <see cref="DimensionProperties.AlternateDimensioningSuffix"/>
        /// is empty the default PostFix is used.
        /// </summary>
        /// <param name="text">A string the prefix and/or suffix is to be added to.</param>
        /// <param name="defaultPostFix">Default prefix/suffix, to be used if the
        /// <see cref="DimensionProperties.AlternateDimensioningSuffix"/> is empty.</param>
        /// <returns>The <paramref name="text"/> with the prefix and/or suffix added.</returns>
        protected string GetAlternateTextWithPostFix(string text) {
            string defaultPostFix = _defaultPostFix.Replace("<>", "[]");
            string postFix = _dimProps.AlternateDimensioningSuffix;
            if (string.IsNullOrEmpty(postFix)) {
                postFix = defaultPostFix;
            }
            else if (!postFix.Contains("[]")) {
                postFix = "[]" + postFix;
            }
            return postFix.Replace("[]", text);
        }


        /// <summary>
        /// Scales the specified measurement or tolerance value with the
        /// <see cref="DimensionProperties.LinearScaleFactor"/>
        /// </summary>
        /// <param name="value">The value to be scaled.</param>
        /// <returns>The scaled value.</returns>
        protected double GetValueScaledAndRounded(double value) {
            double measurementScaled = value * _dimProps.LinearScaleFactor;
            return RoundByUnit(measurementScaled, _dimProps.Rounding);
        }


        /// <summary>
        /// Scales the specified measurement or tolerance value with the
        /// <see cref="DimensionProperties.LinearScaleFactor"/>.
        /// To obtain the value in alternate units it is multiplied with the
        /// <see cref="DimensionProperties.AlternateUnitScaleFactor"/>.
        /// </summary>
        /// <param name="value">The value to be scaled.</param>
        /// <returns>The scaled value.</returns>
        protected double GetAlternateValueScaledAndRounded(double value) {
            double alternateMeasurementScaled = value * _dimProps.LinearScaleFactor * _dimProps.AlternateUnitScaleFactor;
            return RoundByUnit(alternateMeasurementScaled, _dimProps.AlternateUnitRounding);
        }
    }
}