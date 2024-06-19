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


            _style = _multiLeader.Style;
            _contextData = _multiLeader.ContextData;
            _flags = _multiLeader.PropertyOverrideFlags;

            //  Assume: There can exist only one LEADER, we have only one content
            _leaderRoot = _contextData.LeaderRoots[0];
        }

        #region -  Overridden-properties getter

        private MultiLeaderPathType getPathType(LeaderLine leaderLine) {
            if (leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.PathType)) {
                return leaderLine.PathType;
            }
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.PathType)) {
                return _multiLeader.PathType;
            }

            return _style.PathType;
        }


        private LineType getLeaderLineType(LeaderLine leaderLine) {
            if (leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.LineType)) {
                return leaderLine.LineType;
            }
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.LeaderLineType)) {
                return _multiLeader.LineType;
            }

            return _style.LeaderLineType;
        }


        private Color getLeaderLineColor(LeaderLine leaderLine) {
            if (leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.LineColor)) {
                return leaderLine.LineColor;
            }
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.LineColor)) {
                return _multiLeader.LineColor;
            }

            return _style.LineColor;
        }


        private LineweightType getLeaderLineWeight(LeaderLine leaderLine) {
            if (leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.LineWeight)) {
                return leaderLine.LineWeight;
			}
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.LeaderLineWeight)) {
                return _multiLeader.LineWeight;
            }

            return _style.LeaderLineWeight;
        }


        private bool getEnableLanding() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.EnableLanding)) {
                return _multiLeader.EnableLanding;
            }

            return _style.EnableLanding;
        }


        private double getLandingGap() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.LandingGap)) {
                return _contextData.LandingGap;
            }

            return _style.LandingGap;
        }


        private double getLandingDistance() {
            if (_multiLeader.LandingDistance != 0 && _flags.HasFlag(MultiLeaderPropertyOverrideFlags.LandingDistance)) {
                return _leaderRoot.LandingDistance;
            }

            return _style.LandingDistance;
        }


        private bool getEnableDogleg() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.EnableDogleg)) {
                return _multiLeader.EnableDogleg;
            }

            return _style.EnableDogleg;
        }


        private BlockRecord getArrowHead(LeaderLine leaderLine) {
            var mlAh = _multiLeader.Arrowhead;
            var styAh = _style.Arrowhead;
            var lAh = leaderLine.Arrowhead;

            BlockRecord arrowhead = styAh;
            if (styAh == null || (mlAh != null && _multiLeader.PropertyOverrideFlags.HasFlag(MultiLeaderPropertyOverrideFlags.Arrowhead))) {
                arrowhead = mlAh;
            }
            if (arrowhead == null || (lAh != null && leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.Arrowhead))) {
                arrowhead = lAh;
            }

            return arrowhead;
        }


        private double getArrowHeadSize(LeaderLine leaderLine) {
            var mlAhs = _multiLeader.ArrowheadSize;
            var cdAhs = _contextData.ArrowheadSize;
            var styAhs = _style.ArrowheadSize;
            var lAhs = leaderLine.ArrowheadSize;
            var arrowheadSize = styAhs;

            if (cdAhs > 0 && _flags.HasFlag(MultiLeaderPropertyOverrideFlags.ArrowheadSize)) {
                arrowheadSize = cdAhs;
            }
            if (lAhs > 0 && leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.ArrowheadSize)) {
                arrowheadSize = lAhs;
			}

            return arrowheadSize;
        }


        private LeaderContentType getContentType() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.ContentType)) {
                return _multiLeader.ContentType;
            }

            return _style.ContentType;
		}


        private TextStyle getTextStyle() {
            var styTS = _style.TextStyle;
            var cdTS = _contextData.TextStyle;

            if (cdTS != null && _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextStyle)) {
                return _contextData.TextStyle;
			}
            if (styTS != null) {
                return _style.TextStyle;
            }

            return _multiLeader.TextStyle;
		}


        private TextAttachmentType getTextLeftAttachment() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextLeftAttachment)) {
                //  Can _multiLeader.TextLeftAttachment have a different value?
                return _contextData.TextLeftAttachment;
            }

            return _style.TextLeftAttachment;
		}


        private TextAngleType getTextAngle() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextAngle)) {
                return _multiLeader.TextAngle;
            }

            return _style.TextAngle;
        }


        private Color getTextColor() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextColor)) {
                //  return _multiLeader.TextColor;
                return _contextData.TextColor;
            }
            else { 
                return _style.TextColor;
            }
        }


        private double getTextHeight() {
            //  Text height hierarchy:
            //  Text height can only be defined in one place:
            //      -  in the dimension/leader style or
            //      -  in the text style.
            //  If the text height will not change or is disabled in the dimension or leader style,
            //  then it needs to be adjusted in the text style that the dimension or leader style uses.
            //  Zeroing the height in the text style will then allow the height to be defined in the dimension/leader style.

            double textHeight = _multiLeader.TextStyle.Height;
            double styleTextHeight = _style.TextHeight;
            if (textHeight == 0 || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextHeight)) {
                textHeight = _contextData.TextHeight;
            }

            return textHeight;
        }


        private BlockRecord getBlockContent() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.BlockContent)) {
                //  return _multiLeader.BlockContent;
                return _contextData.BlockContent;
            }

            return _style.BlockContent;
        }


        private Color getBlockContentColor() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.BlockContentColor)) {
                //  return _multiLeader.BlockContentColor;
                return _contextData.BlockContentColor;
            }

            return _style.BlockContentColor;
		}


        private XYZ getBlockContentScale() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.BlockContentScale)) {
                //return _multiLeader.BlockContentScale;
                return _contextData.BlockContentScale;
            }

            return _style.BlockContentScale;
        }


        private double getBlockContentRotation() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.BlockContentRotation)) {
                return _multiLeader.BlockContentRotation;
            }

            return _style.BlockContentRotation;
        }


        private object getBlockContentConnection() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.BlockContentConnection)) {
                return _multiLeader.BlockContentConnection;
            }

            return _style.BlockContentConnection;
        }


        private double getScaleFactor() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.ScaleFactor)) {
                return _multiLeader.ScaleFactor;
            }

            return _style.ScaleFactor;
        }


        private TextAttachmentType getTextRightAttachment() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextRightAttachment)) {
                // ?? _multiLeader.TextRightAttachment;
                return _contextData.TextRightAttachment;
            }

            return _style.TextRightAttachment;
        }


        private TextAttachmentType getTextTopAttachment() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextTopAttachment)) {
                // ?? _multiLeader.TextTopAttachment;
                return _contextData.TextTopAttachment;
            }

            return _style.TextTopAttachment;
        }


        private TextAttachmentType getTextBottomAttachment() {
            if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextBottomAttachment)) {
                // ?? _multiLeader.TextBottomAttachment;
                return _contextData.TextBottomAttachment;
            }

            return _style.TextTopAttachment;
        }

        #endregion

        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
			GroupElement groupElement = new GroupElement();
            groupElement.WithID(ID).WithClass(Class);

            //  The end of the leader includuing dogleg
            //  The head vertex of the first leader line or fallback if no leader lines exist?
            //  the location of the text or block context respectively
            XYZ contentBasePoint = _contextData.ContentBasePoint;
            XYZ basePoint = _contextData.BasePoint;
            XYZ location = _contextData.BlockContentLocation;

            //  TODO This is a workaround due to bad data from DWG or lack of understanding?
            if (contentBasePoint.X < basePoint.X) {
                // contentBasePoint.X = location.X + _multiLeader.ContextData.LandingGap;
            }

            ////XYZ labelPoint = basePoint + new XYZ(0, 20, 0);
            ////multiLeaderSb.AppendLine($"<circle cx=\"{Cd(labelPoint.X)}\" cy=\"{Cd(labelPoint.Y)}\" r=\"10\" stroke=\"magenta\"></circle>");

            //  Dogleg etc.
            var landingDistance = getLandingDistance();

            //  var direction = contextData.Direction;
            var dogLegDirection = _leaderRoot.Direction;


            bool enableDogleg = getEnableDogleg();

            bool drawDogleg = enableDogleg &&
                landingDistance > 0 && dogLegDirection.GetLength() > 0;

            XYZ leaderEndPoint;
            if (drawDogleg) {
                leaderEndPoint = contentBasePoint - dogLegDirection * landingDistance;
            }
            else {
                leaderEndPoint = contentBasePoint;
            }

            var enabledLanding = getEnableLanding();
            var landingGap = getLandingGap();
            var contentType = getContentType();
            var textStyle = getTextStyle();
            var textLeftAttachment = getTextLeftAttachment();
            var textAngle = getTextAngle();
            var textColor = getTextColor();
            var scaleFactor = getScaleFactor();
            var textRightAttachment = getTextRightAttachment();
            var textTopAttachment = getTextTopAttachment();
            var textBottomAttachment = getTextBottomAttachment();
            double? dogLegLineWeight = null;

            Color leaderLineColor = Color.ByLayer;
            foreach (LeaderRoot leaderRoot in _multiLeader.ContextData.LeaderRoots) {
                foreach (LeaderLine leaderLine in leaderRoot.Lines) {
                    var pathType = getPathType(leaderLine);
                    var leaderLineType = getLeaderLineType(leaderLine);
                    double? leaderLineWeight = LineUtils.GetLineWeight(getLeaderLineWeight(leaderLine), _multiLeader, _ctx);
                    dogLegLineWeight = leaderLineWeight;
                    leaderLineColor = getLeaderLineColor(leaderLine);

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

                    var arrowHead = getArrowHead(leaderLine);
                    var arrowHeadSize = getArrowHeadSize(leaderLine);

                    var points = leaderLine.Points;
                    XY arrowPoint = points[0].ToXY();
                    XY arrowDirection = (points.Count > 1 ? points[0] - points[1] : points[0] - leaderEndPoint).ToXY().Normalize() * arrowHeadSize;

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
                double textHeight = getTextHeight();
                double textSize = TextUtils.GetTextSize(textHeight);
                var textLocX = location.X;
                var textLocY = location.Y;
                textLocY -= textHeight;

                //  Rotation is performed first, must be positive.
                double rot = _contextData.BlockContentRotation * 180 / Math.PI;

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
                    double lg = getLandingGap(); ;
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
                var blockContent = getBlockContent();
                var blockContentColor = getBlockContentColor();
                var blockContentScale = getBlockContentScale();
                var blockContentRotation = getBlockContentRotation();
                var blockContentConnection = getBlockContentConnection();

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
            XYZ location = _contextData.BlockContentLocation;

            //commentSb.AppendLine(new PointElement().WithXY(contentBasePoint.X, contentBasePoint.Y).WithStroke("aqua").ToString());
            //commentSb.AppendLine(new PointElement().WithXY(basePoint.X, basePoint.Y).WithStroke("magenta").ToString());
            //commentSb.AppendLine(new PointElement().WithXY(location.X, location.Y).WithStroke("pink").ToString());

            commentSb.Append("Scale factor: ").AppendLine($"{getScaleFactor()}");
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
            commentSb.Append("Drehung/Rotation: ").AppendLine(_contextData.BlockContentRotation.ToString());
            //Textzeilenabstand
            //Zuordnungstyp
            commentSb.AppendLine("Zuordungstyp (TextAttachmentrType):");
            commentSb.Append("    ML: ").Append(_multiLeader.TextAttachmentDirection).Append(", MLS: ").Append(_multiLeader.Style.TextAttachmentDirection).Append(", LR0: ").Append(_contextData.LeaderRoots[0].TextAttachmentDirection).AppendLine();
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