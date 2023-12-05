using CSMath;


namespace ACadSvg.SplineUtils {

    public class RationalBSplinePoint {

        private XY _point = new XY();
        private double _weight = 1d;


        public RationalBSplinePoint(XY point, double weight) {
            _point = point;
            _weight = weight;
        }


        public XY Point {
            get { return _point; }
            set {
                _point = value;
            }
        }


        public double Weight {
            get { return _weight; }
            set {
                _weight = value;
            }
        }
    }
}
