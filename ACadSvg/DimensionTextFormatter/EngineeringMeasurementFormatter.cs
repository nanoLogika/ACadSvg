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
    /// Represents a measurement formatter for linear dimensions. The value in inches is
    /// formatted as feet and inches as decimal number.
    /// </summary>
	internal class EngineeringMeasurementFormatter : LinearMeasurementFormatter {

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineeringMeasurementFormatter"/>
        /// </summary>
        /// <inheritdoc />
        public EngineeringMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
			: base(dimension, dimProps, defaultPostFix, textSize) {
		}


        /// <summary>
        /// Formats the specified value (in inches) as feet and inches as decimal number
        /// (see <see cref="MeasurementFormatterBase.FormatInchesToFeetInchesFractional"/>.
        /// </summary>
        /// <returns>
        /// The formatted value with feet and inches as decimal number.
        /// </returns>
        /// <inheritdoc />
        protected override string FormatValue(double value, short decimalPlaces, ZeroHandling zeroHandling) {
            return FormatInchesToFeetInchesDecimal(value, decimalPlaces, zeroHandling);
        }
	}
}
