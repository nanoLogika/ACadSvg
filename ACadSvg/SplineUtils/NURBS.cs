using CSMath;

namespace ACadSvg.SplineUtils {

    public class NURBS {

        private IList<RationalBSplinePoint> _controlPoints = new List<RationalBSplinePoint>();
        private double[] _knots;
        private bool _isBSpline = true;
        private int _degree;
        private double _stepSize = 0.05;


        public IList<RationalBSplinePoint> ControlPoints {
            get { return _controlPoints; }
        }


        public double[] Knots {
            get { return _knots; }
            set { _knots = value; }
        }


        public bool IsBSpline {
            get { return _isBSpline; }
            set { _isBSpline = value; }
        }


        public int Degree {
            get { return _degree; }
            set { _degree = value; }
        }


        public double StepSize {
            get { return _stepSize; }
            set { _stepSize = value; }
        }


        internal static IList<XY> CreateBSplineCurve(int degree, IList<XYZ> xyzControlPoints, IList<double> knotsList) {
            NURBS nurbs = new NURBS();

            nurbs.Degree = degree;
            nurbs.ControlPoints.Clear();
            foreach (XYZ xyz in xyzControlPoints) {
                nurbs.ControlPoints.Add(new RationalBSplinePoint(Utils.ToXY(xyz), 1));
            }

            int m = knotsList.Count;
            double knot0 = knotsList[0];
            double knotf = knotsList[m - 1] - knot0;
            nurbs.Knots = new double[m];
            for (int i = 0; i < knotsList.Count; i++) {
                nurbs.Knots[i] = (knotsList[i] - knot0) / knotf;
            }

            return nurbs.EvaluateBSplineCurve();
        }



        /// <summary>
        /// This code is translated to C# from the original C++  code given on page 74-75 in "The NURBS Book" by Les Piegl and Wayne Tiller 
        /// </summary>
        /// <param name="i">Current control pont</param>
        /// <param name="degree">The picewise polynomial degree</param>
        /// <param name="knots">The knot vector</param>
        /// <param name="u">The value of the current curve point. Valid range from 0 <= u <=1 </param>
        /// <returns>N_{i,p}(u)</returns>
        private static double eveluateNipOfu(int i, int degree, double[] knots, double u) {
            double[] nips = new double[degree + 1];
            double saved;
            double temp;

            int m = knots.Length - 1;
            if ((i == 0 && u == knots[0]) || (i == (m - degree - 1) && u == knots[m])) {
                return 1;
            }

            if (u < knots[i] || u >= knots[i + degree + 1]) {
                return 0;
            }

            for (int j = 0; j <= degree; j++) {
                if (u >= knots[i + j] && u < knots[i + j + 1]) {
                    nips[j] = 1d;
                }
                else {
                    nips[j] = 0d;
                }
            }

            for (int k = 1; k <= degree; k++) {
                if (nips[0] == 0) {
                    saved = 0d;
                }
                else {
                    saved = ((u - knots[i]) * nips[0]) / (knots[i + k] - knots[i]);
                }
                for (int j = 0; j < degree - k + 1; j++) {
                    double uLeft = knots[i + j + 1];
                    double uRight = knots[i + j + k + 1];

                    if (nips[j + 1] == 0) {
                        nips[j] = saved;
                        saved = 0d;
                    }
                    else {
                        temp = nips[j + 1] / (uRight - uLeft);
                        nips[j] = saved + (uRight - u) * temp;
                        saved = (u - uLeft) * temp;
                    }
                }
            }
            return nips[0];
        }


        public IList<XY> EvaluateBSplineCurve() {

            IList<XY> result = new List<XY>();

            for (double t = 0; t < 1; t += _stepSize) {
                if (_isBSpline) {
                    result.Add(createBSplinePoint(t));
                }
                else {
                    result.Add(createRationalBSplinePoint(t));
                }

            }

            if (!result.Contains(_controlPoints[_controlPoints.Count - 1].Point)) {
                result.Add(_controlPoints[_controlPoints.Count - 1].Point);
            }

            return result;
        }


        private XY createBSplinePoint(double t) {

            double x, y;
            x = 0;
            y = 0;
            for (int i = 0; i < _controlPoints.Count; i++) {
                double temp = eveluateNipOfu(i, _degree, _knots, t);
                x += _controlPoints[i].Point.X * temp;
                y += _controlPoints[i].Point.Y * temp;
            }
            return new XY(x, y);
        }


        private XY createRationalBSplinePoint(double t) {

            double x, y;
            x = 0;
            y = 0;
            double rationalWeight = 0d;

            for (int i = 0; i < _controlPoints.Count; i++) {
                double temp = eveluateNipOfu(i, _degree, _knots, t) * _controlPoints[i].Weight;
                rationalWeight += temp;
            }

            for (int i = 0; i < _controlPoints.Count; i++) {
                double temp = eveluateNipOfu(i, _degree, _knots, t);
                x += _controlPoints[i].Point.X * _controlPoints[i].Weight * temp / rationalWeight;
                y += _controlPoints[i].Point.Y * _controlPoints[i].Weight * temp / rationalWeight;
            }
            return new XY(x, y);
        }
    }
}
