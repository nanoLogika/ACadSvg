#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using System.Text;

using ACadSharp.Entities;

using CSMath;

using SvgElements;

using static ACadSharp.Entities.Hatch.BoundaryPath;


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


        /// <summary>
		/// Initializes a new instance of the <see cref="CircleSvg"/> class
		/// for the specified <see cref="Hatch"/> entity.
        /// </summary>
        /// <param name="hatch">The <see cref="Hatch"/> entity to be converted.</param>
        /// <param name="ctx">The conversion context; the <see cref="ConversionContext.BlocksInDefs"/>
        /// in the <see cref="ConversionContext"/> also contains patterns to be used
        /// to fill the hatch shape.
        /// </param>
        public HatchSvg(Entity hatch, ConversionContext ctx) : base(ctx) {
            _hatch = (Hatch)hatch;
            createHatchPattern();

            SetStandardIdAndClassIf(hatch, ctx);
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            var path = new PathElement();
            StringBuilder ellarcs = new StringBuilder();

            foreach (Hatch.BoundaryPath boundaryPath in _hatch.Paths) {

                IList<Hatch.BoundaryPath.Edge> edges = sortHatchEdges2(boundaryPath.Edges, out Dictionary<Hatch.BoundaryPath.Edge, bool> edgeReverse);

                bool first = true;

                foreach (var edge in edges) {
                    bool reverse = edgeReverse[edge];
                    switch (edge.Type) {
                    case Hatch.BoundaryPath.EdgeType.CircularArc:
                        Hatch.BoundaryPath.Arc arc = (Hatch.BoundaryPath.Arc)edge;

                        double arcStartAngle;
                        double arcEndAngle;
                        bool arcCounterClockWise;
                        if (reverse) {
                            arcStartAngle = arc.EndAngle;
                            arcEndAngle = arc.StartAngle;
                            arcCounterClockWise = !arc.CounterClockWise;
                        }
                        else {
                            arcStartAngle = arc.StartAngle;
                            arcEndAngle = arc.EndAngle;
                            arcCounterClockWise = arc.CounterClockWise;
                        }

                        Utils.ArcToPath(
                            path, first,
                            arc.Center, arc.Radius, arcStartAngle, arcEndAngle, arcCounterClockWise);

                        first = false;
                        break;

                    case Hatch.BoundaryPath.EdgeType.EllipticArc:
                        Hatch.BoundaryPath.Ellipse ellarc = (Hatch.BoundaryPath.Ellipse)edge;

                        double ellStartAngle;
                        double ellEndAngle;
                        bool counterClockWise;
                        if (reverse) {
                            ellStartAngle = ellarc.EndAngle;
                            ellEndAngle = ellarc.StartAngle;
                            counterClockWise = !ellarc.CounterClockWise;
                        }
                        else {
                            ellStartAngle = ellarc.StartAngle;
                            ellEndAngle = ellarc.EndAngle;
                            counterClockWise = ellarc.IsCounterclockwise;
                        }
                        Utils.EllipseArcToPath(
                            path, first,
                            ellarc.Center, ellarc.MajorAxisEndPoint, ellarc.MinorToMajorRatio,
                            ellStartAngle, ellEndAngle, counterClockWise);

                        PathElement myPath = new PathElement()
                            .WithClass((counterClockWise ? "CCW" : "CW") + (reverse ? "-r" : ""))
                            .WithStroke(counterClockWise ? reverse ? "green" : "red" : reverse ? "yellow" : "blue")
                            .WithStrokeWidth(0.5)
                            .WithFill("none");
                        Utils.EllipseArcToPath(
                            myPath, true,
                            ellarc.Center, ellarc.MajorAxisEndPoint, ellarc.MinorToMajorRatio,
                            ellStartAngle, ellEndAngle, counterClockWise);

                        ellarcs.AppendLine(myPath.ToString());

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
                        List<XYZ> vertices = new List<XYZ>(polyline.Vertices);
                        List<double> bulges = new List<double>(polyline.Bulges);
                        XY lastVertexLocation = XY.Zero;
                        double lastVertexBulge = 0;
                        if (reverse) {
                            vertices.Reverse();
                            bulges.Reverse();
                        }
                        int bulgIx = 0;
                        foreach (var vertex in vertices) {
                            XY vertexLocation = vertex.ToXY();
                            if (first) {
                                path.AddMove(vertexLocation.X, vertexLocation.Y);
                            }
                            else {
                                //path.AddLine(vertex.X, vertex.Y);
                                addLineOrBulge(path, vertexLocation, lastVertexLocation, lastVertexBulge);
                            }

                            lastVertexLocation = vertexLocation;
                            lastVertexBulge = bulges[bulgIx];
                            first = false;
                            bulgIx++;
                        }

                        if (polyline.IsClosed) {
                            addLineOrBulge(path, vertices[0].ToXY(), lastVertexLocation, lastVertexBulge);
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

            if (ellarcs.Length > 0) {
                System.Diagnostics.Debug.WriteLine(this.ID);
                System.Diagnostics.Debug.WriteLine(ellarcs);
            }

            string hatchPatternName = _hatch.Pattern.Name;
            return path
                .Close()
                .WithID(ID)
                .WithClass(Class)
                .WithStroke("aqua")
                .WithFillURL($"#{hatchPatternName}");
        }


        private static void addLineOrBulge(PathElement pathElement, XY vertexLocation, XY lastVertexLocation, double lastVertexBulge) {
            if (lastVertexBulge != 0) {
                double l = vertexLocation.DistanceFrom(lastVertexLocation);
                double d = l / 2;
                double sagitta = d * Math.Abs(lastVertexBulge);
                double r = (Math.Pow(sagitta, 2) + Math.Pow(d, 2)) / 2 / sagitta;
                bool lf = r < sagitta;
                bool sf = lastVertexBulge > 0;

                pathElement.AddArc(r, r, 0, lf, sf, vertexLocation.X, vertexLocation.Y);
            }
            else {
                pathElement.AddLine(vertexLocation.X, vertexLocation.Y);
            }
        }


        private void createHatchPattern() {
            string hatchPatternName = _hatch.Pattern.Name;
            
            //  If we added the referenced pattern before ...
            if (_ctx.BlocksInDefs.Contains(hatchPatternName)) {
                return;
            }

            //  otherwise; create the pattern
            string patternColor = ColorUtils.GetHtmlColor(_hatch, _hatch.Color);
            PatternSvg patternSvg = new PatternSvg(_hatch.Pattern, patternColor, _ctx);
 
            //  If the pattern has been successfully created add it into the defs section.
            if (patternSvg.Valid) {
                _ctx.BlocksInDefs.Items.Add(patternSvg);
            }
        }


        private static IList<Hatch.BoundaryPath.Edge> sortHatchEdges2(IList<Hatch.BoundaryPath.Edge> edges, out Dictionary<Hatch.BoundaryPath.Edge, bool> edgeReverse) {
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
                diagnose(0, startPoint0, endPoint0, true, false, edges[0]);
            }
            else {
                edgeReverse.Add(edges[0], false);
                endPoint = endPoint0;
                diagnose(0, startPoint0, endPoint0, false, false, edges[0]);
            }

            for (int i = 1; i < edges.Count; i++) {
                var edge = edges[i];
                getStartAndEnd(edges[i], out XY startPointi, out XY endPointi);

                bool gap = (startPointi - endPoint).GetLength() > 0.001 && (endPointi - endPoint).GetLength() > 0.001;

                if ((startPointi - endPoint).GetLength() <= (endPointi - endPoint).GetLength()) {
                    edgeReverse.Add(edge, false);
                    endPoint = endPointi;
                    diagnose(i, startPointi, endPointi, false, gap, edge);
                }
                else {
                    edgeReverse.Add(edge, true);
                    endPoint = endPointi;
                    diagnose(i, startPointi, endPointi, true, gap, edge);
                }
            }

            return edges;
        }


        private static void diagnose(int i, XY startPoint0, XY endPoint0, bool reverse, bool isgap, Edge edge) {
            string rev = reverse ? "reverse" : "x";
            string ellProps = string.Empty;
            string gap = isgap ? "gap" : "x";
            if (edge is Hatch.BoundaryPath.Ellipse ell) {
                double rot = Math.Atan2(ell.MajorAxisEndPoint.Y, ell.MajorAxisEndPoint.X) * 180.0 / Math.PI;
                string ccw = ell.IsCounterclockwise ? "CCW" : "CW";
                ellProps = $"{ell.Center.X} {ell.Center.Y} {ell.MajorAxisEndPoint.X} {ell.MajorAxisEndPoint.Y} {ell.MinorToMajorRatio}  {rot} {ell.StartAngle * 180 / Math.PI} {ell.EndAngle * 180 / Math.PI} {ccw}";
            }
            System.Diagnostics.Debug.WriteLine($"{i} {startPoint0.X} {startPoint0.Y} {endPoint0.X} {endPoint0.Y} {rev} {gap} {ellProps}");
        }


        private static void getStartAndEnd(Hatch.BoundaryPath.Edge edge, out XY startPoint, out XY endPoint) {
            startPoint = XY.Zero;
            endPoint = XY.Zero;
            switch (edge.Type) {
            case Hatch.BoundaryPath.EdgeType.CircularArc:
                Hatch.BoundaryPath.Arc arc = (Hatch.BoundaryPath.Arc)edge;
                Utils.GetArcStartAndEnd(
                    arc.Center, arc.StartAngle, arc.EndAngle, arc.Radius, arc.CounterClockWise,
                    out startPoint, out endPoint);
                break;

            case Hatch.BoundaryPath.EdgeType.EllipticArc:
                Hatch.BoundaryPath.Ellipse ellarc = (Hatch.BoundaryPath.Ellipse)edge;
                Utils.GetEllipseArcStartAndEnd(
                    ellarc.Center, ellarc.MajorAxisEndPoint, ellarc.MinorToMajorRatio,
                    ellarc.StartAngle, ellarc.EndAngle, ellarc.IsCounterclockwise,
                    out startPoint, out endPoint);
                break;

            case Hatch.BoundaryPath.EdgeType.Line:
                Hatch.BoundaryPath.Line line = (Hatch.BoundaryPath.Line)edge;
                startPoint = line.Start;
                endPoint = line.End;
                break;

            case Hatch.BoundaryPath.EdgeType.Polyline:
                Hatch.BoundaryPath.Polyline polyline = (Hatch.BoundaryPath.Polyline)edge;
                startPoint = polyline.Vertices[0].ToXY();
                endPoint = polyline.Vertices[polyline.Vertices.Count - 1].ToXY();
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
    }
}