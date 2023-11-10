#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;


namespace ACadSvg {

    internal class LeaderSvg : EntitySvg {

        private Leader _leader;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="leader"></param>
        /// <param name="ctx"></param>
        public LeaderSvg(Entity leader, ConversionContext ctx) {
            _leader = (Leader)leader;
            SetStandardIdAndClassIf(leader, ctx);
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            return new PathElement()
                .AddPoints(Utils.VerticesToArray(_leader.Vertices))
                .WithID(ID)
                .WithClass(Class)
                .WithStroke(ColorUtils.GetHtmlColor(_leader, _leader.Color));
		}
    }
}