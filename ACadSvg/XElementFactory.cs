#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using SvgElements;
using CSMath;
using ACadSharp.Tables;


namespace ACadSvg {

    public static class XElementFactory {

        public static SvgElementBase CreateArrowhead(BlockRecord arrowHead, XYZ arrowPoint, double arrowHeadSize, XYZ arrowDirection) {
			string blockName = Utils.CleanBlockName(arrowHead.Name);

			return
				new UseElement()
				.WithGroupId(blockName)
				.AddTranslate(arrowPoint.X, arrowPoint.Y)
				.AddScale(arrowHeadSize)
				.AddRotate(Utils.ToXY(arrowDirection).GetAngle() * 180 / Math.PI);
		}


        public static SvgElementBase CreateStandardArrowHead(XYZ arrowPoint, XYZ arrowDirection, string arrowColor) {
            XYZ arrowBase = new XYZ(arrowDirection.Y, -arrowDirection.X, 0) * 0.2;
            XYZ arrowEnd1 = arrowPoint - arrowDirection + arrowBase;
            XYZ arrowEnd2 = arrowPoint - arrowDirection - arrowBase;

            return new PathElement()
                .AddMove(arrowPoint.X, arrowPoint.Y)
                .AddLine(arrowEnd1.X, arrowEnd1.Y)
                .AddLine(arrowEnd2.X, arrowEnd2.Y)
                .Close()
                .WithStroke("none")
                .WithFill(arrowColor);
        }
    }
}
