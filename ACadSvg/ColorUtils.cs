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
            if (color.IsByBlock || color.IsByLayer) {
                return string.Empty;    //  This case should never occur
            }

            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
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
            if (color.IsByBlock || color.IsByLayer) {
                return string.Empty;    //  This case should never occur
            }

            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}
