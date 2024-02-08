using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using ACadSharp.Entities;

using CSMath;

using SvgElements;

namespace ACadSvg {

    internal class DimensionOrdinateSvg : DimensionSvg {

        private DimensionOrdinate _dimOrd;


        public DimensionOrdinateSvg(Entity entity, ConversionContext ctx) : base(entity, ctx) {
            _dimOrd = (DimensionOrdinate)entity;
            _defaultPostFix = "<>";
        }


        public override SvgElementBase ToSvgElement() {
            CreateGroupElement();

            XY fl = Utils.ToXY(_dimOrd.FeatureLocation);
            XY dp = Utils.ToXY(_dimOrd.DefinitionPoint);
            XY le = Utils.ToXY(_dimOrd.LeaderEndpoint);
            XY textMid = Utils.ToXY(_dimOrd.TextMiddlePoint);
            bool isXCoordinate = _dimOrd.IsOrdinateTypeX;

            XY landing;
            XY vertex;
            XY textOnDimLin;
            if (isXCoordinate) {
                double up = le.Y > fl.Y ? 1 : -1;
                XY dimDir = new XY(0, up);
                textOnDimLin = le + dimDir * dimDir.Dot(textMid - le);
                CreateTextElement(textOnDimLin, Math.PI / 2, out double textLen);
                landing = new XY(le.X, le.Y - 2 * up * _arrowSize);
                if (le.Y - 6 * up * _arrowSize > fl.Y) {
                    vertex = new XY(fl.X, le.Y - 4 * up * _arrowSize);
                }
                else {
                    vertex = new XY(fl.X, fl.Y + 2 * up * _arrowSize);
                }
                le.Y += up * textLen;
                fl.Y += up * _dimProps.ExtensionLineOffset;
            }
            else {
                double right = le.X > fl.X ? 1 : -2;
                XY dimDir = new XY(right, 0);
                textOnDimLin = le + dimDir * dimDir.Dot(textMid - le);
                CreateTextElement(textOnDimLin, 0, out double textLen);
                landing = new XY(le.X - 2 * right * _arrowSize, le.Y);
                if (le.X - 6 * right * _arrowSize > fl.X) {
                    vertex = new XY(le.X - 4 * right * _arrowSize, fl.Y);
                }
                else {
                    vertex = new XY(fl.X + 2 * right * _arrowSize, fl.Y);
                }
                le.X += right * textLen;
                fl.X += right * _dimProps.ExtensionLineOffset;
            }

            //  Debug+
            //CreateDebugPoint(textOnDimLin, "blue");
            //CreateDebugPoint(textMid, "blue");
            //CreateDebugPoint(fl, "aqua");
            //CreateDebugPoint(Utils.ToXY(_dimOrd.FeatureLocation), "aqua");
            //CreateDebugPoint(dp, "white");
            //CreateDebugPoint(le, "green");
            //CreateDebugPoint(Utils.ToXY(_dimOrd.LeaderEndpoint), "green");
            //CreateDebugPoint(vertex, "red");
            //CreateDebugPoint(landing, "magenta");
            //  -Debug

            _groupElement.Children.Add(new PathElement()
                .AddMove(fl.X, fl.Y)
                .AddLine(vertex.X, vertex.Y)
                .AddLine(landing.X, landing.Y)
                .AddLine(le.X, le.Y)
                .WithStroke(_dimensionLineColor)
                .WithStrokeWidth(_dimensionLineWidth));

            return _groupElement;
        }
    }
}
