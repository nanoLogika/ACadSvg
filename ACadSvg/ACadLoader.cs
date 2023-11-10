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
    /// AutoCAD <see cref="Entities"/> and to a <see cref="DocumentSvg"/>
    /// object. The DocumentSvg returns a XML/SVG structure.
    /// </summary>
    /// <remarks>
    /// Currently only DWG files can be red.
    /// </remarks>
    public class ACadLoader {

        public DocumentSvg LoadDwg(string path, ConversionContext ctx) {
            ctx.ConversionInfo.Log($"Loading DWG from \"{path}\" started");
            CadDocument doc = DwgReader.Read(path);
			DocumentSvg docSvg = new DocumentSvg(doc, ctx);
			return docSvg;
		}
    }
}