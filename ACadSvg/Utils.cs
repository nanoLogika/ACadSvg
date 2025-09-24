#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Collections.Generic;

using ACadSharp;
using ACadSharp.Entities;

using CSMath;

using SvgElements;

using static ACadSharp.Entities.Hatch.BoundaryPath;


namespace ACadSvg {

    internal static class Utils {

        public static XY[] XYZToXYArray(XYZ[] xyzs) {
            XY[] xys = new XY[xyzs.Length];
            for (int i = 0; i < xyzs.Length; i++) {
                xys[i] = new XY(xyzs[i].X, xyzs[i].Y);
            }
            return xys;
        }


        public static XYZ[] XYToXYZArray(XY[] xys) {
            XYZ[] xyzsOut = new XYZ[xys.Length];
            for (int i = 0; i < xys.Length; i++) {
                xyzsOut[i] = new XYZ(xys[i].X, xys[i].Y, 0);
            }
            return xyzsOut;
        }


        public static double[] VerticesToArray(IList<XY> list) {
            List<double> result = new List<double>();

            foreach (XY v in list) {
                result.Add(v.X);
                result.Add(v.Y);
            }

            return result.ToArray();
        }


        public static double[] VerticesToArray(IList<XYZ> list) {
            List<double> result = new List<double>();

            foreach (XYZ v in list) {
                result.Add(v.X);
                result.Add(v.Y);
            }

            return result.ToArray();
        }


        public static double[] VerticesToArray(IList<LwPolyline.Vertex> list) {
            List<double> result = new List<double>();

            foreach (LwPolyline.Vertex v in list) {
                result.Add(v.Location.X);
                result.Add(v.Location.Y);
            }

            return result.ToArray();
        }


        public static double[] VerticesToArray(IList<Vertex> list) {
            List<double> result = new List<double>();

            foreach (Vertex v in list) {
                result.Add(v.Location.X);
                result.Add(v.Location.Y);
            }

            return result.ToArray();
        }


        public static string CleanBlockName(string name) {
            return name.Replace("_", "__").Replace(" ", "_");
        }


        public static string GetObjectType(Entity entity) {
            if (entity.ObjectType == ObjectType.UNLISTED) {
                return entity.ObjectName;
            }
            else {
                return entity.ObjectType.ToString();
            }
        }


        internal static double GetInfinity(Entity entity) {
            return Math.Max(
                entity.Document.Header.ModelSpaceExtMax.X - entity.Document.Header.ModelSpaceExtMin.X,
                entity.Document.Header.ModelSpaceExtMax.Y - entity.Document.Header.ModelSpaceExtMin.Y);
        }


        internal static ulong ReverseBytes(ulong value) {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }


        internal static void ArcToPath(
            PathElement path, bool move,
            XY arcCenter, double r,
            double startAngle, double endAngle, bool counterClockWise = true) {

            GetArcStartAndEnd(
                arcCenter, startAngle, endAngle, r, counterClockWise,
                out XY startPoint, out XY endPoint);

            bool largeArc = determineLargeArc(startAngle, endAngle);
            bool sweep = counterClockWise;

            if (move) {
                path.AddMoveAndArc(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, r, largeArc, sweep);
            }
            else {
                path.AddLineAndArc(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, r, largeArc, sweep);
            }
        }


        internal static void EllipseArcToPath(
            PathElement path, bool move,
            XY arcCenter, XY majorAxisEndPoint, double minorToMajorRatio,
            double startAngle, double endAngle, bool counterClockWise = true) {

            ////  Major axis vector and length
            ////  Rotation of major axis in degrees
            double rx = majorAxisEndPoint.GetLength();
            double ry = rx * minorToMajorRatio;
            double rot = Math.Atan2(majorAxisEndPoint.Y, majorAxisEndPoint.X) * 180.0 / Math.PI;

            GetEllipseArcStartAndEnd(arcCenter, majorAxisEndPoint, minorToMajorRatio,
                startAngle, endAngle, counterClockWise,
                out XY startPoint, out XY endPoint);

            double sweepAngle = endAngle - startAngle;
            bool largeArc = determineLargeArc(startAngle, endAngle);
            bool sweep = counterClockWise; // CCW = 1, CW = 0

            if (move) {
                path.AddMoveAndArc(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, rx, ry, rot, largeArc, sweep);
            }
            else {
                path.AddLineAndArc(startPoint.X, startPoint.Y, endPoint.X, endPoint.Y, rx, ry, rot, largeArc, sweep);
            }
        }


        internal static void GetArcStartAndEnd(
            XY center, double startAngle, double endAngle, double r, bool counterClockWise,
            out XY startPoint, out XY endPoint) {

            var fa = counterClockWise ? 1 : -1;
            var sa = fa * startAngle;
            var ea = fa * endAngle;

            startPoint = new XY(
                center.X + r * Math.Cos(sa),
                center.Y + r * Math.Sin(sa));
            endPoint = new XY(
                center.X + r * Math.Cos(ea),
                center.Y + r * Math.Sin(ea));
        }


        internal static void GetEllipseArcStartAndEnd(
            XY arcCenter, XY majorAxisEndPoint, double minorToMajorRatio,
            double startAngle, double endAngle, bool counterClockWise,
            out XY startPoint, out XY endPoint) {

            //  Major-axis vector and length
            //  Evaluate minor-axis vector and length
            double rx = majorAxisEndPoint.GetLength();
            double ry = rx * minorToMajorRatio;
            XY majorNorm = majorAxisEndPoint.Normalize();
            XY minorAxisEndPoint = new XY(-majorNorm.Y, majorNorm.X) * ry;

            //  Invert minor-axis vector for CW
            if (!counterClockWise) {
                minorAxisEndPoint = -minorAxisEndPoint;
            }

            startPoint = arcCenter +
                Math.Cos(startAngle) * majorAxisEndPoint +
                Math.Sin(startAngle) * minorAxisEndPoint;

            endPoint = arcCenter +
                Math.Cos(endAngle) * majorAxisEndPoint +
                Math.Sin(endAngle) * minorAxisEndPoint;
        }


        private static bool determineLargeArc(double startAngle, double endAngle) {
            if (startAngle < 0) {
                startAngle += 2 * Math.PI;
            }
            if (endAngle < 0) {
                endAngle += 2 * Math.PI;
            }
            if (startAngle > endAngle) {
                return endAngle - startAngle - 2 * Math.PI > Math.PI;
            }
            else {
                return (endAngle - startAngle) > Math.PI;
            }
        }
    }
}
