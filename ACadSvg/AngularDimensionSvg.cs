#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.IO;
using System.Net;

using ACadSharp.Entities;
using ACadSharp.Types.Units;

using ACadSvg.DimensionTextFormatter;

using CSMath;

using SvgElements;

using static ACadSharp.Entities.Hatch.BoundaryPath;


namespace ACadSvg {

    /// <summary>
    /// Base class for classes representing SVG elements converted from AutoCAD angular-dimension
    /// entities. Methods of this class provide basic methods to create the arc dimension line and
    /// the <i>text</i> element for the mesurement text.
    /// </summary>
    internal abstract class AngularDimensionSvg : DimensionSvg {

        /// <summary>
        /// Initializes a new instance of this <see cref="AngularDimensionSvg"/>.
        /// </summary>
        /// <param name="entity">The angular-dimension to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public AngularDimensionSvg(Entity entity, ConversionContext ctx)
            : base(entity, ctx) {
        }


        /// <summary>
        /// Creates a <i>path</i> SVG element containing an arc and adds it to the <i>group</i>
        /// element of this <see cref="AngularDimensionSvg"/>.
        /// </summary>
        /// <param name="center">The center of the arc.</param>
        /// <param name="r">The radus of the arc.</param>
        /// <param name="startAngle">The angle of the begin of the arc.</param>
        /// <param name="endAngle">The angle of the end of the arc.</param>
        protected void CreateDimensionLineArc(XY center, double r, double startAngle, double endAngle) {

            if (startAngle < 0) {
                startAngle += 2 * Math.PI;
            }
            if (endAngle < 0) {
                endAngle += 2 * Math.PI;
            }
            bool flipped = endAngle < startAngle;

            PathElement path = new PathElement()
                .WithFill("none")
                .WithStroke(_dimensionLineColor)
                .WithFill("none")
                .WithStrokeWidth(_dimensionLineWidth);

            Utils.ArcToPath(path, true, center, r, flipped ? endAngle : startAngle, flipped ? startAngle : endAngle);

            _groupElement.Children.Add(path);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="firstAngle"></param>
        /// <param name="secondAngle"></param>
        /// <param name="firstArrowOutside"></param>
        /// <param name="secondArrowOutside"></param>
        /// <param name="alpha"></param>
        /// <param name="firstArrowDirection"></param>
        /// <param name="secondArrowDirection"></param>
        protected void GetArrorwsDirection(double r, double firstAngle, double secondAngle, bool firstArrowOutside, bool secondArrowOutside, out double alpha, out XY firstArrowDirection, out XY secondArrowDirection) {
            //  Sehne - kreis - winkel: s = 2 r sin(a/2)
            //  sin(a/2) = s / 2 / r
            if (firstAngle < 0) {
                firstAngle += 2 * Math.PI;
            }
            if (secondAngle < 0) {
                secondAngle += 2 * Math.PI;
            }
            double f = (secondAngle > firstAngle) ? 1 : -1;  
            alpha = Math.Asin(_arrowSize / 2 / r);
            double firstAngleAlpha = firstAngle + alpha * (firstArrowOutside ? -1 : 1) * f;
            double secondAngleAlpha = secondAngle - alpha * (secondArrowOutside ? -1 : 1) * f;
            firstArrowDirection = new XY(Math.Sin(firstAngleAlpha), -Math.Cos(firstAngleAlpha)) * (firstArrowOutside ? -1 : 1) * f;
            secondArrowDirection = new XY(-Math.Sin(secondAngleAlpha), Math.Cos(secondAngleAlpha)) * (secondArrowOutside ? -1 : 1) * f;
        }


        /// <summary>
        /// Creates the measurement text from the measurement value as specified by the properties
        /// of the associated <see cref="DimensionStyle"/> object or property values overriden
        /// by values from the <see cref="Dimension"/> object's <see cref="CadObject.ExtendedData"/>.
        /// This implementation supports <see cref="DimensionAngular3Pt">angular dimensions</see>
        /// with primary measurement with the specified respective <see cref="AngularUnitFormat"/>
        /// with or without tolerance. Angular dimenions cannot have alternate-unit dimensioning.
        /// <param name="textSize"></param>
        /// <remarks><para>
        /// Formatting complient with the specified <see cref="AngularUnitFormat"/> is provided
        /// by a class derived from <see cref="LinearMeasurementFormatter"/>.
        /// </para><para>
        /// The created measuement text is coded using AutoCAD "markup" as used for MTEXT entities.
        /// </para></remarks>
        /// <returns>
        /// The created measuerement text.
        /// </returns>
        /// 
        protected override string CreateMeasurementText(double textSize) {
            AngularMeasurementFormatter amFt = AngularMeasurementFormatter.CreateMeasurementFormatter(
                _dimProps.AngularUnit, _dimension, _dimProps, textSize);

            switch (GetToleranceOption()) {
            default:
            case 0: //  Just value, no tolerance
                return amFt.FormatMeasurement();

            case 1:
                //  Symmetric: ±{PlusTolerance}
                //  -  GenerateTolerance = true
                //  -  MinusTolerance == PlusTolerance" (Exactly equal)
                return $"{amFt.FormatMeasurement()}{amFt.FormatMeasurementToleranceSymmetric()}";

            case 2:
                //  Deviation: +{PlusTolerance}^-{MinusTolerance}, aligned
                //  -  GenerateTolerance = true
                //  -  MinusTolerance != PlusTolerance" (Difference may be below precision if, e.g., +5^-5
                string alignment = getVerticalAlingnment();
                return $"{alignment}{amFt.FormatMeasurement()}{amFt.FormatMeasurementTolerancePlusMinus()}";


            case 3:
                //  Limits/oben:  +{Measurement+PlusTolerance}^-{Measurement-MinusTolerance}
                //  -  LimitsGeneration = true
                string alignmentl = getVerticalAlingnment();
                return  $"{alignmentl}{amFt.FormatMeasurementLimits()}";
            }
        }
    }
}