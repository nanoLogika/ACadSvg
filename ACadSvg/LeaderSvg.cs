#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;

using CSMath;

using SvgElements;


namespace ACadSvg {

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="Leader"/> entity.
    /// The <see cref="Leader"/> entity is converted into a complex element including
    /// a <i>path</i> element for the leader line and another <i>path</i> element for
    /// the arrowhead.
    /// </summary>
    /// <remarks><para>
    /// The converter for the <see cref="Leader"/> entity is not yet fully implemented.
    /// Currently only the leader line but no arrowhead is created.
    /// </para></remarks>
    internal class LeaderSvg : EntitySvg {

        private Leader _leader;


        /// <summary>
		/// Initializes a new instance of the <see cref="LeaderSvg"/> class
		/// for the specified <see cref="Leader"/> entity.
        /// </summary>
        /// <param name="leader">The <see cref="Leader"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public LeaderSvg(Entity leader, ConversionContext ctx) {
            _leader = (Leader)leader;
            SetStandardIdAndClassIf(leader, ctx);
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            GroupElement groupElement = new GroupElement();
            groupElement.WithID(ID).WithClass(Class);

            var lineColor = ColorUtils.GetHtmlColor(_leader, _leader.Color);
            var vertices = _leader.Vertices;

            var leaderLine = new PathElement()
                .AddPoints(Utils.VerticesToArray(_leader.Vertices))
                .WithStroke(lineColor);
            groupElement.Children.Add(leaderLine);

            if (_leader.ArrowHeadEnabled) {
                var dimStyle = _leader.Style;
                var arrowColor = lineColor;
                var record = GetExtendedDataRecord(_leader, "ACAD", "DSTYLE", 41);
                double arrowSize;
                if (record != null) {
                    arrowSize = (double)record.Value;
                }
                else {
                    var sf = dimStyle.ScaleFactor;
                    arrowSize = dimStyle.ArrowSize * 250;  //  Why is this scaler necessary 
                }
                var arrowDirection = (vertices[0] - vertices[1]).Normalize() * arrowSize;
                var arrowHead = XElementFactory.CreateStandardArrowHead(vertices[0], arrowDirection, arrowColor);

                groupElement.Children.Add(arrowHead);
            }

            return groupElement;

        }
    }
}