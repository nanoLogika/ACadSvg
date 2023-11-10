#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using ACadSharp.Objects;
using ACadSharp.Tables;


namespace ACadSvg {

	public class BlockRecordSvg : GroupSvg {

		private BlockRecord _blockRecord;


		public override bool Skip {
			get { return ID.StartsWith("*"); }
		}


		public BlockRecordSvg(BlockRecord blockRecord, ConversionContext ctx) {

			_blockRecord = blockRecord;

			ctx.ConversionInfo.Log($"{_blockRecord.Handle.ToString("X")}: Start Block: {blockRecord.Name}");

			ID = Utils.CleanBlockName(_blockRecord.Name);

            if (Skip) {
                ctx.ConversionInfo.Log($"{_blockRecord.Handle.ToString("X")}: Block: {ID} skipped");
				ctx.ConversionInfo.Log($"{_blockRecord.Handle.ToString("X")}: End Block: {blockRecord.Name}");
				return;
            }

            //	Log extended data info
            var blockExtendedDataInfo = GetEntityExtendedDataInfo(_blockRecord);
            if (!string.IsNullOrEmpty(blockExtendedDataInfo)) {
                ctx.ConversionInfo.Log($"ExtendedData: {blockExtendedDataInfo}");
            }

            BlockVisibilityParameter dynamicBLock = getDynamicBlock(blockRecord);

            IList<Entity> blockRecordEntities = new List<Entity>(_blockRecord.Entities);
            GroupSvg childGroupSvg = null;
            if (dynamicBLock != null) {
                //  The Entities list of the BlockVisibilityParameter object contains
                //  all entities of the dynamic block. The combined set of entities
                //  of all subblocks is equal to the total list.
                //  It is not known whether all entities of the block record appear in
                //  the dynamic block. Thus create an additonal group to collect the
                //  rest. Add the "free-entities subblock" as subgroup.
                //  NOTE: 
                foreach (Entity entity in dynamicBLock.Entities) {
                    blockRecordEntities.Remove(entity);
                }
                if (blockRecordEntities.Count > 0) {
                    childGroupSvg = new GroupSvg() {
                        ID = $"{ID}_visible"
                    };
                    Children.Add(childGroupSvg);
                }
            }

            //  Add the ... TODO see above
            GroupSvg targetGroupSvg = childGroupSvg == null ? this : childGroupSvg;
            targetGroupSvg.Children.AddRange(ConvertEntitiesToSvg(blockRecordEntities, ctx));

            if (dynamicBLock != null) {
                if (blockRecord.Entities.Count > 0) {
                    //  Close free-elements group
                    if (childGroupSvg != null) {
                        Children.Add(childGroupSvg);
                    }
                }
                foreach (var subBlock in dynamicBLock.SubBlocks) {
                    GroupSvg subBlockGroupSvg = new GroupSvg() {
                        ID = Utils.CleanBlockName(subBlock.Name)
                    };

                    subBlockGroupSvg.Children.AddRange(ConvertEntitiesToSvg(subBlock.Entities, ctx));

                    Children.Add(subBlockGroupSvg);
                }
            }

            ctx.BlocksInDefs.Items.Add(this);
			ctx.ConversionInfo.Log($"{_blockRecord.Handle.ToString("X")}: End Block: {blockRecord.Name}");
		}


        private static BlockVisibilityParameter getDynamicBlock(BlockRecord blockRecord) {
            BlockVisibilityParameter dynamicBLock = null;
            if (blockRecord.XDictionary != null && blockRecord.XDictionary.EntryNames.Contains("ACAD_ENHANCEDBLOCK")) {
                var enhancedBlock = blockRecord.XDictionary["ACAD_ENHANCEDBLOCK"] as EvaluationGraph;
                if (enhancedBlock != null && enhancedBlock is EvaluationGraph) {
                    foreach (EvaluationGraph.GraphNode node in enhancedBlock.Nodes) {
                        if (node.NodeObject is BlockVisibilityParameter) {
                            dynamicBLock = (BlockVisibilityParameter)node.NodeObject;
                            break;
                        }
                    }
                }
            }

            return dynamicBLock;
        }
    }
}
