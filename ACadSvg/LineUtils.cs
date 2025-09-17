#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Header;
using ACadSharp.Tables;

using SvgElements;

using System.Text;

namespace ACadSvg {

    internal static class LineUtils {


        public static double? GetLineWeight(LineWeightType lineWeightType, Entity entity, ConversionContext ctx) {

            switch (lineWeightType) {
            case LineWeightType.ByDIPs:
            case LineWeightType.Default:
                return getLineWeightValue(ctx.ConversionOptions.DefaultLineWeight, entity, ctx);
            case LineWeightType.ByBlock:
                BlockRecord block = entity.Owner as BlockRecord;
                if (block == null) {
                    return getLineWeightValue(ctx.ConversionOptions.DefaultLineWeight, entity, ctx);
                }

                var lineWeightByBlock = block.BlockEntity.LineWeight;
                if (lineWeightByBlock == LineWeightType.ByLayer) {
                    lineWeightByBlock = block.BlockEntity.Layer.LineWeight;
                }
                if (lineWeightByBlock == LineWeightType.Default || lineWeightByBlock == LineWeightType.ByDIPs) {
                    return getLineWeightValue(ctx.ConversionOptions.DefaultLineWeight, entity, ctx);
                }

                return getLineWeightValue(lineWeightByBlock, entity, ctx);
            case LineWeightType.ByLayer:
                var lv = entity.Layer.LineWeight;
                if (lv == LineWeightType.Default) {
                    return getLineWeightValue(ctx.ConversionOptions.DefaultLineWeight, entity, ctx);
                }
                return getLineWeightValue(entity.Layer.LineWeight, entity, ctx);
            default:
                return getLineWeightValue(lineWeightType, entity, ctx);
            }
        }


        // Convert to millimeters/pixels
        private static double? getLineWeightValue(LineWeightType lineweightType, Entity entity, ConversionContext ctx) {
            CadHeader header = entity.Document.Header;
            double scaleFactor = ctx.ConversionOptions.LineweightScaleFactor;
            if (scaleFactor <= 0) {
                scaleFactor = Math.Max(header.ModelSpaceExtMax.X - header.ModelSpaceExtMin.X, header.ModelSpaceExtMax.Y - header.ModelSpaceExtMin.Y) / 2500;
            }
            return (int)lineweightType * 0.01 * scaleFactor;
        }


        public static string LineToDashArray(Entity entity, LineType lineType) {
            List<double> result = new List<double>();

            LineType lType = lineType;
            if (lType.Name == "ByLayer") {
                lType = entity.Layer.LineType;
            }

            if (lType.Segments.Count() <= 0) {
                return string.Empty;
            }

            foreach (LineType.Segment segment in lType.Segments) {
                if (segment.Length == 0) {
                    result.Add(1);
                }
                else if (segment.Length > 0) {
                    result.Add(segment.Length);
                }
                else {
                    result.Add(Math.Abs(segment.Length));
                }
            }

            while (result.Count % 2 != 2 && result.Count < 4) {
                result.Add(result[result.Count - 2]);
            }

            if (result[result.Count - 1] == 0) {
                result.Add(result[result.Count - 2]);
            }

            StringBuilder sb = new StringBuilder();
            foreach (double item in result) {
                sb.Append(SvgElementBase.Cd(item)).Append(" ");
            }

            return sb.ToString().Trim();
        }
    }
}
