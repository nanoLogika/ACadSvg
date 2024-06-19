#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


using ACadSharp.Entities;
using ACadSharp.Objects;
using ACadSharp.Tables;
using ACadSharp;
using static ACadSharp.Objects.MultiLeaderAnnotContext;
using CSMath;

namespace ACadSvg.Extensions {

    /// <summary>
    /// Exposes all proprties required for the rendering of <see cref="MultiLeader"/>
    /// entity's.
    /// Property values are either provided by the associated <see cref="MultiLeaderStyle"/>
    /// object or may be overriden by properties of the <see cref="MultiLeader"/> and/or
    /// the <see cref="MultiLeaderAnnotContext"/>. The overriding of property values is
    /// controlled by the flags specified in the <see cref="MultiLeader.PropertyOverrideFlags"/>
    /// property.
    /// 
    /// TODO The <see cref="MultiLeader.ScaleFactor"/> is handled internally.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="MultiLeader.ScaleFactor"/> property is retrieved from the
    /// <see cref="DimensionStyle"/> and my be overridden by extended data of the entity.
    /// The value is used to scale the property values that have to be scaled.
    /// </para>
    /// </remarks>
    internal class MultiLeaderProperties : EntityProperties {

        private MultiLeader _multiLeader;
        private MultiLeaderStyle _style;
        private MultiLeaderAnnotContext _contextData;
        private MultiLeaderPropertyOverrideFlags _flags;
        private LeaderRoot _leaderRoot;
        private IDictionary<LeaderLine, LeaderLineProperties> _leaderLines = new Dictionary<LeaderLine, LeaderLineProperties>();


        /// <summary>
        /// Initialzes a new instance of the <see cref="MultiLeaderProperties"/> class.
        /// </summary>
        /// <param name="multiLeader"></param>
        public MultiLeaderProperties(MultiLeader multiLeader) {

            _multiLeader = multiLeader;
            _style = multiLeader.Style;
            _contextData = multiLeader.ContextData;
            _flags = multiLeader.PropertyOverrideFlags;

            //  Assume: There can exist only one LEADER, we have only one content
            _leaderRoot = _contextData.LeaderRoots[0];

            foreach (LeaderRoot leaderRoot in _contextData.LeaderRoots) {
                foreach (LeaderLine leaderLine in leaderRoot.Lines) {
                    _leaderLines.Add(leaderLine, new LeaderLineProperties(leaderLine, _multiLeader, _style, _flags));
                }
            }
        }


        public IDictionary<LeaderLine, LeaderLineProperties> LeaderLines {
            get { return _leaderLines; }
        }


        /// <summary>
        /// Gets the value of the <see cref="MultiLeaderStyle.PathType"/> property or the
        /// the value of the <see cref="MultiLeader.PathType"/> path type if overriding is
        /// specified by the <see cref="MultiLeaderPropertyOverrideFlags.PathType"/> flag.
        /// </summary>
        public MultiLeaderPathType PathType {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.PathType)) {
                    return _multiLeader.PathType;
                }
                return _style.PathType;
            }
        }


        /// <summary>
        /// Gets the value of the <see cref="MultiLeaderStyle.LineColor"/> property or the
        /// the value of the <see cref="MultiLeader.PathType"/> path type if overriding is
        /// specified by the <see cref="MultiLeaderPropertyOverrideFlags.PathType"/> flag.
        /// </summary>
        public Color LineColor {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.LineColor)) {
                    return _multiLeader.LineColor;
                }
                return _style.LineColor;
            }
        }


        public LineType LeaderLineType {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.LeaderLineType)) {
                    var lineType = _multiLeader.LineType;
                    return _multiLeader.LeaderLineType;
                }
                return _style.LeaderLineType;
            }
        }


        public LineweightType LineWeight {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.LeaderLineWeight)) {
                    return _multiLeader.LeaderLineWeight;
                }
                return _style.LeaderLineWeight;
            }
        }


        public bool EnableLanding {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.EnableLanding)) {
                    return _multiLeader.EnableLanding;
                }

                return _style.EnableLanding;
            }
        }


        public double LandingGap {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.LandingGap)) {
                    return _contextData.LandingGap;
                }

                return _style.LandingGap;
            }
        }


        public bool EnableDogleg {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.EnableDogleg)) {
                    return _multiLeader.EnableDogleg;
                }

                return _style.EnableDogleg;
            }
        }


        public double LandingDistance {
            get {
                if (_style == null || _multiLeader.LandingDistance != 0 && _flags.HasFlag(MultiLeaderPropertyOverrideFlags.LandingDistance)) {
                    return _leaderRoot.LandingDistance;
                }

                return _style.LandingDistance;
            }
        }


        public BlockRecord Arrowhead {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.Arrowhead)) {
                    return _multiLeader.Arrowhead;
                }
                return _style.Arrowhead;
            }
        }


        public double ArrowheadSize {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.ArrowheadSize)) {
                    return _multiLeader.ArrowheadSize;
                }
                return _style.ArrowheadSize;
            }
        }


        public LeaderContentType ContentType {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.ContentType)) {
                    return _multiLeader.ContentType;
                }

                return _style.ContentType;
            }
        }


        public TextStyle TextStyle {
            get {
                var styTS = _style == null ? null : _style.TextStyle;
                var cdTS = _contextData.TextStyle;

                if (cdTS != null && _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextStyle)) {
                    return _contextData.TextStyle;
                }
                if (styTS != null) {
                    return styTS;
                }

                return _multiLeader.TextStyle;
            }
        }


        public TextAngleType TextAngle {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextAngle)) {
                    return _multiLeader.TextAngle;
                }

                return _style.TextAngle;
            }
        }


        public TextAlignmentType TextAlignment {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextAlignment)) {
                    //  Can _multiLeader.TextAlignment have a different value?
                    var mlTextAlignment = _multiLeader.TextAlignment;
                    return _contextData.TextAlignment;
                }

                //  System.Diagnostics.Debug.Assert(_contextData.TextAlignment.Equals(_style.TextAlignment));
                return _style.TextAlignment;
            }
        }


        public Color TextColor {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextColor)) {
                    //  return _multiLeader.TextColor;
                    return _contextData.TextColor;
                }

                //  System.Diagnostics.Debug.Assert(_contextData.TextColor.Equals(_style.TextColor));
                return _style.TextColor;
            }
        }


        public double TextHeight {
            get {
                //  Text height hierarchy:
                //  Text height can only be defined in one place:
                //      -  in the dimension/leader style or
                //      -  in the text style.
                //  If the text height will not change or is disabled in the dimension or leader style,
                //  then it needs to be adjusted in the text style that the dimension or leader style uses.
                //  Zeroing the height in the text style will then allow the height to be defined in the dimension/leader style.

                double textHeight = _multiLeader.TextStyle.Height;
                //double styleTextHeight = _style.TextHeight;
                if (textHeight == 0 || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextHeight)) {
                    textHeight = _contextData.TextHeight;
                }

                return textHeight;
            }
        }


        public bool TextFrame {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextFrame)) {
                    return _multiLeader.TextFrame;
                }

                //  System.Diagnostics.Debug.Assert(_multiLeader.TextFrame == _style.TextFrame);
                return _style.TextFrame;
            }
        }


        //public bool EnableUseDefaultMText {
        //    get {
        //        if (_flags.HasFlag(MultiLeaderPropertyOverrideFlags.EnableUseDefaultMText)) {
        //            return _contextData.
        //        }
        //        return _style.En;
        //    }
        //}


        public BlockRecord BlockContent {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.BlockContent)) {
                    //  return _multiLeader.BlockContent;
                    return _contextData.BlockContent;
                }

                //  System.Diagnostics.Debug.Assert(_contextData.BlockContent == _style.BlockContent);
                return _style.BlockContent;
            }
        }


        public Color BlockContentColor {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.BlockContentColor)) {
                    //  return _multiLeader.BlockContentColor;
                    return _contextData.BlockContentColor;
                }

                //  System.Diagnostics.Debug.Assert(_contextData.BlockContentColor.Equals(_style.BlockContentColor));
                return _style.BlockContentColor;
            }
        }


        public XYZ BlockContentScale {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.BlockContentScale)) {
                    //return _multiLeader.BlockContentScale;
                    return _contextData.BlockContentScale;
                }

                //  System.Diagnostics.Debug.Assert(_contextData.BlockContentScale == _style.BlockContentScale);
                return _style.BlockContentScale;
            }
        }


        public double BlockContentRotation {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.BlockContentRotation)) {
                    //return _multiLeader.BlockContentRotation;
                    return _contextData.BlockContentRotation;
                }

                return _style.BlockContentRotation;
            }
        }


        public object BlockContentConnection {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.BlockContentConnection)) {
                    //  return _multiLeader.BlockContentConnection;
                    return _contextData.BlockContentConnection;
                }

                return _style.BlockContentConnection;
            }
        }


        public double ScaleFactor {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.ScaleFactor)) {
                    return _multiLeader.ScaleFactor;
                }

                return _style.ScaleFactor;
            }
        }


        public TextAttachmentType TextLeftAttachment {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextLeftAttachment)) {
                    //  Can _multiLeader.TextLeftAttachment have a different value?
                    return _contextData.TextLeftAttachment;
                }

                //  System.Diagnostics.Debug.Assert(_contextData.TextLeftAttachment == _style.TextLeftAttachment);
                return _style.TextLeftAttachment;
            }
        }


        public TextAttachmentType TextRightAttachment {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextRightAttachment)) {
                    // ?? _multiLeader.TextRightAttachment;
                    return _contextData.TextRightAttachment;
                }

                //  System.Diagnostics.Debug.Assert(_contextData.TextRightAttachment == _style.TextRightAttachment);
                return _style.TextRightAttachment;
            }
        }


        public TextAttachmentType TextTopAttachment {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextTopAttachment)) {
                    // ?? _multiLeader.TextTopAttachment;
                    return _contextData.TextTopAttachment;
                }

                //  System.Diagnostics.Debug.Assert(_contextData.TextTopAttachment == _style.TextTopAttachment);
                return _style.TextTopAttachment;
            }
        }


        public TextAttachmentType TextBottomAttachment {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextBottomAttachment)) {
                    // ?? _multiLeader.TextBottomAttachment;
                    return _contextData.TextBottomAttachment;
                }

                //  System.Diagnostics.Debug.Assert(_contextData.TextBottomAttachment == _style.TextBottomAttachment);
                return _style.TextBottomAttachment;
            }
        }


        /// <summary>
        /// Overridable? does style exist.
        /// </summary>        
        public TextAttachmentPointType TextAttachmentPoint {
            get {
                return _contextData.TextAttachmentPoint;
            }
        }


        public TextAttachmentDirectionType TextAttachmentDirection {
            get {
                if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.TextAttachmentDirection)) {
                    return _multiLeader.TextAttachmentDirection;
                }
                //  System.Diagnostics.Debug.Assert(_multiLeader.TextAttachmentDirection == _style.TextAttachmentDirection);
                return _style.TextAttachmentDirection;
            }
        }


        public FlowDirectionType FlowDirection {
            get {
                FlowDirectionType flowDirection = _contextData.FlowDirection;
                if (flowDirection != FlowDirectionType.ByStyle) {
                    return flowDirection;
                }
                //  TODO find out which MutileaderStyle property defines FlowDirection
                return FlowDirectionType.Horizontal;
            }
        }


        public class LeaderLineProperties {

            private LeaderLine _leaderLine;
            private MultiLeader _multiLeader;
            private MultiLeaderStyle _style;
            private MultiLeaderPropertyOverrideFlags _flags;

            public LeaderLineProperties(
                LeaderLine leaderLine,
                MultiLeader multiLeader,
                MultiLeaderStyle style,
                MultiLeaderPropertyOverrideFlags flags) {

                _leaderLine = leaderLine;
                _multiLeader = multiLeader;
                _style = style;
                _flags = flags;
            }

            public MultiLeaderPathType PathType {
                get {
                    if (_leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.PathType)) {
                        return _leaderLine.PathType;
                    }
                    if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.PathType)) {
                        return _multiLeader.PathType;
                    }

                    return _style.PathType;
                }
            }


            public LineType LeaderLineType {
                get {
                    if (_leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.LineType)) {
                        return _leaderLine.LineType;
                    }
                    if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.LeaderLineType)) {
                        return _multiLeader.LineType;
                    }

                    return _style.LeaderLineType;
                }
            }


            public Color LeaderLineColor {
                get {
                    if (_leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.LineColor)) {
                        return _leaderLine.LineColor;
                    }
                    if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.LineColor)) {
                        return _multiLeader.LineColor;
                    }

                    return _style.LineColor;
                }
            }


            public LineweightType LeaderLineWeight {
                get {
                    if (_leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.LineWeight)) {
                        return _leaderLine.LineWeight;
                    }
                    if (_style == null || _flags.HasFlag(MultiLeaderPropertyOverrideFlags.LeaderLineWeight)) {
                        return _multiLeader.LineWeight;
                    }

                    return _style.LeaderLineWeight;
                }
            }


            public BlockRecord ArrowHead {
                get {
                    var mlAh = _multiLeader.Arrowhead;
                    var styAh = _style == null ? null : _style.Arrowhead;
                    var lAh = _leaderLine.Arrowhead;

                    BlockRecord arrowhead = styAh;
                    if (styAh == null || (mlAh != null && _multiLeader.PropertyOverrideFlags.HasFlag(MultiLeaderPropertyOverrideFlags.Arrowhead))) {
                        arrowhead = mlAh;
                    }
                    if (arrowhead == null || (lAh != null && _leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.Arrowhead))) {
                        arrowhead = lAh;
                    }

                    return arrowhead;
                }
            }


            public double ArrowHeadSize {
                get {
                    var mlAhs = _multiLeader.ArrowheadSize;
                    var cdAhs = _multiLeader.ContextData.ArrowheadSize;
                    var styAhs = _style == null ? 0 :_style.ArrowheadSize;
                    var lAhs = _leaderLine.ArrowheadSize;
                    var arrowheadSize = styAhs;

                    if (cdAhs > 0 && _flags.HasFlag(MultiLeaderPropertyOverrideFlags.ArrowheadSize)) {
                        arrowheadSize = cdAhs;
                    }
                    if (lAhs > 0 && _leaderLine.OverrideFlags.HasFlag(LeaderLinePropertOverrideFlags.ArrowheadSize)) {
                        arrowheadSize = lAhs;
                    }

                    return arrowheadSize;
                }
            }
        }
    }
}
