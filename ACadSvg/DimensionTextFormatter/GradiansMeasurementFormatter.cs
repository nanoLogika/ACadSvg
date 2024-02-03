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
    /// decimal gradians (The full circle is assumed to be 400g).
    /// </summary>
    internal class GradiansMeasurementFormatter : DecimalDegreesMeasurementFormatter {

        /// <summary>
        /// Initializes a new instance of the <see cref="GradiansMeasurementFormatter"/>.
        /// </summary>
        /// <inheritdoc/>
        public GradiansMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
            : base(dimension, dimProps, defaultPostFix, textSize) {
        }


        /// <summary>
        /// Scales the specified angle value from AutoCAD to gradians.
        /// </summary>
        /// <param name="value">The angle value in radians as specified by AutoCAD.</param>
        /// <returns>The angle in gradians.</returns>
        protected override double GetDisplayValue(double value) {
            return value * 200 / Math.PI;
        }
    }
}