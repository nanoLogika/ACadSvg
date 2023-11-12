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
using System.Text;


namespace ACadSvg {

	internal static class Utils {

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


		public static string LineToDashArray(Entity entity, LineType lineType) {
			List<double> result = new List<double>();

			LineType lType = lineType;
			if (lType.Name == "ByLayer") {
				lType = entity.Layer.LineType;
			}

            if (lType.Segments.Count() <= 0) {
				return String.Empty;
            }

            foreach (LineType.Segment segment in lType.Segments) {
                if (segment.Length == 0) {
                    result.Add(1);
                }
                else if (segment.Length > 0) {
                    result.Add(segment.Length);
                }
                else {
                    result.Add(Math.Abs(segment.Length));
                }
            }

            while (result.Count % 2 != 2 && result.Count < 4) {
                result.Add(result[result.Count - 2]);
            }

            if (result[result.Count - 1] == 0) {
                result.Add(result[result.Count - 2]);
            }

            StringBuilder sb = new StringBuilder();
			foreach (double item in result) {
				sb.Append(SvgElementBase.Cd(item)).Append(" ");
			}

			return sb.ToString().Trim();
		}
	}
}
