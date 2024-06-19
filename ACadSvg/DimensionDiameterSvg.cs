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


namespace ACadSvg {
    internal class DimensionDiameterSvg : DimensionSvg {

        DimensionDiameter _diaDim;


        public DimensionDiameterSvg(Entity entity, ConversionContext ctx) : base(entity, ctx){
            _diaDim = (DimensionDiameter)entity;
            _defaultPostFix = "∅<>";
        }


        public override SvgElementBase ToSvgElement() {
            CreateGroupElement();

            //               ∅0000
            //      (<---------x--------->)         !arrowOutside + !textOutside + textCenter
            //                     ∅0000
            //      (              ------>)         !arrowOutside + !textOutside + !textCenter 
            //                               ∅0000
            //   -->(                     )<------  arrowOutside

            XY angleVertex = _diaDim.AngleVertex.ToXY();
            XY dp = _diaDim.DefinitionPoint.ToXY();
            XY center = (angleVertex + dp) / 2;
            XY textMid = _diaDim.TextMiddlePoint.ToXY();
            XY dimDir = (angleVertex - dp).Normalize();

            BlockRecord arrowHead1 = _dimProps.ArrowHeadBlock1;
            BlockRecord arrowHead2 = _dimProps.ArrowHeadBlock2;

            XY textOnDimLin = angleVertex + dimDir * dimDir.Dot(textMid - angleVertex);
            bool textCenter = textOnDimLin.Equals(center);
            bool textOutside = (textOnDimLin - center).GetLength() > (dp - center).GetLength();
            bool arrowOutside = textOutside;

            //  Debug+
            //CreateDebugPoint(textOnDimLin, "blue");
            //CreateDebugPoint(textMid, "blue");
            //CreateDebugPoint(dp, "red");
            //CreateDebugPoint(angleVertex, "aqua");

            //XY textUp = textOnDimLin + (textMid - textOnDimLin).Normalize() * 100;
            //_groupElement.Children.Add(new PathElement().AddLine(textOnDimLin.X, textOnDimLin.Y, textMid.X, textMid.Y).WithStroke("blue").WithStrokeWidth(0.25));
            //_groupElement.Children.Add(new PathElement().AddLine(textOnDimLin.X, textOnDimLin.Y, textUp.X, textUp.Y).WithStroke("blue").WithStrokeWidth(0.25));
            //  -Debug

            //  Text
            CreateTextElement(textOnDimLin, dimDir.GetAngle(), out double textLen);

            //  Dimenion line and Arrow
            XY arrDir = dimDir * (arrowOutside ? -1 : 1);

            //  Dimension line
            //  ends at arrow when inside, otherwise at circle line,
            //  starts at dp or when text is inside half textLen beyond. 
            XY dla = arrowOutside ? dp : dp + dimDir * _arrowSize;
            XY dle = arrowOutside ? angleVertex : (textCenter ? angleVertex - dimDir * _arrowSize : textOnDimLin + dimDir * textLen / 2);
            CreateDimensionLine(dla, dle);

            if (textCenter || textOutside) {
                //  Arrow at angle vertex is drawn only when text is centered or outside
                CreateArrowHead(arrowHead1, angleVertex, arrDir);
                if (arrowOutside) {
                    //  -->(
                    CreateDimensionLineExtension(angleVertex, XY.Zero, dimDir, _arrowSize, false, arrowOutside, 0);
                }
            }

            CreateArrowHead(arrowHead2, dp, -arrDir);
            if (arrowOutside || textOutside) {
                //  Dimension line extension when arrow or text is outside
                //  )<-------
                CreateDimensionLineExtension(dp, textOnDimLin, -dimDir, _arrowSize, textOutside, arrowOutside, textLen);
            }

            return _groupElement;
        }
    }
}