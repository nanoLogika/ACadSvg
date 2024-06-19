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

    internal class DimensionRadiusSvg : DimensionSvg {

        DimensionRadius _radDim;


        public DimensionRadiusSvg(Entity entity, ConversionContext ctx) : base(entity, ctx) {
            _radDim = (DimensionRadius)entity;
            _defaultPostFix = "R<>";
        }


        public override SvgElementBase ToSvgElement() {
            CreateGroupElement();

            XY angleVertex = _radDim.AngleVertex.ToXY();
            XY dp = _radDim.DefinitionPoint.ToXY();
            XY textMid = _radDim.TextMiddlePoint.ToXY();
            XY dimDir = (angleVertex - dp).Normalize();

            BlockRecord arrowHead = _dimProps.ArrowHeadBlock2;

            XY textOnDimLin = angleVertex + dimDir * dimDir.Dot(textMid - angleVertex);
            bool textOutside = (textOnDimLin - dp).GetLength() > (angleVertex - dp).GetLength();
            bool arrowOutside = textOutside;

            //  Debug+
            CreateDebugPoint(textMid, "blue");
            CreateDebugPoint(textOnDimLin, "green");
            CreateDebugPoint(dp, "red");
            CreateDebugPoint(angleVertex, "aqua");
            //  -Debug

            //  Text
            CreateTextElement(textOnDimLin, dimDir.GetAngle(), out double textLen);

            //  Dimenion line and Arrow
            XY arrowDir = dimDir * (arrowOutside ? -1 : 1);

            //  Dimension line ends at arrow when inside, otherwise at circle line,
            //  starts at dp or when text is inside half textLen beyond. 
            XY dla = textOutside ? dp : dp - dimDir * textLen / 2;
            XY dle = arrowOutside ? angleVertex : angleVertex - dimDir * _arrowSize;
            CreateDimensionLine(dla, dle);

            CreateArrowHead(arrowHead, angleVertex, arrowDir);
            if (arrowOutside || textOutside) {
                //  Dimension line extension when arrow or text is outsize
                CreateDimensionLineExtension(angleVertex, textOnDimLin, dimDir, _arrowSize, textOutside, arrowOutside, textLen);
            }

            return _groupElement;
        }
    }
}
