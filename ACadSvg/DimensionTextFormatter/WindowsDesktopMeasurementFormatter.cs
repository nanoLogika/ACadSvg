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
    /// Represents a formatter for linear measurements.
    /// </summary>
    /// <remarks>
    /// <b>This formatter is not yet implemented.</b>
    /// </remarks>
    internal class WindowsDesktopMeasurementFormatter : LinearMeasurementFormatter {

        /// <summary>
        /// Initializes a new instance of a <see cref="WindowsDesktopMeasurementFormatter"/>.
        /// </summary>
        /// <inheritdoc/>
        public WindowsDesktopMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
            : base(dimension, dimProps, defaultPostFix, textSize) {
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
