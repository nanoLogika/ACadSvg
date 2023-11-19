#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


namespace ACadSvg {

    /// <summary>
    /// Provides a xmin, y-min, width and height for an optional <i>viewbox</i> attribute
    /// to be added to the <i>svg</i> element. 
    /// </summary>
    public class ViewboxData {

        /// <summary>
        /// Gets or sets a value indicates that a <i>viewbox</i> attribute is to added to the
        /// <i>svg</i> element.
        /// </summary>
        /// <value><b>true</b>, when an <i>viewbox</i> attribute is to be added; otherwise, <b>false</b>.</value>
        public bool Enabled { get; set; } = false;


        /// <summary>
        /// Gets or sets the min-x value for the <i>viewbox</i> attribute.
        /// </summary>
        public double MinX { get; set; } = 0;


        /// <summary>
        /// Gets or sets the min-y value for the <i>viewbox</i> attribute.
        /// </summary>
        public double MinY { get; set; } = 0;


        /// <summary>
        /// Gets or sets the width value for the <i>viewbox</i> attribute.
        /// </summary>
        public double Width { get; set; } = 1000;


        /// <summary>
        /// Gets or sets the height value for the <i>viewbox</i> attribute.
        /// </summary>
        public double Height { get; set; } = 1000;
    }
}
