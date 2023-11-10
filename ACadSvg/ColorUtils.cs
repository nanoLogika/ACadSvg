#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;


namespace ACadSvg {

    internal static class ColorUtils {

        public static string GetHtmlColor(Entity entity, Color color) {
            if (color.IsByBlock) {
                //  Color will be set at group that represents this block
                BlockRecord block = entity.Owner as BlockRecord;
                if (block == null) {
                    return string.Empty;
                }

                color = block.BlockEntity.Color;
                if (!color.IsByLayer) {
                    return string.Empty;
                }
            }

            if (color.IsByLayer) {
                color = entity.Layer.Color;
            }
            ReadOnlySpan<byte> colorSpan = getColorRgb(color);

            if (colorSpan.IsEmpty) {
                return string.Empty;
            }

            System.Drawing.Color colorRgb = System.Drawing.Color.FromArgb(255, colorSpan[0], colorSpan[1], colorSpan[2]);
            return $"{System.Drawing.ColorTranslator.ToHtml(colorRgb)}";
        }


        public static string GetHtmlTextColor(ACadSharp.Entities.Entity entity, Color color) {
            if (color.IsByBlock) {
                //  Text color must be set as fill, cannot be set at group.
                BlockRecord block = entity.Owner as BlockRecord;
                if (block == null) {
                    return string.Empty;
                }

                color = block.BlockEntity.Color;
            }

            if (color.IsByLayer) {
                color = entity.Layer.Color;
            }
            ReadOnlySpan<byte> colorSpan = getColorRgb(color);

            if (colorSpan.IsEmpty) {
                return string.Empty;
            }

            System.Drawing.Color colorRgb = System.Drawing.Color.FromArgb(255, colorSpan[0], colorSpan[1], colorSpan[2]);
            return $"{System.Drawing.ColorTranslator.ToHtml(colorRgb)}";
        }


        /// <summary>
        /// Gets a RGB color value from an <see cref="ACadSharp.Color"/> object.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private static ReadOnlySpan<byte> getColorRgb(Color color) {
            if (color.IsByBlock || color.IsByLayer) {
                return ReadOnlySpan<byte>.Empty;    //  This case should never occur
            }
            return color.GetRgb();
        }
    }
}
