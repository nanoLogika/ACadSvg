#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Text;

using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Objects;
using ACadSharp.Tables;
using SvgElements;
using CSMath;
using static ACadSharp.Objects.MultiLeaderAnnotContext;
using ACadSvg.Extensions;
using static ACadSvg.Extensions.MultiLeaderProperties;


namespace ACadSvg {

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="MultiLeader"/> entity.
    /// The <see cref="MultiLeader"/> entity is converted into a complex element including
    /// several <i>path</i> elements for the for the leader line and optionlly a dog-leg.
    /// A standard arrowhead can be constituted by a filled path element. Other standrd
    /// arrowheads or custom arrowheds can be contituted by the entities from a
    /// <see cref="BlockRecord"/> evolving into a <i>use</i> element.
    /// The informational content of the <see cref="MultiLeader"/> can either be a multiline
    /// text contituted by a <i>text</i> element with one or more <i>tspn</i> elements or
    /// custom content from a <see cref="BlockRecord"/> evolving into a <i>use</i> element.
    /// </summary>
    internal class MultiLeaderSvg : EntitySvg {

        private MultiLeader _multiLeader;
        private MultiLeaderProperties _mlProps;
        private MultiLeaderStyle _style;
        private MultiLeaderAnnotContext _contextData;
        private MultiLeaderPropertyOverrideFlags _flags;
        private LeaderRoot _leaderRoot;


        /// <summary>
		/// Initializes a new instance of the <see cref="MultiLeaderSvg"/> class
		/// for the specified <see cref="MultiLeader"/> entity.
        /// </summary>
        /// <param name="multiLeader">The <see cref="MultiLeader"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public MultiLeaderSvg(Entity multiLeader, ConversionContext ctx) : base(ctx) {
            _multiLeader = (MultiLeader)multiLeader;
            SetStandardIdAndClassIf(multiLeader, ctx);

            _mlProps = new MultiLeaderProperties(_multiLeader);

            _style = _multiLeader.Style;
            _contextData = _multiLeader.ContextData;
            _flags = _multiLeader.PropertyOverrideFlags;

            //  Assume: There can exist only one LEADER, we have only one content
            _leaderRoot = _contextData.LeaderRoots[0];
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
			GroupElement groupElement = new GroupElement();
            groupElement.WithID(ID).WithClass(Class);

            //  The end of the leader includuing dogleg
            //  The head vertex of the first leader line or fallback if no leader lines exist?
            //  the location of the text or block context respectively
            XYZ contentBasePoint = _contextData.ContentBasePoint;
            XYZ basePoint = _contextData.BasePoint;
            XYZ location = _contextData.Location;

            //  TODO This is a workaround due to bad data from DWG or lack of understanding?
            if (contentBasePoint.X < basePoint.X) {
                // contentBasePoint.X = location.X + _multiLeader.ContextData.LandingGap;
            }

            ////XYZ labelPoint = basePoint + new XYZ(0, 20, 0);
            ////multiLeaderSb.AppendLine($"<circle cx=\"{Cd(labelPoint.X)}\" cy=\"{Cd(labelPoint.Y)}\" r=\"10\" stroke=\"magenta\"></circle>");

            //  Dogleg etc.
            var landingDistance = _mlProps.LandingDistance;

            //  var direction = contextData.Direction;
            var dogLegDirection = _leaderRoot.Direction;


            bool enableDogleg = _mlProps.EnableDogleg;

            bool drawDogleg = enableDogleg &&
                landingDistance > 0 && dogLegDirection.GetLength() > 0;

            XYZ leaderEndPoint;
            if (drawDogleg) {
                leaderEndPoint = contentBasePoint - dogLegDirection * landingDistance;
            }
            else {
                leaderEndPoint = contentBasePoint;
            }

            var enabledLanding = _mlProps.EnableLanding;
            var landingGap = _mlProps.LandingGap;
            var contentType = _mlProps.ContentType;
            var textStyle = _mlProps.TextStyle;
            var textLeftAttachment = _mlProps.TextLeftAttachment;
            var textAngle = _mlProps.TextAngle;
            var textColor = _mlProps.TextColor;
            var scaleFactor = _mlProps.ScaleFactor;
            var textRightAttachment = _mlProps.TextRightAttachment;
            var textTopAttachment = _mlProps.TextTopAttachment;
            var textBottomAttachment = _mlProps.TextBottomAttachment;
            double? dogLegLineWeight = null;

            Color leaderLineColor = Color.ByLayer;
            foreach (LeaderRoot leaderRoot in _multiLeader.ContextData.LeaderRoots) {
                foreach (LeaderLine leaderLine in leaderRoot.Lines) {
                    LeaderLineProperties lineProps = _mlProps.LeaderLines[leaderLine];
                    var pathType = lineProps.PathType;
                    var leaderLineType =  lineProps.LeaderLineType;
                    double? leaderLineWeight = LineUtils.GetLineWeight(lineProps.LeaderLineWeight, _multiLeader, _ctx);
                    dogLegLineWeight = leaderLineWeight;
                    leaderLineColor = lineProps.LeaderLineColor;

                    switch (pathType) {
                    case MultiLeaderPathType.Spline:
                        //  TODO
                        break;
                    case MultiLeaderPathType.StraightLineSegments:
							groupElement.Children.Add(
                            new PathElement()
                            .AddPoints(Utils.VerticesToArray(leaderLine.Points))
                            .AddLine(leaderEndPoint.X, leaderEndPoint.Y)
                            .WithStroke(ColorUtils.GetHtmlColor(_multiLeader, leaderLineColor))
                            .WithStrokeWidth(leaderLineWeight));
                        break;
                    case MultiLeaderPathType.Invisible:
                    default:
                        break;
                    }

                    var arrowHead = lineProps.ArrowHead;
                    var arrowHeadSize = lineProps.ArrowHeadSize;

                    var points = leaderLine.Points;
                    XY arrowPoint = Utils.ToXY(points[0]);
                    XY arrowDirection = Utils.ToXY(points.Count > 1 ? points[0] - points[1] : points[0] - leaderEndPoint).Normalize() * arrowHeadSize;

                    if (arrowHead != null) {
						groupElement.Children.Add(XElementFactory.CreateArrowheadFromBlock(arrowHead, arrowPoint, arrowDirection, arrowHeadSize));
                    }
                    else {
                        var arrowColor = leaderLineColor;
                        SvgElementBase arrowPath = XElementFactory.CreateStandardArrowHead(arrowPoint, arrowDirection, ColorUtils.GetHtmlColor(_multiLeader, arrowColor));
						groupElement.Children.Add(arrowPath);
                    }
                }
            }
            if (drawDogleg) {
				groupElement.Children.Add(new PathElement()
                    .AddLine(leaderEndPoint.X, leaderEndPoint.Y, contentBasePoint.X, contentBasePoint.Y)
                    .WithStroke(ColorUtils.GetHtmlColor(_multiLeader, leaderLineColor))
                    .WithStrokeWidth(dogLegLineWeight));
            }

            //groupElement.Comment = buildPropertiesComment();

            string text = _contextData.TextLabel;
            if (_contextData.HasTextContents && !string.IsNullOrEmpty(text.Trim())) {
                double textHeight = _mlProps.TextHeight;
                double textSize = TextUtils.GetTextSize(textHeight);
                var textLocX = location.X;
                var textLocY = location.Y;
                textLocY -= textHeight;

                //  Rotation is performed first, must be positive.
                double rot = _contextData.Rotation * 180 / Math.PI;

                string textAnchor = TextUtils.HorizontalAlignmentToTextAnchor(_multiLeader.TextAttachmentPoint);
                TextUtils.StyleToValues(textStyle, textSize, out string fontFamily, out double fontSize, out bool bold, out bool italic);
                var tspans = TextUtils.ConvertMTextToHtml(textLocX, textLocY, text, textSize, textStyle);

                var textElement = new TextElement()
                        .WithFont(fontFamily, fontSize, bold, italic)
                        .WithTextAnchor(textAnchor)
                        .WithTspans(tspans)
                        .WithFill(ColorUtils.GetHtmlTextColor(_multiLeader, textColor))
                        .ReverseY(_ctx.ConversionOptions.ReverseY)
                        .AddRotate(rot, textLocX, textLocY);
                groupElement.Children.Add(textElement);

                if (_multiLeader.TextFrame) {
                    double lg = _mlProps.LandingGap;
                    double fx = contentBasePoint.X;
                    double fy = location.Y - lg;
                    double fw = 60;
                    double fh = 3 * textSize + 2 * lg;
                    var rectangleElement = new RectangleElement()
					{
                        X = fx,
                        Y = fy,
                        Width = fw,
                        Height = fh
					};
                    rectangleElement.AddRotate(rot, textLocX, textLocY);
					groupElement.Children.Add(rectangleElement);
                }
            }

            if (_contextData.HasContentsBlock) {
                var blockContent = _mlProps.BlockContent;
                var blockContentColor = _mlProps.BlockContentColor;
                var blockContentScale = _mlProps.BlockContentScale;
                var blockContentRotation = _mlProps.BlockContentRotation;
                var blockContentConnection = _mlProps.BlockContentConnection;

                var blockLocX = location.X;
                var blockLocY = location.Y;
                var blockName = Utils.CleanBlockName(blockContent.Name);
				groupElement.Children.Add(new UseElement().WithXY(blockLocX, blockLocY).WithGroupId(blockName));
            }

            return groupElement;
        }


        private string buildPropertiesComment() {
            StringBuilder commentSb = new StringBuilder();

            commentSb.AppendLine("MultiLeader");

            //  Draw points for test
            XYZ contentBasePoint = _contextData.ContentBasePoint;
            XYZ basePoint = _contextData.BasePoint;
            XYZ location = _contextData.Location;

            //commentSb.AppendLine(new PointElement().WithXY(contentBasePoint.X, contentBasePoint.Y).WithStroke("aqua").ToString());
            //commentSb.AppendLine(new PointElement().WithXY(basePoint.X, basePoint.Y).WithStroke("magenta").ToString());
            //commentSb.AppendLine(new PointElement().WithXY(location.X, location.Y).WithStroke("pink").ToString());

            commentSb.Append("Scale factor: ").AppendLine($"{_mlProps.ScaleFactor}");
            //Dogleg Length
            commentSb.Append("Dogleg Length ml: ").Append(_multiLeader.LandingDistance).Append(", mls: ").Append(_multiLeader.Style.LandingDistance).AppendLine();
            //Dogleg/Landing
            commentSb.Append("Dogleg, Landing: ").Append(_multiLeader.EnableDogleg).Append(", ").AppendLine(_multiLeader.EnableLanding.ToString());
            commentSb.Append("Dogleg, Landing: ").Append(_contextData.LeaderRoots[0].Unknown).Append(", ").AppendLine(_contextData.LeaderRoots[0].ContentValid.ToString());
            //Ausrichten
            commentSb.Append("Ausrichten?/Alignment: ").AppendLine(_contextData.TextAlignment.ToString());
            //Ausrichten
            commentSb.Append("Ausrichten?/TextAttachmentPoint: ").AppendLine(_multiLeader.TextAttachmentPoint.ToString());
            //Richtung
            commentSb.Append("Richtung/Direction: ").AppendLine(_contextData.Direction.ToString());
            //Drehung
            commentSb.Append("Drehung/Rotation: ").AppendLine(_contextData.Rotation.ToString());
            //Textzeilenabstand
            //Zuordnungstyp
            commentSb.AppendLine("Zuordungstyp (TextAttachmentrType):");
            commentSb.Append("    ML: ").Append(_multiLeader.TextAttachmentDirection).Append(", MLS: ").Append(_multiLeader.Style.TextAttachmentDirection).Append(", LR0: ").Append(_contextData.LeaderRoots[0].AttachmentDirection).AppendLine();
            commentSb.Append("Zuordnungstyp/AttachmentType: ").AppendLine(_contextData.AttachmentType.ToString());
            //Zuordnung links
            commentSb.Append("Zuordnung links/TextLeftAttachment: ").AppendLine(_contextData.TextLeftAttachment.ToString());
            //Zuordnung rechts
            commentSb.Append("Zuordnungstyp/TextRightAttachment: ").AppendLine(_contextData.TextRightAttachment.ToString());
            //Zuordnung oben
            commentSb.Append("Zuordnung oben/TextTopAttachment: ").AppendLine(_contextData.TextTopAttachment.ToString());
            //Zuordnung unten
            commentSb.Append("Zuordnung unten/TextBottomAttachment: ").AppendLine(_contextData.TextBottomAttachment.ToString());
            //Abstand der Verlängerung
            commentSb.Append("Abstand der Verlängerung/LandingGap: ").AppendLine(_contextData.LandingGap.ToString());
            //Textrahmen
            commentSb.Append("Textrahmen/TextFrame: ").AppendLine(_multiLeader.TextFrame.ToString());
            commentSb.Append("??/BoundaryWidth: ").AppendLine(_contextData.BoundaryWidth.ToString());
            commentSb.Append("??/BoundaryHeight: ").AppendLine(_contextData.BoundaryHeight.ToString());

            return commentSb.ToString();
        }
    }
}