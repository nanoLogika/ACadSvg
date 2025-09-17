﻿#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using ACadSharp.Types.Units;

namespace ACadSvg.Extensions {

    /// <summary>
    /// Exposes all proprties required for the rendering of <see cref="Dimension"/> and
    /// derived entity's and <see cref="Leader"/> entity's. Property values are either
    /// provided by the associated <see cref="DimensionStyle"/> object or may be overriden
    /// by extended data of the entity. The <see cref="DimensionStyle.ScaleFactor"/> is
    /// handled internally.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="DimensionStyle.ScaleFactor"/> property is retrieved from the
    /// <see cref="DimensionStyle"/> and my be overridden by extended data of the entity.
    /// The value is used to scale the property values that have to be scaled.
    /// </para><para>
    /// The <see cref="DimensionStyle"/> object provides a set of properties controlling
    /// the dimension text. 
    /// </para>
    /// </remarks>
    internal class DimensionProperties : EntityProperties {

        private Entity _entity;
        private DimensionStyle _dimStyle;
        private double _scaleFactor;


        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionProperties"/> class.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="style"></param>
        public DimensionProperties(Entity entity, DimensionStyle style) {
            _entity = entity;
            _dimStyle = style;

            _scaleFactor = getScaleFactor();
        }


        private double getScaleFactor() {
            double scaleFactor = GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 40, _dimStyle.ScaleFactor);
            if (scaleFactor == 0) {
                scaleFactor = _entity.Document.Header.DimensionScaleFactor * 10;
            }
            return scaleFactor;
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ScaleFactor"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public double ScaleFactor {
            get { return _scaleFactor; }
        }


        #region -  Line properties

        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.DimensionLineColor"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public Color DimensionLineColor {
                get {
                    return GetExtendedDataColor(_entity, "ACAD", "DSTYLE", 176, _dimStyle.DimensionLineColor);
                }
            }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ExtensionLineColor"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public Color ExtensionLineColor {
            get {
                return GetExtendedDataColor(_entity, "ACAD", "DSTYLE", 177, _dimStyle.ExtensionLineColor);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.DimensionLineWeight"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public LineWeightType DimensionLineWeight {
            get {
                return GetExtendedDataValue<LineWeightType>(_entity, "ACAD", "DSTYLE", 371, _dimStyle.DimensionLineWeight);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ExtensionLineWeight"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public LineWeightType ExtensionLineWeight {
            get {
                return GetExtendedDataValue<LineWeightType>(_entity, "ACAD", "DSTYLE", 372, _dimStyle.ExtensionLineWeight);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ExtensionLineExtension"/> property or the
        /// overriden value from the entity's extended data. The value returned is already
        /// multiplied with the <see cref="ScaleFactor"/> value.
        /// </summary>
        public double ExtensionLineExtension {
            get {
                double extensionLineExtension = GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 44, _dimStyle.ExtensionLineExtension);
                return extensionLineExtension * _scaleFactor;
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ExtensionLineOffset"/> property or the
        /// overriden value from the entity's extended data. The value returned is already
        /// multiplied with the <see cref="ScaleFactor"/> value.
        /// </summary>
        public double ExtensionLineOffset {
            get {
                double extensionLineOffset = GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 42, _dimStyle.ExtensionLineOffset);
                return extensionLineOffset * _scaleFactor;
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.DimensionLineExtension"/> property or the
        /// overriden value from the entity's extended data. The value returned is already
        /// multiplied with the <see cref="ScaleFactor"/> value.
        /// </summary>
        public double DimensionLineExtension {
            get {
                double dimensionLineGap = GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 46, _dimStyle.DimensionLineExtension);
                return dimensionLineGap * _scaleFactor;
            }
        }

        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.DimensionLineGap"/> property or the
        /// overriden value from the entity's extended data. The value returned is already
        /// multiplied with the <see cref="ScaleFactor"/> value.
        /// </summary>
        public double DimensionLineGap {
            get {
                double dimensionLineGap = GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 147, _dimStyle.DimensionLineGap);
                return dimensionLineGap * _scaleFactor;
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.SuppressFirstDimensionLine"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public bool SuppressFirstDimensionLine {
            get {
                return GetExtendedDataValue<bool>(_entity, "ACAD", "DSTYLE", 281, _dimStyle.SuppressFirstDimensionLine);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.SuppressSecondDimensionLine"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public bool SuppressSecondDimensionLine {
            get {
                return GetExtendedDataValue<bool>(_entity, "ACAD", "DSTYLE", 282, _dimStyle.SuppressSecondDimensionLine);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.SuppressFirstExtensionLine"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public bool SuppressFirstExtensionLine {
            get {
                return GetExtendedDataValue<bool>(_entity, "ACAD", "DSTYLE", 75, _dimStyle.SuppressFirstExtensionLine);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.SuppressSecondExtensionLine"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public bool SuppressSecondExtensionLine {
            get {
                return GetExtendedDataValue<bool>(_entity, "ACAD", "DSTYLE", 76, _dimStyle.SuppressSecondExtensionLine);
            }
        }

        #endregion
        #region -  Arrow properties

        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.LeaderArrow"/> property or the
        /// overriden value from the entity's extended data. Note, that the entities of the
        /// <see cref="BlockRecord"/> have to be scaled by the <see cref="ArrowSize"/>
        /// properties that includes the <see cref="ScaleFactor"/>.
        /// </summary>
        public BlockRecord LeaderArrow {
            get {
                return GetExtendedDataBlockReference(_entity, "ACAD", "DSTYLE", 341, _dimStyle.LeaderArrow);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ArrowBlock"/> property or the
        /// overriden value from the entity's extended data. Note, that the entities of the
        /// <see cref="BlockRecord"/> have to be scaled by the <see cref="ArrowSize"/>
        /// properties that includes the <see cref="ScaleFactor"/>.
        /// 
        /// </summary>
        public BlockRecord ArrowBlock {
            get {
                return GetExtendedDataBlockReference(_entity, "ACAD", "DSTYLE", 342, _dimStyle.ArrowBlock);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ArrowHeadBlock1"/> property or the
        /// overriden value from the entity's extended data. Note, that the entities of the
        /// <see cref="BlockRecord"/> have to be scaled by the <see cref="ArrowSize"/>
        /// properties that includes the <see cref="ScaleFactor"/>.
        /// </summary>
        public BlockRecord ArrowHeadBlock1 {
            get {
                return GetExtendedDataBlockReference(_entity, "ACAD", "DSTYLE", 343, _dimStyle.DimArrow1);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ArrowHeadBlock2"/> property or the
        /// overriden value from the entity's extended data. Note, that the entities of the
        /// <see cref="BlockRecord"/> have to be scaled by the <see cref="ArrowSize"/>
        /// properties that includes the <see cref="ScaleFactor"/>.
        /// </summary>
        public BlockRecord ArrowHeadBlock2 {
            get {
                return GetExtendedDataBlockReference(_entity, "ACAD", "DSTYLE", 344, _dimStyle.DimArrow2);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ArrowSize"/> property or the
        /// overriden value from the entity's extended data. The value returned is already
        /// multiplied with the <see cref="ScaleFactor"/> value.
        /// </summary>
		public double ArrowSize {
            get {
                double arrowSize = GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 41, _dimStyle.ArrowSize);
                if (arrowSize == 0.18) {
                    arrowSize = _entity.Document.Header.DimensionArrowSize;
                }
                return arrowSize * _scaleFactor;
            }
        }

        #endregion
        #region -  Text positioning and sizing

        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.Style"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public TextStyle TextStyle {
            get { return _dimStyle.Style; }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.TextColor"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public Color TextColor {
            get {
                return GetExtendedDataColor(_entity, "ACAD", "DSTYLE", 178, _dimStyle.TextColor);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.TextHeight"/> property or the
        /// overriden value from the entity's extended data. The value returned is already
        /// multiplied with the <see cref="ScaleFactor"/> value.
        /// </summary>
        public double TextHeight {
            get {
                double textHeight = GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 140, _dimStyle.TextHeight);
                return textHeight * _scaleFactor;
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.TextMovement"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public TextMovement TextMovement {
            get {
                return GetExtendedDataValue<TextMovement>(_entity, "ACAD", "DSTYLE", 279, _dimStyle.TextMovement);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.TextHorizontalAlignment"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public DimensionTextHorizontalAlignment TextHorizontalAlignment {
            get {
                return GetExtendedDataValue<DimensionTextHorizontalAlignment>(_entity, "ACAD", "DSTYLE", 280, _dimStyle.TextHorizontalAlignment);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.TextVerticalAlignment"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public DimensionTextVerticalAlignment TextVerticalAlignment {
            get {
                return GetExtendedDataValue<DimensionTextVerticalAlignment>(_entity, "ACAD", "DSTYLE", 77, _dimStyle.TextVerticalAlignment);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.TextVerticalPosition"/> property or the
        /// overriden value from the entity's extended data. The value returned is already
        /// multiplied with the <see cref="ScaleFactor"/> value.
        /// </summary>
        public double TextVerticalPosition {
            get {
                double textVerticalPosition = GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 154, _dimStyle.TextVerticalPosition);
                return textVerticalPosition * _scaleFactor;
            }
        }

        #endregion
        #region -  Text formatting

        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.LinearScaleFactor"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public double LinearScaleFactor {
            get {
                return GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 144, _dimStyle.LinearScaleFactor);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.DecimalSeparator"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public char DecimalSeparator {
            get {
                return GetExtendedDataValue<char>(_entity, "ACAD", "DSTYLE", 278, _dimStyle.DecimalSeparator);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.DecimalPlaces"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public short DecimalPlaces {
            get {
                return GetExtendedDataValue<short>(_entity, "ACAD", "DSTYLE", 271, _dimStyle.DecimalPlaces);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.LinearUnitFormat"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public LinearUnitFormat LinearUnitFormat {
            get {
                return GetExtendedDataValue<LinearUnitFormat>(_entity, "ACAD", "DSTYLE", 277, _dimStyle.LinearUnitFormat);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.Rounding"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public double Rounding {
            get {
                return GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 45, _dimStyle.Rounding);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ZeroHandling"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public ZeroHandling ZeroHandling {
            get {
                return GetExtendedDataValue<ZeroHandling>(_entity, "ACAD", "DSTYLE", 78, _dimStyle.ZeroHandling);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.PostFix"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public string PostFix {
            get {
                return GetExtendedDataValue<string>(_entity, "ACAD", "DSTYLE", 3, _dimStyle.PostFix);
            }
        }

        #endregion
        #region -  Tolerances

        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.GenerateTolerances"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public bool GenerateTolerances {
            get {
                return GetExtendedDataValue<bool>(_entity, "ACAD", "DSTYLE", 71, _dimStyle.GenerateTolerances);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.LimitsGeneration"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public bool LimitsGeneration {
            get {
                return GetExtendedDataValue<bool>(_entity, "ACAD", "DSTYLE", 72, _dimStyle.LimitsGeneration);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ToleranceAlignment"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public ToleranceAlignment ToleranceAlignment {
            get {
                return GetExtendedDataValue<ToleranceAlignment>(_entity, "ACAD", "DSTYLE", 283, _dimStyle.ToleranceAlignment);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ToleranceZeroHandling"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public ZeroHandling ToleranceZeroHandling {
            get {
                return GetExtendedDataValue<ZeroHandling>(_entity, "ACAD", "DSTYLE", 286, _dimStyle.ToleranceZeroHandling);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ToleranceDecimalPlaces"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public short ToleranceDecimalPlaces {
            get {
                return GetExtendedDataValue<short>(_entity, "ACAD", "DSTYLE", 272, _dimStyle.ToleranceDecimalPlaces);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.PlusTolerance"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public double PlusTolerance {
            get {
                return GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 47, _dimStyle.PlusTolerance);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.MinusTolerance"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public double MinusTolerance {
            get {
                return GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 48, _dimStyle.MinusTolerance);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.ToleranceScaleFactor"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public double ToleranceScaleFactor {
            get {
                return GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 146, _dimStyle.ToleranceScaleFactor);
            }
        }

        #endregion
        #region -  Alternate Dimension Text formatting

        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.AlternateUnitDimensioning"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public bool AlternateUnitDimensioning {
            get {
                return GetExtendedDataValue<bool>(_entity, "ACAD", "DSTYLE", 170, _dimStyle.AlternateUnitDimensioning);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.AlternateUnitScaleFactor"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public double AlternateUnitScaleFactor {
            get {
                return GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 143, _dimStyle.AlternateUnitScaleFactor);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.AlternateDimensioningSuffix"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public string AlternateDimensioningSuffix {
            get {
                return GetExtendedDataValue<string>(_entity, "ACAD", "DSTYLE", 4, _dimStyle.AlternateDimensioningSuffix);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.AlternateUnitDecimalPlaces"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public short AlternateUnitDecimalPlaces {
            get {
                return GetExtendedDataValue<short>(_entity, "ACAD", "DSTYLE", 171, _dimStyle.AlternateUnitDecimalPlaces);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.AlternateUnitFormat"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public LinearUnitFormat AlternateUnitFormat {
            get {
                return GetExtendedDataValue<LinearUnitFormat>(_entity, "ACAD", "DSTYLE", 273, _dimStyle.AlternateUnitFormat);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.AlternateUnitRounding"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public double AlternateUnitRounding {
            get {
                return GetExtendedDataValue<double>(_entity, "ACAD", "DSTYLE", 148, _dimStyle.AlternateUnitRounding);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.AlternateUnitZeroHandling"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public ZeroHandling AlternateUnitZeroHandling {
            get {
                return GetExtendedDataValue<ZeroHandling>(_entity, "ACAD", "DSTYLE", 285, _dimStyle.AlternateUnitZeroHandling);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.AlternateUnitToleranceDecimalPlaces"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public short AlternateUnitToleranceDecimalPlaces {
            get {
                return GetExtendedDataValue<short>(_entity, "ACAD", "DSTYLE", 274, _dimStyle.AlternateUnitToleranceDecimalPlaces);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.AlternateUnitToleranceZeroHandling"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public ZeroHandling AlternateUnitToleranceZeroHandling {
            get {
                return GetExtendedDataValue<ZeroHandling>(_entity, "ACAD", "DSTYLE", 286, _dimStyle.AlternateUnitToleranceZeroHandling);
            }
        }

        #endregion
        #region -  Angular text formatting

        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.AngularDimensionDecimalPlaces"/> property or the
        /// overriden value from the entity's extended data.
        /// </summary>
        public AngularUnitFormat AngularUnit {
            get {
                return GetExtendedDataValue<AngularUnitFormat>(_entity, "ACAD", "DSTYLE", 275, _dimStyle.AngularUnit);
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="DimensionStyle.AngularDimensionDecimalPlaces"/> property or the
        /// overriden value from the entity's extended data. If the value is less than zero the value of the
        /// <see cref="DecimalPlaces"/> property is returned.
        /// </summary>
        public short AngularDimensionDecimalPlaces {
            get {
                short angularDimensionDecimalPlaces = GetExtendedDataValue<short>(_entity, "ACAD", "DSTYLE", 179, _dimStyle.AngularDecimalPlaces);
                if (angularDimensionDecimalPlaces >= 0) {
                    return angularDimensionDecimalPlaces;
                }
                return DecimalPlaces;
            }
        }


        public ZeroHandling AngularZeroHandling {
            get {
                return GetExtendedDataValue<ZeroHandling>(_entity, "ACAD", "DSTYLE", 79, _dimStyle.AngularZeroHandling);
            }
        }

        #endregion
    }
}
