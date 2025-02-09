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
using ACadSvg.Extensions;


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

        private DimensionProperties _dimProps;
        private Leader _leader;


        /// <summary>
		/// Initializes a new instance of the <see cref="LeaderSvg"/> class
		/// for the specified <see cref="Leader"/> entity.
        /// </summary>
        /// <param name="leader">The <see cref="Leader"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public LeaderSvg(Entity leader, ConversionContext ctx) : base(ctx) {
            _leader = (Leader)leader;
            _dimProps = new DimensionProperties(_leader, _leader.Style);
            SetStandardIdAndClassIf(leader, ctx);
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            GroupElement groupElement = new GroupElement();
            groupElement.WithID(ID).WithClass(Class);

            string? lineColor = ColorUtils.GetHtmlColor(_leader, _leader.Color);
            //  TODO Also consider _dimEx.DimensionLineColor

            IList<XYZ> vertices = new List<XYZ>(_leader.Vertices);
            if (_leader.HasHookline) {
                var annotationOffset = _leader.AnnotationOffset;
                var blockOffset = _leader.BlockOffset;
                var hookLineDirectionF = _leader.HookLineDirection ? -1 : 1;
                var horizontalDirection = _leader.HorizontalDirection * hookLineDirectionF;
                XYZ landingPoint = vertices[vertices.Count - 1] - horizontalDirection * _leader.Style.ArrowSize;
                vertices.Insert(vertices.Count - 1, landingPoint);

                double textLength = 0;
                if (_leader.AssociatedAnnotation is TextEntity textEntity) {
                    var text = textEntity.Value;
                    textLength = TextUtils.GetTextLength(text, textEntity.Style.Height);
                }
                if (_leader.AssociatedAnnotation is MText mText) {
                    var text = mText.Value;
                    textLength = TextUtils.GetTextLength(text, mText.Style.Height);
                }

                if (textLength > 0) {
                    XYZ textEndPoint = vertices[vertices.Count - 1] + horizontalDirection * textLength;
                    vertices.Add(textEndPoint);
                }
            }

            var leaderLine = new PathElement()
                .AddPoints(Utils.VerticesToArray(vertices))
                .WithStroke(lineColor)
                .WithStrokeWidth(LineUtils.GetLineWeight(_leader.LineWeight, _leader, _ctx))
                .WithFill("none");
            groupElement.Children.Add(leaderLine);

            if (_leader.ArrowHeadEnabled) {
                double arrowSize = _dimProps.ArrowSize;
                var arrowColor = lineColor;
                XY arrowPoint = vertices[0].ToXY();
                XY arrowDirection = (vertices[0] - vertices[1]).ToXY().Normalize() * arrowSize;

                BlockRecord arrowBlock = _dimProps.LeaderArrow;
                if (arrowBlock == null) {
                    groupElement.Children.Add(XElementFactory.CreateStandardArrowHead(arrowPoint, arrowDirection, arrowColor));
                }
                else {
                    groupElement.Children.Add(XElementFactory.CreateArrowheadFromBlock(arrowBlock, arrowPoint, arrowDirection, arrowSize));
                }
            }

            return groupElement;
        }
    }
}