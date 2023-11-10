#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion


using SvgElements;


namespace ACadSvg {

	public class GroupSvg : EntitySvg {

		public List<EntitySvg> Children = new List<EntitySvg>();


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			GroupElement groupElement = new GroupElement();
			groupElement.Comment = Comment;
			groupElement.ID = ID;
			groupElement.Class = Class;
			foreach (EntitySvg child in Children) {
				groupElement.Children.Add(child.ToSvgElement());
			}
			return groupElement;
		}
	}
}
