#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;
using CSMath;
using System.Xml.Linq;


namespace ACadSvg {

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="Hatch"/> entity.
    /// The <see cref="Hatch"/> entity is converted into one complex <i>path</i> element
    /// for the shape. The contour of the shape is invisible. The pattern to fill the
    /// shape is referenced by the <i>fill</i> attribute linking to a <i>pattern</i>
    /// member in the <i>defs</i> section.
    /// </summary>
    /// <remarks><para>
    /// The pattern to fill the shape either has been created before and is found in
    /// the the <see cref="ConversionContext.BlocksInDefs"/> list in the
    /// <see cref="ConversionContext"/> or has to be created during the intializtion
    /// of this object and stored in the <see cref="ConversionContext.BlocksInDefs"/> list.
    /// </para></remarks>
    internal class HatchSvg : EntitySvg {

        private Hatch _hatch;
        private ConversionContext _ctx;


        /// <summary>
		/// Initializes a new instance of the <see cref="CircleSvg"/> class
		/// for the specified <see cref="Hatch"/> entity.
        /// </summary>
        /// <param name="hatch">The <see cref="Hatch"/> entity to be converted.</param>
        /// <param name="ctx">The conversion context; the <see cref="ConversionContext.BlocksInDefs"/>
        /// in the <see cref="ConversionContext"/> also contains patterns to be used
        /// to fill the hatch shape.
        /// </param>
        public HatchSvg(Entity hatch, ConversionContext ctx) {
            _hatch = (Hatch)hatch;
            SetStandardIdAndClassIf(hatch, ctx);

            _ctx = ctx;

            ToSvgElement();
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            var path = new PathElement();

            foreach (Hatch.BoundaryPath boundaryPath in _hatch.Paths) {

                IList<Hatch.BoundaryPath.Edge> edges = sortHatchEdges2(boundaryPath.Edges, out Dictionary<Hatch.BoundaryPath.Edge, bool> edgeReverse);

                int ellarcCounter = 0;
                bool first = true;

                foreach (var edge in edges) {
                    bool reverse = edgeReverse[edge];
                    switch (edge.Type) {
                    case Hatch.BoundaryPath.EdgeType.CircularArc:
                        Hatch.BoundaryPath.Arc arc = (Hatch.BoundaryPath.Arc)edge;

                        if (first) {
                            path.AddMoveAndArc(arc.Center.X, arc.Center.Y, arc.StartAngle, arc.EndAngle, arc.Radius, arc.CounterClockWise);
                        }
                        else {
                            path.AddLineAndArc(arc.Center.X, arc.Center.Y, arc.StartAngle, arc.EndAngle, arc.Radius, arc.CounterClockWise);
                        }

                        first = false;
                        break;

                    case Hatch.BoundaryPath.EdgeType.EllipticArc:
                        Hatch.BoundaryPath.Ellipse ellarc = (Hatch.BoundaryPath.Ellipse)edge;

                        double largeAxis = ellarc.MajorAxisEndPoint.GetLength();

                        if (first) {
                            path.AddMoveAndArc(ellarc.Center.X, ellarc.Center.Y, ellarc.StartAngle, ellarc.EndAngle, largeAxis, ellarc.CounterClockWise);
                        }
                        else {
                            path.AddLineAndArc(ellarc.Center.X, ellarc.Center.Y, ellarc.StartAngle, ellarc.EndAngle, largeAxis, ellarc.CounterClockWise);
                        }

                        ellarcCounter++;

                        first = false;
                        break;

                    case Hatch.BoundaryPath.EdgeType.Line:
                        Hatch.BoundaryPath.Line line = (Hatch.BoundaryPath.Line)edge;
                        if (!reverse) {
                            if (first) {
                                path.AddMove(line.Start.X, line.Start.Y);
                            }
                            else {
                                path.AddLine(line.Start.X, line.Start.Y);
                            }

                            first = false;
                            path.AddLine(line.End.X, line.End.Y);
                        }
                        else {
                            if (first) {
                                path.AddMove(line.End.X, line.End.Y);
                            }
                            else {
                                path.AddLine(line.End.X, line.End.Y);
                            }

                            first = false;
                            path.AddLine(line.Start.X, line.Start.Y);
                        }
                        break;

                    case Hatch.BoundaryPath.EdgeType.Polyline:
                        Hatch.BoundaryPath.Polyline polyline = (Hatch.BoundaryPath.Polyline)edge;
                        List<XY> vertices = new List<XY>(polyline.Vertices);
                        if (reverse) {
                            vertices.Reverse();
                        }
                        foreach (var vertex in vertices) {
                            if (first) {
                                path.AddMove(vertex.X, vertex.Y);
                            }
                            else {
                                path.AddLine(vertex.X, vertex.Y);
                            }

                            first = false;
                        }
                        break;

                    case Hatch.BoundaryPath.EdgeType.Spline:
                        Hatch.BoundaryPath.Spline spline = (Hatch.BoundaryPath.Spline)edge;
                        List<XYZ> cp = new List<XYZ>(spline.ControlPoints);
                        if (reverse) {
                            cp.Reverse();
                        }
                        foreach (var loc in cp) {
                            XY vertex = new XY(loc.X, loc.Y);
                            if (first) {
                                path.AddMove(vertex.X, vertex.Y);
                            }
                            else {
                                path.AddLine(vertex.X, vertex.Y);
                            }
                            first = false;
                        }
                        break;

                    default:
                        break;
                    }
                }
            }

            string hatchPatternName = _hatch.Pattern.Name;

            //  If we did not add the referenced pattern before ...
            //  -  Add well-known ANSI31 or Concrete Pattern or
            //  -  create new pattern from data
            if (!_ctx.BlocksInDefs.Contains(hatchPatternName)) {
                PatternSvg patternSvg = null;

                if (_hatch.Pattern.Name == "ANSI31") {
                    patternSvg = new PatternSvg();
                    patternSvg.ID = _hatch.Pattern.Name;
                    patternSvg.Width = "16";
                    patternSvg.Height = "16";
                    patternSvg.Elements.Add(XElement.Parse($"<path d=\"M -4,4 l 8,-8 M 0,16 l 16,-16 M 12,20 l 8,-8\"></path>"));
                }
                else if (hatchPatternName == "CONC") {
                    patternSvg = new PatternSvg();
                    patternSvg.ID = _hatch.Pattern.Name;
                    patternSvg.Width = $"{_hatch.Pattern.Scale}%";
                    patternSvg.Height = $"{_hatch.Pattern.Scale}%";
                    patternSvg.Rotation = 45; // TODO: patternTransform

                    string pattern = Resources.concrete_pattern;
                    using (StringReader stringReader = new StringReader(pattern)) {
                        string line;
                        while ((line = stringReader.ReadLine()) != null) {
                            patternSvg.Elements.Add(XElement.Parse(line));
                        }
                    }

                    //  TODO: Check pattern name for concrete
                    /*
					hatchPatternSb.AppendLine($"<pattern id=\"{hatchPatternName}\" x=\"0\" y=\"0\" width=\"{_hatch.Pattern.Scale}%\" height=\"{_hatch.Pattern.Scale}%\" patternTransform=\"rotate(45)\">");
                    hatchPatternSb.AppendLine(Resources.concrete_pattern);
                    hatchPatternSb.AppendLine("</pattern>");
                    */
                }
                else {
                    //foreach (HatchPattern.Line line in _hatch.Pattern.Lines) {
                    //    hatchPatternSb.AppendLine($"    <path transform=\"rotate({_hatch.Pattern.Angle})\" d=\"M {line.BasePoint.X} {line.BasePoint.Y} L {-line.Offset.X} {line.Offset.Y}\"></path>".Replace(",", "."));
                    //    hatchPatternSb.AppendLine($"    <path transform=\"rotate({45})\" d=\"M {line.BasePoint.X} {line.BasePoint.Y} L {-line.Offset.X * 2} {line.Offset.Y * 2}\" fill=\"url(#" + _hatch.Pattern.Name + ")\"></path>".Replace(",", "."));
                    //}
                }

                if (patternSvg != null) {
                    _ctx.BlocksInDefs.Items.Add(patternSvg);
                }
            }

            return path.Close().WithID(ID).WithStroke("none").WithFillURL($"#{hatchPatternName}");
        }


        private static IList<Hatch.BoundaryPath.Edge> sortHatchEdges2(List<Hatch.BoundaryPath.Edge> edges, out Dictionary<Hatch.BoundaryPath.Edge, bool> edgeReverse) {
            edgeReverse = new Dictionary<Hatch.BoundaryPath.Edge, bool>();

            if (edges.Count < 2) {
                edgeReverse.Add(edges[0], false);
                return edges;
            }

            getStartAndEnd(edges[0], out XY startPoint0, out XY endPoint0);
            getStartAndEnd(edges[1], out XY startPoint1, out XY endPoint1);
            XY endPoint;

            if ((startPoint1 - startPoint0).GetLength() < 0.001 || (endPoint1 - startPoint0).GetLength() < 0.001) {
                edgeReverse.Add(edges[0], true);
                endPoint = startPoint0;
            }
            else {
                edgeReverse.Add(edges[0], false);
                endPoint = endPoint0;
            }

            for (int i = 1; i < edges.Count; i++) {
                var edge = edges[i];
                getStartAndEnd(edges[i], out XY startPointi, out XY endPointi);
                if ((startPointi - endPoint).GetLength() <= (endPointi - endPoint).GetLength()) {
                    edgeReverse.Add(edge, false);
                    endPoint = endPointi;
                }
                else {
                    edgeReverse.Add(edge, true);
                    endPoint = endPointi;
                }
            }

            return edges;
        }


        private static void getStartAndEnd(Hatch.BoundaryPath.Edge edge, out XY startPoint, out XY endPoint) {
            startPoint = XY.Zero;
            endPoint = XY.Zero;
            switch (edge.Type) {
            case Hatch.BoundaryPath.EdgeType.CircularArc:
                Hatch.BoundaryPath.Arc arc = (Hatch.BoundaryPath.Arc)edge;

                var fa = arc.CounterClockWise ? 1 : -1;
                var saa = fa * arc.StartAngle;
                var eaa = fa * arc.EndAngle;

                getArcStartAndEnd(arc.Center, saa, eaa, arc.Radius, arc.CounterClockWise, out startPoint, out endPoint);
                break;

            case Hatch.BoundaryPath.EdgeType.EllipticArc:
                Hatch.BoundaryPath.Ellipse ellarc = (Hatch.BoundaryPath.Ellipse)edge;

                var f = ellarc.CounterClockWise ? 1 : -1;

                double largeAxis = ellarc.MajorAxisEndPoint.GetLength();
                double smallAxis = largeAxis * ellarc.MinorToMajorRatio;
                var ella = ellarc.MajorAxisEndPoint.GetAngle();
                var sae = ella + f * ellarc.StartAngle;
                var eae = ella + f * ellarc.EndAngle;

                if (Math.Round(ellarc.MinorToMajorRatio, 1) != 1) {
                }

                getArcStartAndEnd(ellarc.Center, sae, eae, largeAxis, ellarc.CounterClockWise, out startPoint, out endPoint);
                break;

            case Hatch.BoundaryPath.EdgeType.Line:
                Hatch.BoundaryPath.Line line = (Hatch.BoundaryPath.Line)edge;
                startPoint = line.Start;
                endPoint = line.End;
                break;
            case Hatch.BoundaryPath.EdgeType.Polyline:
                Hatch.BoundaryPath.Polyline polyline = (Hatch.BoundaryPath.Polyline)edge;
                startPoint = polyline.Vertices[0];
                endPoint = polyline.Vertices[polyline.Vertices.Count - 1];
                break;
            case Hatch.BoundaryPath.EdgeType.Spline:
                Hatch.BoundaryPath.Spline spline = (Hatch.BoundaryPath.Spline)edge;
                XYZ start = spline.ControlPoints[0];
                startPoint = new XY(start.X, start.Y);
                XYZ end = spline.ControlPoints[spline.ControlPoints.Count - 1];
                endPoint = new XY(end.X, end.Y);
                break;
            }
        }


        private static void getArcStartAndEnd(XY c, double sa, double ea, double r, bool counterClockWise, out XY startPoint, out XY endPoint) {
            var mx = c.X + r * Math.Cos(sa);
            var my = c.Y + r * Math.Sin(sa);
            var ex = c.X + r * Math.Cos(ea);
            var ey = c.Y + r * Math.Sin(ea);

            startPoint = new XY(mx, my);
            endPoint = new XY(ex, ey);
        }
    }
}