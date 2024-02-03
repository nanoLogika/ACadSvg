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
    /// The base class for angular formatters used by angular dimension entities.
    /// The text created contains a value part with or without tolerance, but no
    /// alternate-unit value. Various formatters according to the specified
    /// <see cref="DimensionProperties.AngularUnitFormat"/> can be created with the
    /// <see cref="CreateMeasurementFormatter"/> method.
    /// </summary>
    internal abstract class AngularMeasurementFormatter : MeasurementFormatterBase {

        /// <summary>
        /// Initializes a new instance of an angular measurement formatter.
        /// </summary>
        /// <param name="dimension">The<see cref="Dimension"/> object the dimension text is to be created for.</param>
        /// <param name="dimProps">A <see cref="DimensionProperties"/> object providing all
        /// dimension-style properties specufied in the <see cref="DimensionStyle"/> object
        /// associated with the dimension object possibly overriden by the dimension object's
        /// extended data.</param>
        /// <param name="defaultPostFix">A string specifying a default prefix and/or postfix
        /// used for both the primary and the alternate value if no prefix/postfix is specified
        /// by the dimension-style properties.</param>
        /// <remarks>
        /// </remarks>
        protected AngularMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
            : base(dimension, dimProps, defaultPostFix, textSize) {
        }


        /// <summary>
        /// Creates a new instance of an angular measurement formatter as specified by the
        /// <paramref name="format"/> parameter. 
        /// </summary>
        /// <param name="dimension">The<see cref="Dimension"/> object the dimension text is to be created for.</param>
        /// <param name="dimProps">A <see cref="DimensionProperties"/> object providing all
        /// dimension-style properties specufied in the <see cref="DimensionStyle"/> object
        /// associated with the dimension object possibly overriden by the dimension object's
        /// extended data.</param>
        /// <returns>
        /// An angular measurement formatter as specified by the <paramref name="format"/>
        /// parameter. The returned formatter can be a <see cref="DecimalDegreesMeasurementFormatter"/>,
        /// <see cref="RadiansMeasurementFormatter"/>, <see cref="GradiansMeasurementFormatter"/>,
        /// <see cref="SurveyorUnitsMeasurementFormatter"/>, or <see cref="DegreesMinutesSecondsMeasurementFormatter"/>.
        /// </returns>
        public static AngularMeasurementFormatter CreateMeasurementFormatter(
            AngularUnitFormat format,
            Dimension dimension,
            DimensionProperties dimProps,
            double textSize) {

            switch (format) {
            default:
            case AngularUnitFormat.DecimalDegrees:
                return new DecimalDegreesMeasurementFormatter(dimension, dimProps, "<>°", textSize);

            case AngularUnitFormat.Radians:
                return new RadiansMeasurementFormatter(dimension, dimProps, "<>r", textSize);

            case AngularUnitFormat.Gradians:
                return new GradiansMeasurementFormatter(dimension, dimProps, "<>g", textSize);

            case AngularUnitFormat.SurveyorsUnits:
                return new SurveyorUnitsMeasurementFormatter(dimension, dimProps, "<>", textSize);

            case AngularUnitFormat.DegreesMinutesSeconds:
                return new DegreesMinutesSecondsMeasurementFormatter(dimension, dimProps, "<>", textSize);
            }
        }


        /// <summary>
        /// When implemented in a derived class scales the angle value in radians from AutoCAD
        /// to the specific angular unit.
        /// </summary>
        /// <param name="value">The angle value in radians as specified by AutoCAD.</param>
        /// <returns>The scaled value.</returns>
        protected abstract double GetDisplayValue(double value);


        /// <inheritdoc/>
        /// <remarks>
        /// The measurement value
        /// <include file='_comments.xml' path='docTokens/docToken[@name="angularAcaledTo"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="angularFormattedAs"]'/>
        /// <see cref="DimensionProperties.AngularDimensionDecimalPlaces"/> and
        /// <see cref="DimensionProperties.AngularZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="primaryPostFix"]'/>.
        /// </remarks>
        public override string FormatMeasurement() {
            double measurement = GetDisplayValue(_dimension.Measurement);
            string degsText = FormatValue(measurement, _dimProps.AngularDimensionDecimalPlaces, _dimProps.AngularZeroHandling);
            return GetTextWithPostFix(degsText);
        }


        /// <inheritdoc/>
        /// <remarks>
        /// The <see cref="DimensionProperties.PlusTolerance"/> value
        /// <include file='_comments.xml' path='docTokens/docToken[@name="angularScaledTo"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="angularFormattedAs"]'/>
        /// <see cref="DimensionProperties.ToleranceDecimalPlaces"/> and
        /// <see cref="DimensionProperties.ToleranceZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="primaryPostFix"]'/>.
        /// </remarks>
        public override string FormatMeasurementToleranceSymmetric() {
            double tolerance = GetDisplayValue(_dimProps.PlusTolerance);
            string degsText = FormatValue(tolerance, _dimProps.ToleranceDecimalPlaces, _dimProps.ToleranceZeroHandling);
            return "±" + GetTextWithPostFix(degsText);
        }


        /// <inheritdoc/>
        /// <remarks>
        /// <see cref="DimensionProperties.PlusTolerance" />, <see cref="DimensionProperties.MinusTolerance" /> values 
        /// <include file='_comments.xml' path='docTokens/docToken[@name="angularScaledToPlural"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="angularFormattedAs"]'/>
        /// <see cref="DimensionProperties.ToleranceDecimalPlaces"/> and
        /// <see cref="DimensionProperties.ToleranceZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="primaryPostFix"]'/>
        /// to both.
        /// </remarks>
        public override string FormatMeasurementTolerancePlusMinus() {
            double minusTolerance = GetDisplayValue(_dimProps.MinusTolerance);
            double plusTolerance = GetDisplayValue(_dimProps.PlusTolerance);

            short toleranceDecimalPlaces = _dimProps.ToleranceDecimalPlaces;
            ZeroHandling toleranceZeroHandling = _dimProps.ToleranceZeroHandling;
            string minusToleranceFormatted = FormatValue(minusTolerance, toleranceDecimalPlaces, toleranceZeroHandling);
            string plusToleranceFormatted = FormatValue(plusTolerance, toleranceDecimalPlaces, toleranceZeroHandling);

            return FormatToleranceStacked(
                "-" + GetTextWithPostFix(minusToleranceFormatted),
                "+" + GetTextWithPostFix(plusToleranceFormatted));
        }


        /// <inheritdoc/>
        /// <remarks>
        /// The limits
        /// (Measurement + <see cref="DimensionProperties.PlusTolerance" /> and
        /// Measumreent - <see cref="DimensionProperties.MinusTolerance" />)
        /// <include file='_comments.xml' path='docTokens/docToken[@name="angularScaledToPlural"]'/>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="angularFormattedAs"]'/>
        /// <see cref="DimensionProperties.ToleranceDecimalPlaces"/> and
        /// <see cref="DimensionProperties.ToleranceZeroHandling"/>.
        /// <include file='_comments.xml' path='docTokens/docToken[@name="primaryPostFix"]'/>
        /// to both.
        /// </remarks>
        public override string FormatMeasurementLimits() {
            double minValue = GetDisplayValue(_dimension.Measurement - _dimProps.MinusTolerance);
            double maxValue = GetDisplayValue(_dimension.Measurement + _dimProps.PlusTolerance);

            short decimalPlaces = _dimProps.AngularDimensionDecimalPlaces;
            ZeroHandling zeroHandling = _dimProps.ZeroHandling;
            string minLimitFormatted = FormatValue(minValue, decimalPlaces, zeroHandling);
            string maxLimitFormatted = FormatValue(maxValue, decimalPlaces, zeroHandling);

            return FormatToleranceStacked(
                GetTextWithPostFix(minLimitFormatted),
                GetTextWithPostFix(maxLimitFormatted));
        }
    }
}