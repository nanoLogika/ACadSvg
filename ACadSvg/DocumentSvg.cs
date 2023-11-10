#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp;
using SvgElements;


namespace ACadSvg {

	public class DocumentSvg : EntitySvg {

		private CadDocument _doc;

		private ConversionContext _ctx;


		public DocumentSvg(CadDocument doc, ConversionContext ctx) {
			_doc = doc;
			_ctx = ctx;
		}


		/// <summary>
		/// TODO 
		/// </summary>
		/// <returns></returns>
		public override SvgElementBase ToSvgElement() {

			int layerCount = _doc.Layers.Count;
			_ctx.ConversionInfo.Log($"Layer count: {layerCount}");

			//	Create a container for all BlockRecord objects listed in the
			//	document header. Hatch patterns will be added to too.
			//	These elements will be appear under the defs element.
			//  Convert all Block Records first.
			//	NOTE: The blocksInDefs container ist passed to all Svg-element
			//	constructors, in case the conversion of an element requires
			//	to add a block record or pattern. It is not intended to look
			//	an item.

			foreach (var blockRecord in _doc.BlockRecords) {
				BlockRecordSvg blockRecordSvg = new BlockRecordSvg(blockRecord, _ctx);
				//	TODO	BlockRecordSvg does everything in the constructor
				//			Implement ToSvg() ?
			}

			//	Convert all entities directly placed in the document
			//	separate InsertSvg converted to use elements and place
			//	it at the end.
			var convertedEntities = ConvertEntitiesToSvg(_doc.Entities.ToList(), _ctx);
			bool hasRelevantEntries = convertedEntities.Count > 0;
			IList<InsertSvg> convertedInserts = new List<InsertSvg>();
			foreach (EntitySvg entitySvg in convertedEntities) {
				if (entitySvg is InsertSvg insertSvg) {
					convertedInserts.Add(insertSvg);
				}
			}
			foreach (InsertSvg insertSvg in convertedInserts) {
				convertedEntities.Remove(insertSvg);
			}

			//	If no entities directly placed in the document were found
			//	create a use element to place it in the main group.
			if (!hasRelevantEntries && _ctx.BlocksInDefs.Items.Count > 0) {
				string blockName = _ctx.BlocksInDefs.Items.ToArray()[0].ID;
				convertedInserts.Add(InsertSvg.Dummy(blockName));
				_ctx.ConversionInfo.Log($"Dummy use of first block {blockName} added.");
			}
			
			MainGroupSvg mainGroup = new MainGroupSvg();

			List<EntitySvg> children = mainGroup.Children;
			children.AddRange(convertedEntities);
			children.AddRange(convertedInserts);
			children.Add(_ctx.BlocksInDefs);

			_ctx.ConversionInfo.Log($"Loading finished");
			_ctx.ConversionInfo.Log($"Converted {_ctx.ConversionInfo.SuccessfulEntityConversions} of {_ctx.ConversionInfo.TotalEntities} entities");

			SvgElementBase svg = mainGroup.ToSvgElement();
			if (_ctx.ConversionOptions.ReverseY) {
				svg.AddScale(1, -1);
			}
			return svg;
		}
	}
}
