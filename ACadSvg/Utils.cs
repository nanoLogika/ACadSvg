#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using CSMath;

using SvgElements;

using System.Text;


namespace ACadSvg {

	internal static class Utils {

        public static XY[] XYZToXYArray(XYZ[] xyzs) {
            XY[] xys = new XY[xyzs.Length];
            for (int i = 0; i < xyzs.Length; i++) {
                xys[i] = new XY(xyzs[i].X, xyzs[i].Y);
            }
            return xys;
        }


        public static XYZ[] XYToXYZArray(XY[] xysOut) {
            XYZ[] xyzsOut = new XYZ[xysOut.Length];
            for (int i = 0; i < xysOut.Length; i++) {
                xyzsOut[i] = new XYZ(xysOut[i].X, xysOut[i].Y, 0);
            }
            return xyzsOut;
        }


        public static double[] VerticesToArray(IList<XY> list) {
            List<double> result = new List<double>();

            foreach (XYZ v in list) {
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
	}
}
