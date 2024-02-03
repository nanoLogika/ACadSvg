#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Diagnostics.Metrics;

using ACadSharp.Entities;
using ACadSharp.Tables;

using ACadSvg.Extensions;


namespace ACadSvg.DimensionTextFormatter {

	/// <summary>
	/// Represents an architectural-measurement formatter for linear dimensions. The measuement
	/// value specified in inches is to be formatted as feets, integer-inches value, and a
	/// fraction.
	/// </summary>
    internal class ArchitecturalMeasurementFormatter : LinearMeasurementFormatter {

        /// <summary>
        /// Initializes a new instance of the <see cref="ArchitecturalMeasurementFormatter"/>.
        /// </summary>
        /// <inheritdoc />
        public ArchitecturalMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
            : base(dimension, dimProps, defaultPostFix, textSize) {
        }


        /// <summary>
        /// Formats the specified value (in inches) as feets and inches, and the fractional
        /// part of the inches as fraction
        /// (see <see cref="MeasurementFormatterBase.FormatInchesToFeetInchesFractional"/>.
        /// </summary>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="decimalPlacesParameterFraction"]/*'/>
        /// <returns>
        /// The formatted value with feet, integer inches, and fraction.
        /// </returns>
        /// <inheritdoc />
        protected override string FormatValue(double value, short decimalPlaces, ZeroHandling zeroHandling) {
            return FormatInchesToFeetInchesFractional(value, decimalPlaces, zeroHandling);
        }
    }
}
