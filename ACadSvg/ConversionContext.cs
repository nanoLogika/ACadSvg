#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


namespace ACadSvg {

	public class ConversionContext {

		public BlockRecordsSvg BlocksInDefs { get; } = new BlockRecordsSvg();


		public ConversionInfo ConversionInfo { get; } = new ConversionInfo();


		public ConversionOptions ConversionOptions { get; set; }


		public ViewboxData ViewboxData { get; set; }


		public GlobalAttributeData GlobalAttributeData { get; set; }


		public void UpdateSettings(ConversionOptions conversionOptions, ViewboxData viewboxData, GlobalAttributeData globalAttributeData) {
			ConversionOptions = conversionOptions;
			ViewboxData = viewboxData;
			GlobalAttributeData = globalAttributeData;
		}
	}
}
