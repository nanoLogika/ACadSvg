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

        private static double StandardArrowWidth = 0.1667266;


        public static SvgElementBase CreateArrowHead(BlockRecord arrowHeadBlock, XY arrowPoint, XY arrowDirection, double arrowSize, string arrowColor) {
            if (arrowHeadBlock != null) {
                return CreateArrowheadFromBlock(arrowHeadBlock, arrowPoint, arrowDirection, arrowSize);
            }
            else {
                return CreateStandardArrowHead(arrowPoint, arrowDirection, arrowColor);
            }
        }


        public static SvgElementBase CreateArrowheadFromBlock(BlockRecord arrowHead, XY arrowPoint, XY arrowDirection, double arrowHeadSize) {
			string blockName = Utils.CleanBlockName(arrowHead.Name);

			return
				new UseElement()
				.WithGroupId(blockName)
				.AddTranslate(arrowPoint.X, arrowPoint.Y)
				.AddScale(arrowHeadSize)
				.AddRotate(arrowDirection.GetAngle() * 180 / Math.PI);
		}


        public static SvgElementBase CreateStandardArrowHead(XY arrowPoint, XY arrowDirection, string arrowColor) {
            XY arrowBase = new XY(arrowDirection.Y, -arrowDirection.X) * StandardArrowWidth;
            XY arrowEnd1 = arrowPoint - arrowDirection + arrowBase;
            XY arrowEnd2 = arrowPoint - arrowDirection - arrowBase;

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
