#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp;
using ACadSharp.IO;


namespace ACadSvg {

    /// <summary>
    /// This loader provides methods to read AutoCAD files and convert
    /// AutoCAD <see cref="Entities"/> and related objects
    /// to a <see cref="DocumentSvg"/>
    /// object. The DocumentSvg returns a XML/SVG structure.
    /// </summary>
    /// <remarks>
    /// Currently only DWG files can be red.
    /// </remarks>
    public static class ACadLoader {

        /// <summary>
        /// Loads a DWG file from the specified path and converts it to a SVG/XML structure.
        /// SVG is represented by a <see cref="DocumentSvg"/> object.
        /// </summary>
        /// <param name="path">The path of the DWG file to be read.</param>
        /// <param name="ctx">The conversion context providing conversion options.
        /// It also received the conversion log.</param>
        /// <returns>A <see cref="DocumentSvg" /> representing the read document converted to SVG/XML.</returns>
        public static DocumentSvg LoadDwg(string path, ConversionContext ctx) {
            ctx.ConversionInfo.Log($"Loading DWG from \"{path}\" started");
            CadDocument doc = DwgReader.Read(path);
			DocumentSvg docSvg = new DocumentSvg(doc, ctx);
			return docSvg;
		}


        /// <summary>
        /// Loads a DXF file from the specified path and converts it to a SVG/XML structure.
        /// SVG is represented by a <see cref="DocumentSvg"/> object.
        /// </summary>
        /// <param name="path">The path of the DWG file to be read.</param>
        /// <param name="ctx">The conversion context providing conversion options.
        /// It also received the conversion log.</param>
        /// <returns>A <see cref="DocumentSvg" /> representing the read dicument converted to SVG/XML.</returns>
		public static DocumentSvg LoadDxf(string path, ConversionContext ctx) {
			ctx.ConversionInfo.Log($"Loading DXF from \"{path}\" started");
			CadDocument doc = DxfReader.Read(path);
			DocumentSvg docSvg = new DocumentSvg(doc, ctx);
			return docSvg;
		}
	}
}