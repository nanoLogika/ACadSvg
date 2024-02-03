#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Diagnostics.Metrics;
using System.Globalization;

using ACadSharp.Entities;
using ACadSharp.Tables;

using ACadSvg.Extensions;

using SvgElements;

namespace ACadSvg.DimensionTextFormatter {

    /// <summary>
    /// Represents a formatter for angular dimensions. The angle value is formatted as
    /// decimal degrees.
    /// </summary>
    internal class DecimalDegreesMeasurementFormatter : AngularMeasurementFormatter {

        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalDegreesMeasurementFormatter"/>.
        /// </summary>
        /// <inheritdoc />
        public DecimalDegreesMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
            : base(dimension, dimProps, defaultPostFix, textSize) {
        }


        /// <summary>
        /// Scales the specified angle value from AutoCAD to degrees.
        /// </summary>
        /// <param name="value">The angle value in radians as specified by AutoCAD.</param>
        /// <returns>The angle in degrees.</returns>
        protected override double GetDisplayValue(double value) {
            return value * 180 / Math.PI;
        }


        /// <summary>
        /// Formats the passed <paramref name="value"/> as decimal number
        /// <see cref="<see cref="MeasurementFormatterBase.FormatDecimal(double, short, ZeroHandling)"/>.
        /// </summary>
        /// <inheritdoc/>
        /// <returns>A string with the value formatted as decimal expression.</returns>
        protected override string FormatValue(double value, short decimalPlaces, ZeroHandling zeroHandling) {
            return FormatDecimal(value, decimalPlaces, zeroHandling);
        }
    }
}
