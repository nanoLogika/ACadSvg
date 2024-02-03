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


namespace ACadSvg.DimensionTextFormatter {

    /// <summary>
    /// Represents a formatter for linear measurements. The value is formatted as
    /// a decimal number as exponential expression.
    /// </summary>
    internal class ScientificMeasurementFormatter : LinearMeasurementFormatter {

        /// <summary>
        /// Initializes a new instance of the <see cref="ScientificMeasurementFormatter"/>.
        /// </summary>
        /// <inheritdoc />
        public ScientificMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
            : base(dimension, dimProps, defaultPostFix, textSize) {
        }


        /// <summary>
        /// Formats the value as a decimal number as exponential expression
        /// (see <see cref="MeasurementFormatterBase.FormatExponential(double, short, ZeroHandling)"/>.
        /// </summary>
        /// <inheritdoc />
        /// <returns>
        /// The formatted value as decimal number as exponetial expression.
        /// </returns>
        protected override string FormatValue(double value, short decimalPlaces, ZeroHandling zeroHandling) {
            return FormatExponential(value, decimalPlaces, zeroHandling);
        }
    }
}
