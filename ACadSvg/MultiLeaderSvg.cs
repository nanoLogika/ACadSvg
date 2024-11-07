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
        private double _multiLeaderLineWidth;
        private string _multiLeaderLineColor;
        private GroupElement _groupElement;


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

            _multiLeaderLineWidth = LineUtils.GetLineWeight(_mlProps.LineWeight, _multiLeader, ctx).GetValueOrDefault();
            _multiLeaderLineColor = ColorUtils.GetHtmlColor(_multiLeader, _mlProps.LineColor);
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
			_groupElement = new GroupElement();
            _groupElement.WithID(ID).WithClass(Class);

            //  The end of the leader includuing dogleg
            //  The head vertex of the first leader line or fallback if no leader lines exist?
            XY contentBasePoint = _contextData.ContentBasePoint.ToXY();
            XY basePoint = _contextData.BasePoint.ToXY();

            bool enableDogleg = _mlProps.EnableDogleg;

            bool enabledLanding = _mlProps.EnableLanding;
            double landingGap = _mlProps.LandingGap;

            CreateDebugPoint(basePoint, "pink");
            CreateDebugPoint(contentBasePoint, "dodgerblue");

            TextAlignmentType styleTextAlignment = _style == null ? TextAlignmentType.Left : _style.TextAlignment;
            TextAlignmentType ctxTextAlignment = _contextData.TextAlignment;
            TextAlignmentType textAlignment = _multiLeader.TextAlignment;
            bool overTextAlignment = _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextAlignment);

            TextAttachmentPointType ctxTextAttaPoint = _contextData.TextAttachmentPoint;
            TextAttachmentPointType textAttaPoint = _multiLeader.TextAttachmentPoint;

            string hexHandle = _multiLeader.Handle.ToString("X");
            System.Diagnostics.Debug.WriteLine($"------------ Handle: {hexHandle} -----------------");
            System.Diagnostics.Debug.WriteLine($"Override flag:       {overTextAlignment}");
            System.Diagnostics.Debug.WriteLine($"Style.TextAlignment: {styleTextAlignment}");
            System.Diagnostics.Debug.WriteLine($"Ctx.TextAlignment:   {ctxTextAlignment}");
            System.Diagnostics.Debug.WriteLine($"ML.TextAlignment:    {textAlignment}");

            System.Diagnostics.Debug.WriteLine($"Ctx.TextAttaPoint:   {ctxTextAttaPoint}");
            System.Diagnostics.Debug.WriteLine($"ML.TextAttaPoint:    {textAttaPoint}");

            System.Diagnostics.Debug.WriteLine($"Ctx.FlowDrection:    {_mlProps.FlowDirection}");

            var scaleFactor = _mlProps.ScaleFactor;
            var contentType = _mlProps.ContentType;

            foreach (LeaderRoot leaderRoot in _multiLeader.ContextData.LeaderRoots) {
                XY connectionPoint = leaderRoot.ConnectionPoint.ToXY();
                CreateDebugPoint(connectionPoint, "green");

                XY direction = leaderRoot.Direction.ToXY();
                double landingDistance = leaderRoot.LandingDistance;

                //  NOTE: use _multiLeader.TextAttachmentDirection instead _mlProps.TextAttachmentDirection
                //        Flag is missing or definition is wrong
                bool drawDogleg = enableDogleg &&
                        landingDistance > 0 &&
                        direction.GetLength() > 0;

                XY landingEndPoint = connectionPoint + direction * landingDistance;
                if (_multiLeader.TextAttachmentDirection == TextAttachmentDirectionType.Horizontal) {
                    if (!drawDogleg) {
                        connectionPoint = landingEndPoint;
                    }
                }
                else {
                    drawDogleg = false;
                }

                //CreateDebugPoint(connectionPoint, "yellow");
                //CreateDebugPoint(landingEndPoint, "green");

                foreach (LeaderLine leaderLine in leaderRoot.Lines) {
                    LeaderLineProperties lineProps = _mlProps.LeaderLines[leaderLine];
                    var pathType = lineProps.PathType;
                    var leaderLineType =  lineProps.LeaderLineType;
                    double? leaderLineWeight = LineUtils.GetLineWeight(lineProps.LeaderLineWeight, _multiLeader, _ctx);
                    string leaderLineColor = ColorUtils.GetHtmlColor(_multiLeader, lineProps.LeaderLineColor);

                    switch (pathType) {
                    case MultiLeaderPathType.Spline:
                        //  TODO Create spline leader
                        break;
                    case MultiLeaderPathType.StraightLineSegments:
						_groupElement.Children.Add(
                            new PathElement()
                                .AddPoints(Utils.VerticesToArray(leaderLine.Points))
                                .AddLine(connectionPoint.X, connectionPoint.Y)
                                .WithStroke(leaderLineColor)
                                .WithStrokeWidth(leaderLineWeight));
                        break;
                    case MultiLeaderPathType.Invisible:
                    default:
                        break;
                    }

                    var arrowHead = lineProps.ArrowHead;
                    var arrowHeadSize = lineProps.ArrowHeadSize;

                    var points = leaderLine.Points;
                    XY arrowPoint = points[0].ToXY();
                    XY arrowDirection = (points.Count > 1 ? arrowPoint - points[1].ToXY() : arrowPoint - connectionPoint).Normalize() * arrowHeadSize;

                    if (arrowHead != null) {
						_groupElement.Children.Add(XElementFactory.CreateArrowheadFromBlock(arrowHead, arrowPoint, arrowDirection, arrowHeadSize));
                    }
                    else {
                        var arrowColor = leaderLineColor;
                        SvgElementBase arrowPath = XElementFactory.CreateStandardArrowHead(arrowPoint, arrowDirection, leaderLineColor);
						_groupElement.Children.Add(arrowPath);
                    }
                }

                if (drawDogleg) {
				    _groupElement.Children.Add(new PathElement()
                        .AddLine(connectionPoint.X, connectionPoint.Y, landingEndPoint.X, landingEndPoint.Y)
                        .WithStroke(_multiLeaderLineColor)
                        .WithStrokeWidth(_multiLeaderLineWidth));
                }
            }

            //groupElement.Comment = buildPropertiesComment();

            if (_contextData.HasTextContents && !string.IsNullOrEmpty(_contextData.TextLabel.Trim())) {
                XY contentLocation = _contextData.TextLocation.ToXY();
                CreateDebugPoint(contentLocation, "blue");
                createTextLabel(contentLocation, out double textLabelWidth, out double textLabelHeight);
                createTextFrame(contentBasePoint, contentLocation, _contextData.LeaderRoots[0].Direction.ToXY(), textLabelWidth, textLabelHeight);
            }

            if (_contextData.HasContentsBlock) {
                XY contentLocation = _contextData.BlockContentLocation.ToXY();
                CreateDebugPoint(contentLocation, "blue");
                createBlockLabel(contentLocation);
            }

            return _groupElement;
        }


        private void createTextLabel(XY textLocation, out double textLabelWidth, out double textLabelheight) {
            string text = _contextData.TextLabel;
            if (_contextData.FlowDirection == FlowDirectionType.Vertical) {
                text = text.Replace("\n", " ").Replace(@"\P", " ").Replace("^J", " ");
            }

            var textStyle = _mlProps.TextStyle;
            var textAngle = _mlProps.TextAngle;
            var textColor = _mlProps.TextColor;

            var textLeftAttachment = _mlProps.TextLeftAttachment;
            var textRightAttachment = _mlProps.TextRightAttachment;
            var textTopAttachment = _mlProps.TextTopAttachment;
            var textBottomAttachment = _mlProps.TextBottomAttachment;

            double textHeight = _mlProps.TextHeight;
            double textSize = TextUtils.GetTextSize(textHeight);
 
            //  Rotation is performed first, must be positive.
            double rot = _contextData.TextRotation * 180 / Math.PI;

            double textLocationY = textLocation.Y;
            if (_contextData.FlowDirection != FlowDirectionType.Vertical) {
                textLocationY -= textHeight;
            }

            string textAnchor = TextUtils.HorizontalAlignmentToTextAnchor(_mlProps.TextAttachmentPoint);
            TextUtils.StyleToValues(textStyle, textSize, out string fontFamily, out double fontSize, out bool bold, out bool italic);
            var tspans = TextUtils.ConvertMTextToHtml(textLocation.X, textLocationY, text, textSize, textStyle);

            var textElement = new TextElement()
                    .WithFont(fontFamily, fontSize * 1.5, bold, italic)
                    .WithTextAnchor(textAnchor)
                    .WithTspans(tspans)
                    .WithFill(ColorUtils.GetHtmlTextColor(_multiLeader, textColor))
                    .ReverseY(_ctx.ConversionOptions.ReverseY)
                    .AddRotate(rot, textLocation.X, textLocation.Y);
            _groupElement.Children.Add(textElement);

            textLabelWidth = 60;
            textLabelheight = 3 * textHeight;
        }


        private void createTextFrame(XY contentBasePoint, XY contentLocation, XY direction, double textLabelWidth, double textLabelHeight) {
            if (_mlProps.TextFrame) {
                double textHeight = _mlProps.TextHeight;

                double rot = _contextData.TextRotation * 180 / Math.PI;

                double lg = _mlProps.LandingGap;
                double frameLeft = contentBasePoint.X;
                double frameWidth;
                if (_mlProps.TextAttachmentPoint == TextAttachmentPointType.Right) {
                    frameWidth = (contentLocation - contentBasePoint).Dot(direction) + lg;
                }
                else {
                    frameWidth = textLabelWidth + 2 * lg;
                }

                //  Text-position offset perpendicular to direction.
                double textUp = contentBasePoint.Y + (contentLocation - contentBasePoint).Dot(direction.Rotate90());
                double frameTop;
                switch (_mlProps.FlowDirection) {
                case FlowDirectionType.Horizontal:
                default:
                    frameTop = textUp + lg;
                    break;
                case FlowDirectionType.Vertical:
                    frameTop = textUp + lg + textHeight;
                    break;
                }

                double boundaryHeight = _contextData.BoundaryHeight;
                double boundaryWidth = _contextData.BoundaryWidth;

                double frameHeight = textLabelHeight + 2 * lg;
                double frameBottom = frameTop - frameHeight;
                var rectangleElement = new RectangleElement() {
                    X = frameLeft,
                    Y = frameBottom,
                    Width = frameWidth,
                    Height = frameHeight
                };
                rectangleElement
                    .AddRotate(rot, contentBasePoint.X, contentBasePoint.Y)
                    .WithStroke(_multiLeaderLineColor)
                    .WithStrokeWidth(_multiLeaderLineWidth);
                _groupElement.Children.Add(rectangleElement);

                //CreateDebugPoint(new XY(contentBasePoint.X, textUp), "aqua");
                //CreateDebugPoint(contentBasePoint, "aqua");
            }
        }


        private void createBlockLabel(XY contentLocation) {
            var blockContent = _mlProps.BlockContent;
            var blockContentColor = _mlProps.BlockContentColor;
            var blockContentScale = _mlProps.BlockContentScale;
            var blockContentRotation = _mlProps.BlockContentRotation;
            var blockContentConnection = _mlProps.BlockContentConnection;

            //  TODO Does contentLocation consider blockContentConnection?

            var blockLocX = contentLocation.X;
            var blockLocY = contentLocation.Y;
            var blockName = Utils.CleanBlockName(blockContent.Name);
            _groupElement.Children.Add(new UseElement().WithXY(blockLocX, blockLocY).WithGroupId(blockName));
        }


        private string buildPropertiesComment() {
            StringBuilder commentSb = new StringBuilder();

            commentSb.AppendLine("MultiLeader");

            //  Draw points for test
            XYZ contentBasePoint = _contextData.ContentBasePoint;
            XYZ basePoint = _contextData.BasePoint;
            XYZ textlocation = _contextData.TextLocation;

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
            commentSb.Append("Drehung/Rotation: ").AppendLine(_contextData.TextRotation.ToString());
            //Textzeilenabstand
            //Zuordnungstyp
            commentSb.AppendLine("Zuordungstyp (TextAttachmentrType):");
            commentSb.Append("    ML: ").Append(_multiLeader.TextAttachmentDirection).Append(", MLS: ").Append(_multiLeader.Style.TextAttachmentDirection).Append(", LR0: ").Append(_contextData.LeaderRoots[0].TextAttachmentDirection).AppendLine();
            commentSb.Append("Zuordnungstyp/AttachmentType: ").AppendLine(_contextData.BlockContentConnection.ToString());
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


        protected void CreateDebugPoint(XY point, string color) {
            if (_ctx.ConversionOptions.CreateDebugElements) {
                _groupElement.Children.Add(new CircleElement() { Cx = point.X, Cy = point.Y, R = 0.25 }.WithStroke(color));
            }
        }
    }
}