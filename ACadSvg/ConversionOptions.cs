#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


namespace ACadSvg {

    /// <summary>
    /// Provides providing various options for the conversion process.
    /// </summary>
    public class ConversionOptions {

        /// <summary>
        /// Get or sets a value indication that the y-direction is to be reversed, i.e.
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
        public bool ReverseY {  get; set; } = true;


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
        /// Gets or sets a value indicating that a comment is to be created
        /// for every converted AutoCAD Entity. This option is not yet implemented.
        /// </summary>
        /// <value>
        /// <b>true</b>, when ; otherwise, <b>false</b>.
        /// </value>
		public bool EnableComments { get; set; } = false;
	}
}
