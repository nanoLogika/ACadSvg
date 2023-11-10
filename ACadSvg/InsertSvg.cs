#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;


namespace ACadSvg {

    internal class InsertSvg : EntitySvg {

        private Insert _insert;
        private string _blockName = string.Empty;


        public override bool Skip {
            get { return _insert.Block.Name.StartsWith("*"); }
        }


        /// <summary>
		/// Initializes a new instance of the <see cref="InsertSvg"/> class
		/// without reference to a <see cref="Insert"/> entity. This is used
        /// to create a <i>use</i> element for an arbitrary SVG group.
        /// </summary>
        private InsertSvg() {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="InsertSvg"/> class
        /// for the specified <see cref="Insert"/> entity.
        /// </summary>
        /// <param name="insert">The <see cref="Circle"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public InsertSvg(Entity insert, ConversionContext ctx) {
            _insert = (Insert)insert;
		}


        /// <summary>
        /// Creates an <see cref="InsertSvg"/> object representing a <i>use</i> element
        ///  for an arbitrary SVG group with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the group to be referenced, normally a block name.</param>
        /// <returns></returns>
        public static InsertSvg Dummy(string id) {
            InsertSvg dummy = new InsertSvg();
            dummy._blockName = id;
            return dummy;
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            if (_insert == null) {
                return new UseElement()
                    .WithGroupId(_blockName);
            }

            double rot = _insert.Rotation * 180 / Math.PI;
			double xs = _insert.InsertPoint.X / _insert.XScale;
			double ys = _insert.InsertPoint.Y / _insert.YScale;
			string blockName = Utils.CleanBlockName(_insert.Block.Name);

            return new UseElement()
                .WithGroupId(blockName)
                .WithXY(xs, ys)
                .AddScale(_insert.XScale, _insert.YScale)
                .AddRotate(rot, xs, ys);
		}
    }
}
