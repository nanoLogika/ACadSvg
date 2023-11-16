#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp.Entities;
using SvgElements;


namespace ACadSvg {

    /// <summary>
    /// Represents an SVG element converted from an ACad <see cref="AttributeDefinition"/> entity.
    /// Conversion of this entity is not yet supported.
    /// </summary>
    internal class AttributeDefinitionSvg : EntitySvg {

        private AttributeDefinition _attributeDefinition;


        /// <summary>
		/// Initializes a new instance of the <see cref="AttributeDefinitionSvg"/> class
		/// for the specified <see cref="AttributeDefinition"/> entity.
        /// </summary>
        /// <param name="attributeDefinition">The <see cref="AttributeDefinition"/> entity to be converted.</param>
        /// <param name="ctx">This parameter is not used in this class.</param>
        public AttributeDefinitionSvg(Entity attributeDefinition, ConversionContext ctx) {
            _attributeDefinition = (AttributeDefinition)attributeDefinition;
        }


        /// <inheritdoc />
        public override SvgElementBase ToSvgElement() {
            Comment = $"ObjectType: {_attributeDefinition.ObjectType} not supported";
            return null;
		}


        /// <inheritdoc />
        public override string ToSvg() {
            return string.Empty;
        }
    }
}