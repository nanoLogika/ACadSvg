#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using ACadSharp.Tables;

using CSMath;

using SvgElements;


namespace ACadSvg {

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="DimensionLinear"/> entity.
    /// The <see cref="DimensionLinear"/> entity is a complex element including several <i>path</i>
    /// elements for the dimension lines, extension lines, filled <i>path</i> elements for the
    /// standard arrowheads, and finally a <i>text</i> element for the mesurement text.
    /// </summary>
    internal class DimensionLinearSvg : DimensionAlignedSvg {

        /// <summary>
        /// Initializes a new instance of the <see cref="DimensionLinearSvg"/> class
        /// for the specified <see cref="DimensionLinear"/> entity.
        /// </summary>
        /// <param name="linDim">The <see cref="DimensionLinear"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public DimensionLinearSvg(Entity linDim, ConversionContext ctx)
            : base(linDim, ctx) {
        }


        protected override void CalculateDirections(
            XY p1, XY p2, XY dp2,
            out XY dp1, out XY dimDir, out XY dext1Dir, out XY dext2Dir) {

            //  TODO Find out what these property is for
            double dimRot = ((DimensionLinear)_aliDim).Rotation;

            double angp1p2 = (p2 - p1).GetAngle();
            double angp1dp2 = (dp2 - p1).GetAngle();
            bool ccw = Math.Sin(angp1dp2 - angp1p2) > 0;
            XYZ n = XYZ.Cross(_aliDim.SecondPoint - _aliDim.FirstPoint, _aliDim.DefinitionPoint - _aliDim.FirstPoint);
            ccw = n.Z > 0;

            dext2Dir = (dp2 - p2).Normalize();
            dimDir = ccw ? new XY(-dext2Dir.Y, dext2Dir.X) : new XY(dext2Dir.Y, -dext2Dir.X);
            dp1 = dp2 + dimDir * _aliDim.Measurement;
            dext1Dir = (dp1 - p1).Normalize();
        }
    }
}