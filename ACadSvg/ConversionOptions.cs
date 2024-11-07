#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


using ACadSharp;

namespace ACadSvg {

    /// <summary>
    /// Provides providing various options for the conversion process.
    /// </summary>
    public class ConversionOptions {

        /// <summary>
        /// Defines values for the <see cref="GroupFilterMode"/> property indicating
        /// whether the filter expresion for blocks is to be applied as an exlude filter,
        /// an include filter, or the filter is off.
        /// </summary>
        public enum FilterMode : int {
            Exclude = 2,
            Include = 1,
            Off = 0
        }


        /// <summary>
        /// Get or sets a value indicating that the y-direction is to be reversed, i.e.
        /// y-coordinates grow from bottom to top.
        /// </summary>
        /// <remarks>
        /// In the SVG coordinate system y grows from top to bottom.
        /// This option allows to reverse the y-coodinates so that y grows from bottom
        /// to top and AutoCAD coordinates can be used as they are delivered.
        /// </remarks>
        /// <value>
        /// <b>true</b>, when the y-direction is to be reversed; otherwise, <b>false</b>.
        /// </value>
        public bool ReverseY { get; set; } = true;


        /// <summary>
        /// Gets or sets a value indicating that an <i>id</i> attribute is to be created 
        /// for every converted AutoCAD Entity from the entity handle (Hex).
        /// </summary>
        /// <value>
        /// <b>true</b>, when a<i>id</i> attributes are to be created; otherwise, <b>false</b>.
        /// </value>
		public bool ExportHandleAsID { get; set; } = true;


        /// <summary>
        /// Gets or sets a value indicating that a <i>class</i>-attribute is to created
        /// for every converted AutoCAD Entity from the name of the entity's layer.
        /// </summary>
        /// <value>
        /// <b>true</b>, when <i>class</i>-attributes are to created; otherwise, <b>false</b>.
        /// </value>
		public bool ExportLayerAsClass { get; set; } = true;


        /// <summary>
        /// Gets or sets a value indicating that a <i>class</i>>-attribute is to be created
        /// for every converted AutoCAD Entity from the object type.
        /// </summary>
        /// <value>
        /// <b>true</b>, when <i>class</i>>-attributes are to be created; otherwise, <b>false</b>.
        /// </value>
		public bool ExportObjectTypeAsClass { get; set; } = true;


        /// <summary>
        /// Gets or sets the default line weight in hundreths of mm.
        /// </summary>
        public LineweightType DefaultLineweight { get; set; } = LineweightType.W25;


        /// <summary>
        /// Gets or sets the lineweight scale factor.
        /// </summary>
        /// <value>
        /// The lineweight factor or zero if the lineweight values shall be useded as
        /// read from AutoCAD.
        /// </value>
        /// <remarks>
        /// The lineweight value specified in mm in WCS is to be multiplied with a scale
        /// factor to create reasonable stroke-width attributes in SVG.
        /// </remarks>
        public double LineweightScaleFactor { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether a rectangle element representing the
        /// model-space extent is to be created and stored in the Scales editor.
        /// </summary>
        /// <value>
        /// <b>true</b> if a rectangle element is to be created; otherwise, <b>false</b>.
        /// </value>
        public bool CreateScaleFromModelSpaceExtent { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether the viewbox limits shall be set
        /// according to the model-space extent.
        /// </summary>
        /// <value>
        /// <b>true</b> if the viewbox limits shall be set; otherwise, <b>false</b>.
        /// </value>
        public bool CreateViewboxFromModelSpaceExtent { get; set; }


        /// <summary>
        /// Gets or sets a regular expression that is to be used to filter the Blocks read
        /// from the AutoCAD file by their name. 
        /// </summary>
        public string GroupFilterRegex { get; set; } = string.Empty;


        /// <summary>
        /// Gets or sets a value indicating whether the filter expresion for blocks is to be
        /// applied as exlude filter, as include filter, or the filter is off.
        /// </summary>
        public FilterMode GroupFilterMode { get; set; } = FilterMode.Off;


        /// <summary>
        /// Gets or sets a value indicating whether Insert entities converted to &lt;use ...&gt;
        /// elements are collected and placed at the the main group. Inserts foung in blocks
        /// are not affected.
        /// </summary>
        public bool ConcentrateInserts { get; set; } = false;


        /// <summary>
        /// Gets or sets a value indicating whether an extra group for free elements, i.e.
        /// elements that are not part of a block, is created.
        /// </summary>
        public bool CreateExtraGroupForFreeElements { get; set; } = false;


        /// <summary>
        /// Gets or sets the prefix for dynamic-block subblock IDs. 
        /// </summary>
        public string BlockVisibilityParametersPrefix { get; set; } = "_";


        /// <summary>
        /// Gets or sets a value indicating whether SVG elements are to be included
        /// that reperesent coordinates or definition points, e.g., in leaders or dimensions.
        /// </summary>
        public bool CreateDebugElements { get; set; } = false;
    }
}
