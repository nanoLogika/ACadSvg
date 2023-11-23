#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp;
using ACadSharp.Tables;
using SvgElements;


namespace ACadSvg {

    /// <summary>
    /// Represents SVG elements from an complete AutoCAD document, i.e. the
    /// list of <see cref="BlockRecordSvg"/> objects representing <see cref="BlockRecord"/>
    /// objects, and a list of <see cref="EntitySvg" /> objects representing AutoCAD
    /// entities that are not member of a <see cref="BlockRecord"/>.
    /// </summary>
	public class DocumentSvg : EntitySvg {

		private CadDocument _doc;

		//private BlockRecordsSvg _blockRecordSvg;
        private IList<EntitySvg> _convertedEntities;
        private IList<InsertSvg> _convertedInserts;


        /// <summary>
        /// Creates a new instance of a <see cref="DocumentSvg"/> object by converting
        /// the <see cref="CadDocument" /> specified as <paramref name="doc"/>.
        /// </summary>
        /// <param name="doc">The <see cref="CadDocument" /> to be converted.</param>
        /// <param name="ctx">The <see cref="ConversionContext"/> specifys conversion
        /// options and receives the conversion log.</param>
        public DocumentSvg(CadDocument doc, ConversionContext ctx) : base(ctx) {
			_doc = doc;

            int layerCount = _doc.Layers.Count;
            _ctx.ConversionInfo.Log($"Layer count: {layerCount}");

            convertBlockRecordsFromDocHeader();

            //	Convert all entities directly placed in the document
            _convertedEntities = ConvertEntitiesToSvg(_doc.Entities.ToList(), _ctx);
            _convertedInserts = new List<InsertSvg>();
            //placeInsertSvgToTheEnd(); //  TODO This shall be controlle by a conversion option.

            _ctx.ConversionInfo.Log($"Loading finished");
            _ctx.ConversionInfo.Log($"Converted {_ctx.ConversionInfo.SuccessfulEntityConversions} of {_ctx.ConversionInfo.TotalEntities} entities");
        }


        /// <summary>
        /// Creates a main group containing all converted entities that are not bound
        /// in block records converted into an SVG <i>g</i> element.
        /// </summary>
        /// <returns></returns>
        public SvgElementBase MainGroupToSvgElement() {
            createDummyInsertIfMainGroupIsEmpty();

            MainGroupSvg mainGroup = new MainGroupSvg(_ctx);

            List<EntitySvg> children = mainGroup.Children;
            children.AddRange(_convertedEntities);
            children.AddRange(_convertedInserts);

            SvgElementBase svgElement = mainGroup.ToSvgElement();
            if (_ctx.ConversionOptions.ReverseY) {
                svgElement.AddScale(1, -1);
            }
            return svgElement;
        }



        /// <summary>
        /// Returns an SVG element containing the list of block records converted into
        /// <i>defs</i> element.
        /// </summary>
        /// <returns></returns>
        public SvgElementBase DefsToSvgElement() {
            return _ctx.BlocksInDefs.ToSvgElement();
        }


        /// <summary>
        /// Creates a main group containing all converted entities that are not bound
        /// in block records and the <i>defs</i> element and create SVG elements.
        /// </summary>
        /// <returns></returns>
        public override SvgElementBase ToSvgElement() {

            createDummyInsertIfMainGroupIsEmpty();

            MainGroupSvg mainGroup = new MainGroupSvg(_ctx);

            List<EntitySvg> children = mainGroup.Children;
            children.AddRange(_convertedEntities);
            children.AddRange(_convertedInserts);
            children.Add(_ctx.BlocksInDefs);

            SvgElementBase svg = mainGroup.ToSvgElement();
            if (_ctx.ConversionOptions.ReverseY) {
                svg.AddScale(1, -1);
            }
            return svg;
        }


        //	Creates a container for all BlockRecord objects listed in the
        //	document header. Hatch patterns will be added to too when Hatch
        //	entities are converted. These elements will be appear under the
        //	defs element.
        //  Convert all Block Records first.
        //	NOTE: The blocksInDefs container ist passed to all Svg-element
        //	constructors, in case the conversion of an element requires
        //	to add a block record or pattern. It is not intended to look up
        //	an item.
        private void convertBlockRecordsFromDocHeader() {
            foreach (var blockRecord in _doc.BlockRecords) {
                BlockRecordSvg blockRecordSvg = new BlockRecordSvg(blockRecord, _ctx);
                //	TODO	BlockRecordSvg does everything in the constructor
                //			Implement ToSvg() ?
            }
        }


        //	Separate InsertSvg converted to use elements and place
        //	it at the end.
        private void placeInsertSvgToTheEnd() {
            _convertedInserts = new List<InsertSvg>();
            foreach (EntitySvg entitySvg in _convertedEntities) {
                if (entitySvg is InsertSvg insertSvg) {
                    _convertedInserts.Add(insertSvg);
                }
            }
            foreach (InsertSvg insertSvg in _convertedInserts) {
                _convertedEntities.Remove(insertSvg);
            }
        }


        //	If no entities directly placed in the document were found
        //	create a use element to place it in the main group.
        private void createDummyInsertIfMainGroupIsEmpty() {
            bool hasRelevantEntries = _convertedEntities.Count > 0 || _convertedInserts.Count > 0;
            if (!hasRelevantEntries && _ctx.BlocksInDefs.Items.Count > 0) {
                string blockName = _ctx.BlocksInDefs.Items.ToArray()[0].ID;
                _convertedInserts.Add(InsertSvg.Dummy(blockName));
                _ctx.ConversionInfo.Log($"Dummy use of first block {blockName} added.");
            }
        }


        /// <summary>
        /// Creates a <see cref="SvgElement"/> object and sets values for the attributes
        /// <i>viewbox</i>, <i>stroke</i>, <i>strike-width</i>, and <i>fill</i>.
        /// </summary>
        /// <param name="ctx">The conversion context provides several options and the values for the attributes to create.</param>
        /// <returns>The ctreated <see cref="SvgElement"/>.</returns>
        public static SvgElement CreateSVG(ConversionContext ctx) {

            SvgElement svgElement = new SvgElement() { ID = "svg-element" };

            if (ctx.ViewboxData.Enabled) {
                svgElement.WithViewbox(
                    ctx.ViewboxData.MinX,
                    ctx.ConversionOptions.ReverseY ? ctx.ViewboxData.MinY - ctx.ViewboxData.Height : ctx.ViewboxData.MinY,
                    ctx.ViewboxData.Width,
                    ctx.ViewboxData.Height);
            }

            if (ctx.GlobalAttributeData.StrokeEnabled) {
                svgElement.WithStroke(ctx.GlobalAttributeData.Stroke);
            }

            if (ctx.GlobalAttributeData.StrokeWidthEnabled) {
                svgElement.WithStrokeWidth(ctx.GlobalAttributeData.StrokeWidth);
            }

            if (ctx.GlobalAttributeData.FillEnabled) {
                svgElement.WithFill(ctx.GlobalAttributeData.Fill);
            }
            else {
                svgElement.WithFill("none");
            }

            return svgElement;
        }
    }
}
