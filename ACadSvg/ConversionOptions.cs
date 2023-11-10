#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


namespace ACadSvg {

	public class ConversionOptions {

		public bool ReverseY {  get; set; } = true;


		public bool ExportHandleAsID { get; set; } = true;


		public bool ExportLayerAsClass { get; set; } = true;


		public bool EnableComments { get; set; } = true;
	}
}
