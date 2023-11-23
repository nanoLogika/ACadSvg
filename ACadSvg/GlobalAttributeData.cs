#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


namespace ACadSvg {

    public class GlobalAttributeData {

		public bool StrokeEnabled { get; set; }


		public string Stroke {  get; set; }


        public bool StrokeWidthEnabled { get; set; }


        public double StrokeWidth { get; set; }


		public bool FillEnabled { get; set; }


		public string Fill { get; set; }


		public double TransX { get; set; }


		public double TransY { get; set; }


		public double ScaleX { get; set; }


		public double ScaleY { get; set; }


		public double Rotation {  get; set; }
    }
}