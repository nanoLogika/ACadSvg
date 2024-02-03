#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using ACadSharp.Tables;

using ACadSvg.Extensions;


namespace ACadSvg.DimensionTextFormatter {

    /// <summary>
    /// Represents a formatter for linear measurements. The value is formatted as
    /// a decimal number.
    /// </summary>
    internal class DecimalMeasurementFormatter : LinearMeasurementFormatter {


        /// <summary>
        /// Initializes a new instance of a <see cref="DecimalMeasurementFormatter"/>.
        /// </summary>
        /// <inheritdoc/>
        public DecimalMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
            : base(dimension, dimProps, defaultPostFix, textSize) {
        }


        /// <summary>
        /// Formats the specified value as decimal number
        /// (see <see cref="MeasurementFormatterBase.FormatDecimal"/>).
        /// </summary>
        /// <returns>
        /// The formatted value as decimal number.
        /// </returns>
        /// <inheritdoc/>
        protected override string FormatValue(double value, short decimalPlaces, ZeroHandling zeroHandling) {
            return FormatDecimal(value, decimalPlaces, zeroHandling);
        }
    }
}
