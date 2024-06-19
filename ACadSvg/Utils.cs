#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp;
using ACadSharp.Entities;

using CSMath;


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
    }
}
