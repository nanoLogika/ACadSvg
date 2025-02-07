#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using ACadSharp.Objects;
using ACadSharp.Objects.Evaluations;
using ACadSharp.Tables;
using System.Text.RegularExpressions;

namespace ACadSvg {
    //  TODO refactor

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="BlockRecord"/> table entry.
    /// The <see cref="BlockRecord"/> is converted into a <i>g</i> element. A <see cref="BlockRecord"/>
    /// may be associated wit a <see cref="BlockVisibilityParameter"/> object that defines subsets
    /// of entities that are converted to subordinate <i>g</i> elements.
    /// </summary>
    /// <remarks><para>
    /// The implementation of ths class is not complete:
    /// </para>
    /// <list type="bullet"> <item>
    /// The conversion is executed in the constructor, the ToSvgElement() method is missing.
    /// </item><item>
    /// Currently a fixed pattern for the <see cref="Skip"/> condtion is used:
    /// Block records are skipped, when the name starts with "*".
    /// </item></list>
    /// </remarks>
    public class BlockRecordSvg : GroupSvg {

		private BlockRecord _blockRecord;

        /// <summary>
        /// Gets a value indicating whether this <see cref="BlockRecord"/> is to be skipped
        /// and excluded from the conversion. The conversion ist to be skipped when the
        /// block name matches a pattern, defined in the conversion options
        /// (<see cref="ConversionContext"/>).
        /// </summary>
		public override bool Skip {
			get {
                try {
                    ConversionOptions options = _ctx.ConversionOptions;
                    if (options.GroupFilterMode == ConversionOptions.FilterMode.Include) {
                        return !new Regex(options.GroupFilterRegex).IsMatch(ID);
                    }
                    else if (options.GroupFilterMode == ConversionOptions.FilterMode.Exclude) {
                        return new Regex(options.GroupFilterRegex).IsMatch(ID);
                    }
                }
                catch {
                    //	Ignore, do not filter
                }

                return false;
            }
		}


		public BlockRecordSvg(BlockRecord blockRecord, ConversionContext ctx) : base(ctx) {

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
                foreach (Entity entity in dynamicBLock.Entities) {
                    blockRecordEntities.Remove(entity);
                }
                if (blockRecordEntities.Count > 0) {
                    childGroupSvg = new GroupSvg(_ctx) {
                        ID = $"{ID}_visible"
					};
                    //  Add "free-entities subblock" and convert the remaining entities
                    Children.Add(childGroupSvg);
                    childGroupSvg.Children.AddRange(ConvertEntitiesToSvg(blockRecordEntities, ctx));
                }
            }
            else {
                this.Children.AddRange(ConvertEntitiesToSvg(blockRecordEntities, ctx));
            }

            if (dynamicBLock != null) {
                if (blockRecord.Entities.Count > 0) {
                    //  Close free-elements group
                    if (childGroupSvg != null) {
                        Children.Add(childGroupSvg);
                    }
                }
                foreach (var state in dynamicBLock.States) {
                    GroupSvg subBlockGroupSvg = new GroupSvg(_ctx) {
                        ID = $"{_ctx.ConversionOptions.BlockVisibilityParametersPrefix}{Utils.CleanBlockName(state.Name)}"
                    };

                    subBlockGroupSvg.Children.AddRange(ConvertEntitiesToSvg(state.Entities, ctx));

                    Children.Add(subBlockGroupSvg);
                }
            }

			Class = "block-record";

			ctx.BlocksInDefs.Items.Add(this);
			ctx.ConversionInfo.Log($"{_blockRecord.Handle.ToString("X")}: End Block: {blockRecord.Name}");
		}


        private static BlockVisibilityParameter getDynamicBlock(BlockRecord blockRecord) {
            BlockVisibilityParameter dynamicBLock = null;
            if (blockRecord.XDictionary != null && blockRecord.XDictionary.EntryNames.Contains("ACAD_ENHANCEDBLOCK")) {
                var enhancedBlock = blockRecord.XDictionary["ACAD_ENHANCEDBLOCK"] as EvaluationGraph;
                if (enhancedBlock != null && enhancedBlock is EvaluationGraph) {
                    foreach (EvaluationGraph.Node node in enhancedBlock.Nodes) {
                        if (node.Expression is BlockVisibilityParameter) {
                            dynamicBLock = (BlockVisibilityParameter)node.Expression;
                            break;
                        }
                    }
                }
            }

            return dynamicBLock;
        }
    }
}
