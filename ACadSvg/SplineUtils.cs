#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


using System.Runtime.Serialization;

using CSMath;

namespace ACadSvg {

    internal static class SplineUtils {

        /// <summary>
        /// Generate a smooth (interpolated) curve that follows the path of the given XY points.
        /// </summary>
        public static XY[] InterpolateXY(XY[] xys, int count) {
            if (xys is null || xys.Length == 0)
                throw new ArgumentException($"{nameof(xys)} must not be null, or have zero length.");

            int inputPointCount = xys.Length;
            double[] inputDistances = new double[inputPointCount];
            for (int i = 1; i < inputPointCount; i++) {
                XY dxy = xys[i] - xys[i - 1];
                double distance = dxy.GetLength();
                inputDistances[i] = inputDistances[i - 1] + distance;
            }

            double meanDistance = inputDistances.Last() / (count - 1);
            double[] evenDistances = Enumerable.Range(0, count).Select(x => x * meanDistance).ToArray();

            return Interpolate(inputDistances, xys, evenDistances);
        }


        private static XY[] Interpolate(double[] inputDistances, XY[] xyOrig, double[] evenDistances) {

            (double[] xs, double[] ys) = fromXYArray(xyOrig);

            double[] xsOut = Interpolate(inputDistances, xs, evenDistances);
            double[] ysOut = Interpolate(inputDistances, ys, evenDistances);

            XY[] xysOut = toXYArray(xsOut, ysOut);

            return xysOut;
        }


        private static (double[] xs, double[] ys) fromXYArray(XY[] xys) {
            double[] xsOut = new double[xys.Length];
            double[] ysOut = new double[xys.Length];
            for (int i = 0; i < xsOut.Length; i++) {
                xsOut[i] = xys[i].X;
                ysOut[i] = xys[i].Y;
            }
            return (xsOut, ysOut);
        }


        private static XY[] toXYArray(double[] xsOut, double[] ysOut) {
            XY[] xysOut = new XY[xsOut.Length];
            for (int i = 0; i < xsOut.Length; i++) {
                xysOut[i] = new XY(xsOut[i], ysOut[i]);
            }
            return xysOut;
        }


        private static double[] Interpolate(double[] xOrig, double[] yOrig, double[] xInterp) {
            (double[] a, double[] b) = FitMatrix(xOrig, yOrig);

            double[] yInterp = new double[xInterp.Length];
            for (int i = 0; i < yInterp.Length; i++) {
                int j;
                for (j = 0; j < xOrig.Length - 2; j++)
                    if (xInterp[i] <= xOrig[j + 1])
                        break;

                double dx = xOrig[j + 1] - xOrig[j];
                double t = (xInterp[i] - xOrig[j]) / dx;
                double y = (1 - t) * yOrig[j] + t * yOrig[j + 1] +
                    t * (1 - t) * (a[j] * (1 - t) + b[j] * t);
                yInterp[i] = y;
            }

            return yInterp;
        }


        private static (double[] a, double[] b) FitMatrix(double[] x, double[] y) {
            int n = x.Length;
            double[] a = new double[n - 1];
            double[] b = new double[n - 1];
            double[] r = new double[n];
            double[] A = new double[n];
            double[] B = new double[n];
            double[] C = new double[n];

            double dx1, dx2, dy1, dy2;

            dx1 = x[1] - x[0];
            C[0] = 1.0f / dx1;
            B[0] = 2.0f * C[0];
            r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);

            for (int i = 1; i < n - 1; i++) {
                dx1 = x[i] - x[i - 1];
                dx2 = x[i + 1] - x[i];
                A[i] = 1.0f / dx1;
                C[i] = 1.0f / dx2;
                B[i] = 2.0f * (A[i] + C[i]);
                dy1 = y[i] - y[i - 1];
                dy2 = y[i + 1] - y[i];
                r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
            }

            dx1 = x[n - 1] - x[n - 2];
            dy1 = y[n - 1] - y[n - 2];
            A[n - 1] = 1.0f / dx1;
            B[n - 1] = 2.0f * A[n - 1];
            r[n - 1] = 3 * (dy1 / (dx1 * dx1));

            double[] cPrime = new double[n];
            cPrime[0] = C[0] / B[0];
            for (int i = 1; i < n; i++)
                cPrime[i] = C[i] / (B[i] - cPrime[i - 1] * A[i]);

            double[] dPrime = new double[n];
            dPrime[0] = r[0] / B[0];
            for (int i = 1; i < n; i++)
                dPrime[i] = (r[i] - dPrime[i - 1] * A[i]) / (B[i] - cPrime[i - 1] * A[i]);

            double[] k = new double[n];
            k[n - 1] = dPrime[n - 1];
            for (int i = n - 2; i >= 0; i--)
                k[i] = dPrime[i] - cPrime[i] * k[i + 1];

            for (int i = 1; i < n; i++) {
                dx1 = x[i] - x[i - 1];
                dy1 = y[i] - y[i - 1];
                a[i - 1] = k[i - 1] * dx1 - dy1;
                b[i - 1] = -k[i] * dx1 + dy1;
            }

            return (a, b);
        }
    }
}
