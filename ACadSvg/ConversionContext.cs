#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


using ACadSharp.Tables;


namespace ACadSvg {

    /// <summary>
    /// The conversion context contains various informations provided by the calling
    /// application. The <see cref="ConversionOptions"/> determine various details of
    /// the conversion process. The <see cref="ConversionInfo"/> receives the
	/// conversion log and the summary of occurring entities. 
	/// <see cref="ViewboxData"/> and <see cref="GlobalAttributeData"/> provide setting
	/// required for building the SVG element for the display in an application and for
	/// the output as SVG file.
	/// The internal <see cref="BlocksInDefs"/> property contains the list of converted
	/// <see cref="BlockRecord"/> objects that is filled during the conversion.
    /// </summary>
    public class ConversionContext {

		/// <summary>
		/// Gets a <see cref="BlockRecordsSvg"/> object containing a list of
		/// <see cref="BlockRecordSvg"/> objects representing the <see cref="BlockRecord"/>
		/// objects found in the AutoCAD document and <see cref="PatternSvg"/> objects
		/// used by <see cref="HatchSvg"/> objects. This list is filled during the
		/// conversion.
		/// </summary>
		internal BlockRecordsSvg BlocksInDefs { get; }


        /// <summary>
        /// Gets a <see cref="ACadSvg.ConversionInfo"/> object receifing the conversion
        /// log and the summary of occurring entities.
        /// </summary>
        public ConversionInfo ConversionInfo { get; }


		/// <summary>
		/// Gets or sets a <see cref="ACadSvg.ConversionOptions"/> object providing
		/// various options for the conversion process.
		/// </summary>
		public ConversionOptions ConversionOptions { get; set; }


        /// <summary>
        /// Gets or sets a <see cref="ACadSvg.ViewboxData"/> object providung data for an
        /// optional <i>viewbox</i> attribute for the <i>svg</i> element.
        /// </summary>
        public ViewboxData ViewboxData { get; set; }


		/// <summary>
		/// Gets or sets a <see cref="ACadSvg.GlobalAttributeData"/> object providing various
		/// setings to create attributes for the main group and the <i>svg</i> element.
		/// </summary>
		public GlobalAttributeData GlobalAttributeData { get; set; } = new GlobalAttributeData();


		public ConversionContext() {
			BlocksInDefs = new BlockRecordsSvg(this);
			ConversionInfo = new ConversionInfo();
			ConversionOptions = new ConversionOptions();
			ViewboxData = new ViewboxData();
			GlobalAttributeData = new GlobalAttributeData();
		}


		/// <summary>
		/// Updates the <see cref="ACadSvg.ConversionOptions"/>, <see cref="ACadSvg.ViewboxData"/>,
		/// and <see cref="ACadSvg.GlobalAttributeData"/> of this <see cref="ConversionContext"/>.
		/// </summary>
		/// <param name="conversionOptions"></param>
		/// <param name="viewboxData"></param>
		/// <param name="globalAttributeData"></param>
		public void UpdateSettings(
			ConversionOptions conversionOptions,
			ViewboxData viewboxData,
			GlobalAttributeData globalAttributeData) {
			ConversionOptions = conversionOptions;
			ViewboxData = viewboxData;
			GlobalAttributeData = globalAttributeData;
		}
	}
}
