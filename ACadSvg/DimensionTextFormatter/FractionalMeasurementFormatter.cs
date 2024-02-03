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
    /// as integer part and fraction.
    /// </summary>
    internal class FractionalMeasurementFormatter : LinearMeasurementFormatter {

        /// <summary>
        /// Initializes a new instance of a <see cref="FractionalMeasurementFormatter"/>.
        /// </summary>
        /// <inheritdoc/>
        public FractionalMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
            : base(dimension, dimProps, defaultPostFix, textSize) {
        }


        /// <summary>
        /// Formats the value as integer part and fraction.
        /// (see <see cref="FormatFractional"/>).
        /// </summary>
        /// <include file='_comments.xml' path='docTokens/docToken[@name="decimalPlacesParameterFraction"]/*'/>
        /// <returns>
        /// The formatted value with integer part and fraction.
        /// </returns>
        /// <inheritdoc/>
        protected override string FormatValue(double value, short decimalPlaces, ZeroHandling zeroHandling) {
            return FormatFractional(value, decimalPlaces, zeroHandling);
        }
    }
}
