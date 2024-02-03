#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;

using ACadSvg.Extensions;


namespace ACadSvg.DimensionTextFormatter {

    /// <summary>
    /// Represents a formatter for angular dimensions. The angle value is formatted as
    /// decimal radians.
    /// </summary>
    internal class RadiansMeasurementFormatter : DecimalDegreesMeasurementFormatter {

        /// <summary>
        /// Initializes a new instance of the <see cref="RadiansMeasurementFormatter"/>.
        /// </summary>
        /// <inheritdoc/>
        public RadiansMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
            : base(dimension, dimProps, defaultPostFix, textSize) {
        }


        /// <summary>
        /// Returns the specified angle value from AutoCAD in radians without scaling.
        /// </summary>
        /// <param name="value">The angle value in radians as specified by AutoCAD.</param>
        /// <returns>The angle in gradians.</returns>
        protected override double GetDisplayValue(double value) {
            return value;
        }
    }
}