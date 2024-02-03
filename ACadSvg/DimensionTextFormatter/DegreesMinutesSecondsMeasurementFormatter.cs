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
    /// Represents a formatter for angular dimensions. The angle value is formatted as
    /// degrees, minutes and seconds.
    /// </summary>
    internal class DegreesMinutesSecondsMeasurementFormatter : AngularMeasurementFormatter {

        /// <summary>
        /// Initializes a new instance of the <see cref="DegreesMinutesSecondsMeasurementFormatter"/>.
        /// </summary>
        /// <inheritdoc />
        public DegreesMinutesSecondsMeasurementFormatter(Dimension dimension, DimensionProperties dimProps, string defaultPostFix, double textSize)
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
        /// Formats an angular value specified in degrees as degrees, minutes an seconds.
        /// </summary>
        /// <param name="value">An angular value in degrees.</param>
        /// <param name="decimalPlaces">controls whether minutes and seconds are displayed
        /// at all an how many fractional digits are dieplayed at the seconds part:
        /// <para>
        /// 0: Only the degrees part is visible.
        /// </para>
        /// <para>
        /// 1, 2: degrees and rounded minutes.
        /// </para>
        /// <para>
        /// 3, 4: degrees, minutes, and rounded seconds.
        /// </para>
        /// <para>
        /// >4: degrees, minutes, and seconds with (decimalPlaces - 4) fractional digits.
        /// </para>
        /// </param>
        /// <returns>
        /// The formatted value with feet degrees, munutes and seconds.
        /// </returns>
        /// <inheritdoc/>
        protected override string FormatValue(double value, short decimalPlaces, ZeroHandling zeroHandling) {
            switch (decimalPlaces) {
            case 0:
                //  No minutes, seconds, degrees rounded to integer
                return $"{FormatDecimal(value, 0, zeroHandling)}°";
            case 1:
            case 2:
                //  No seconds, minutes rounded to integer
                {
                double intDegrees = Math.Floor(value);
                    double minutes = (value - intDegrees) * 60.0;
                    string strMinutes = FormatDecimal(minutes, 0, zeroHandling);
                    return $"{intDegrees}°{strMinutes}'";
                }
            default:
                //  decimalPlaces = 3, 4: degrees, minutes, seconds rounded to integer 
                //  decimalPlaces > 4: degrees, minutes, seconds rounded with decimalPLaces - 4 fractional digits. 
                {
                    double intDegrees = Math.Floor(value);
                    double minutes = (value - intDegrees) * 60.0;
                    double intMinutes = Math.Floor(minutes);
                    double seconds = (minutes - intMinutes) * 60.0;
                    short secDecimalPlaces = decimalPlaces == 3 ? (short)0 : (short)(decimalPlaces - 4);
                    string strSeconds = FormatDecimal(seconds, secDecimalPlaces, zeroHandling);

                    return $"{intDegrees}°{intMinutes}'{strSeconds}\"";
                }
            }
        }
    }
}