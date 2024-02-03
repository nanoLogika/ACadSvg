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
    /// Represents a formatter for angular dimensions. The angle value is formatted in
    /// surveyors units.
    /// </summary>
    /// <remarks>
    /// <b>This formatter is not yet implemented.</b>
    /// </remarks>
    internal class SurveyorUnitsMeasurementFormatter : AngularMeasurementFormatter {

        /// <summary>
        /// Initializes a new instance of a <see cref="SurveyorUnitsMeasurementFormatter"/>.
        /// </summary>
        /// <inheritdoc/>
        public SurveyorUnitsMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
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
        /// This method is not yet implemented.
        /// </summary>
        /// <inheritdoc/>
        /// <exception cref="NotImplementedException"></exception>
        protected override string FormatValue(double value, short decimalplaces, ZeroHandling zeroHandling) {
            throw new NotImplementedException();
        }
    }
}