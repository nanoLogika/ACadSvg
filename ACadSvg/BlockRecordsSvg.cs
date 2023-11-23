#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using SvgElements;
using ACadSharp.Tables;


namespace ACadSvg {

	/// <summary>
	/// Represents the list of all <see cref="BlockRecordSvg"/> objects converted
	/// from the <see cref="BlockRecord"/> objects found in the DWG document header.
	/// </summary>
	public class BlockRecordsSvg : EntitySvg {

		public SortedSet<GroupSvg> Items = new SortedSet<GroupSvg>(Comparer<GroupSvg>.Create((x, y) => x.ID.CompareTo(y.ID)));


		public BlockRecordsSvg(ConversionContext ctx) : base(ctx) { }


		public bool Contains(string id) {
			foreach (GroupSvg group in Items) {
				if (group.ID == id) {
					return true;
				}
			}

			return false;
		}


		/// <inheritdoc />
		public override SvgElementBase ToSvgElement() {
			DefsElement defsElement = new DefsElement();
			foreach (GroupSvg group in Items) {
				defsElement.Children.Add(group.ToSvgElement());
			}
			return defsElement;
		}
	}
}
