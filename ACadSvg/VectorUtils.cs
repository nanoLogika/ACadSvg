#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


using CSMath;

namespace ACadSvg {

    internal static class VectorUtils {

        /// <summary>
        /// Returns this <see cref="XY"/> rotated by 90° counterclockwise.
        /// </summary>
        /// <param name="xy"></param>
        /// <returns>A <see cref="XY"/> rotated by 90° from this XY.</returns>
        internal static XY Rotate90(this XY xy) {
            return xy.Rotate(Math.PI / 2);
        }


        /// <summary>
        /// Returns this <see cref="XY"/> rotated by <paramref name="angle"/> counterclockwise.
        /// </summary>
        /// <param name="xy"></param>
        /// <returns>A <see cref="XY"/> rotated by <paramref name="angle"/> from this XY.</returns>
        internal static XY Rotate(this XY xy, double angle) {
            return new XY(
                xy.X * Math.Cos(angle) - xy.Y * Math.Sin(angle),
                xy.X * Math.Sin(angle) + xy.Y * Math.Cos(angle));
        }


        internal static XY ToXY(this XYZ xyz) {
            return new XY(xyz.X, xyz.Y);
        }


        internal static void XYZToDoubles(List<XYZ> xyzs, out double[] xs, out double[] ys) {
            xs = new double[xyzs.Count];
            ys = new double[xyzs.Count];
            for (int i = 0; i < xyzs.Count; i++) {
                XYZ xyz = xyzs[i];
                xs[i] = xyz.X;
                ys[i] = xyz.Y;
            }
        }


        internal static XY[] DoublesToXYx(double[] xs, double[] ys) {
            XY[] xYs = new XY[xs.Length];
            for (int i = 0; i < xs.Length; i++) {
                xYs[i] = new XY(xs[i], ys[i]);
            }
            return xYs;
        }
    }
}
