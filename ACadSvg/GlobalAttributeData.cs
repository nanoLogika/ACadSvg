#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Drawing;


namespace ACadSvg {

    public class GlobalAttributeData {

		public bool StrokeEnabled { get; set; }


		public string Stroke {  get; set; }


		public double StrokeWidth { get; set; }


		public bool FillEnabled { get; set; }


		public string Fill { get; set; }


		public double TransX { get; set; }


		public double TransY { get; set; }


		public double ScaleX { get; set; }


		public double ScaleY { get; set; }


		public double Rotation {  get; set; }


		public GlobalAttributeData(
            bool strokeEnabled,
            Color strokeColor,
            double strokeWidth,
            bool fillEnabled,
            Color fillColor,
            double transX,
            double transY,
            double scaleX,
            double scaleY,
            double rot) {

			StrokeEnabled = strokeEnabled;
			Stroke = strokeColor.Name;
			StrokeWidth = strokeWidth;
			FillEnabled = fillEnabled;
			Fill = fillColor.Name;
			TransX = transX;
			TransY = transY;
			ScaleX = scaleX;
			ScaleY = scaleY;
			Rotation = rot;
        }
	}
}