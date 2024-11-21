#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


using SvgElements;


namespace ACadSvg {

	/// <summary>
	/// The base class for classes representing converted ACad entities
	/// containing other entities.
	/// </summary>
	public class GroupSvg : EntitySvg {

		public List<EntitySvg> Children = new List<EntitySvg>();


        public GroupSvg(ConversionContext ctx) : base(ctx) { }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            GroupElement groupElement = new GroupElement();
            groupElement.Comment = Comment;
            groupElement.ID = ID;
            groupElement.Class = Class;
            int insertAt = 0;
            foreach (EntitySvg child in Children) {
                if (child.InsertAtTopOfTheParentGroup) {
                    groupElement.Children.Insert(insertAt, child.ToSvgElement());
                    insertAt++;
                }
                else {
                    groupElement.Children.Add(child.ToSvgElement());
                }
            }
            return groupElement;
        }
    }
}
