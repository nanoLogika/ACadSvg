#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Text;

using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using ACadSharp.Tables.Collections;

using SvgElements;


namespace ACadSvg {

    /// <summary>
    /// The base class for classes representing converted ACad entities.
    /// </summary>
    public abstract class EntitySvg {

        /// <summary>
        /// Gets or sets a value for the <i>id</i> attribute
        /// </summary>
        public string ID { get; set; }


        /// <summary>
        /// Gets or sets a value for the <i>class</i> attribute
        /// </summary>
        public string Class { get; set; }


        /// <summary>
        /// Gets a value indicating whether this entity is to be skipped
        /// and excluded from the conversion. The standard value is <b>false</b>.
        /// A derived class may implement a contion to exclude the entity from
        /// conversion.
        /// </summary>
        public virtual bool Skip {
            get { return false; }
        }


        /// <summary>
        /// Gets or sets a comment text that is to be created as comment element
        /// before this element.
        /// </summary>
        public string Comment { get; set; }


        /// <summary>
        /// Converts  list of ACad entities to SVG elements.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        protected static IList<EntitySvg> ConvertEntitiesToSvg(IList<Entity> entities, ConversionContext ctx) {

            IList<EntitySvg> convertedEntities = new List<EntitySvg>();
            foreach (Entity entity in entities) {
                var entitySvg = CreateEntitySvg(entity, ctx);
                if (entitySvg == null) {
                    ctx.ConversionInfo.RegisterConversion(entity, ConversionInfo.ConversionStatus.NotSupported);
                }
                else if (entitySvg.Skip) {
                    ctx.ConversionInfo.RegisterConversion(entity, ConversionInfo.ConversionStatus.Skipped);
                }
                else {
                    convertedEntities.Add(entitySvg);
                    ctx.ConversionInfo.RegisterConversion(entity);
                }
            }

            return convertedEntities;
        }


        //  Reserved for future use 
        public static string GetEntityExtendedDataInfo(CadObject entity) {
            StringBuilder exdSb = new StringBuilder();
            ExtendedDataDictionary extendedData = entity.ExtendedData;
            var doc = entity.Document;
            AppIdsTable appIds = doc.AppIds;
            foreach (var appId in appIds) {
                if (extendedData.ContainsKey(appId)) {
                    exdSb.AppendLine();
                    exdSb.Append("    ").Append(appId.Name).Append(":: ");
                    IList<ExtendedDataRecord> exd = extendedData.Get(appId).Data;
                    foreach (var e in exd) {
                        exdSb.AppendLine();
                        exdSb.Append($"      {e.Code.ToString()}: {e.Value.ToString()}");
                        //exdSb.Append(" . ");
                    }
                }
            }

            return exdSb.ToString();
        }


        /// <summary>
        /// Gets a record from an <see cref="ExtendedData"/> dictionary with the specified
        /// <see cref="AppId"/> name and 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="appIdName"></param>
        /// <param name="entryName"></param>
        /// <param name="field"></param>
        /// <returns>A <see cref="ExtendedDataRecord"/> or null when the specified record was not found.</returns>
        /// <remarks>
        /// <para>
        /// An <see cref="ExtendedDataRecord"/> contains a <see cref="ExtendedDataRecord.Code"/>
        /// and a <see cref="ExtendedDataRecord.Value"/>. The Code indicates the type of the value.
        /// The record provides no information about the meaning of the value. We assume that the
        /// preceding record tells what the value of the value ist to be used for. The <paramref name="field"/>
        /// is the value of the preceding field, thus specified the next record to be returned.
        /// </para><para>
        /// Example:
        /// </para><para>
        /// When in AutoCAD for a LEADER a custom arrowsize is set, the value appears in an
        /// <see cref="ExtendedData"/> dictionary associated with the <see cref="AppId"/> having the
        /// name <i>ACAD</i>. The <see cref="ExtendedData"/> entries contain a list of
        /// <see cref="ExtendedDataRecord"/> records. The first record contains the name
        /// <i>DSTYLE</i>. One of the next records contains the desired arrowsize. The
        /// preceding record contains the value <i>41</i>. We believe that this indicates that the
        /// desired data are in the next record.
        /// </para>
        /// </remarks>
        protected ExtendedDataRecord GetExtendedDataRecord(Entity entity, string appIdName, string entryName, short field) {
            ExtendedData extendedData = getExtendedData(entity, appIdName, entryName);
            if (extendedData == null) {
                return null;
            }
            bool fieldFound = false;
            foreach (ExtendedDataRecord record in extendedData.Data) {
                if (record.Code == DxfCode.ExtendedDataInteger16 && (short)record.Value == field) {
                    fieldFound = true;
                }
                else if (fieldFound) {
                    //  If preceding record matched.
                    return record;
                }
            }
            return null;
        }


        //protected ExtendedDataRecord GetExtendedData(Entity entity, string appIdName, int index) {
        //    ExtendedData extendedData = getExtendedDataDictionary(entity, appIdName, "");
        //    return extendedData.Data[index];
        //}


        private ExtendedData getExtendedData(Entity entity, string appIdName, string entryName) {
            ExtendedDataDictionary extendedDataDict = entity.ExtendedData;
            var doc = entity.Document;
            AppIdsTable appIds = doc.AppIds;

            AppId appIdByName = null;
            foreach (var appId in appIds) {
                if (appId.Name == appIdName) {
                    appIdByName = appId;
                    break;
                }
            }
            if (appIdByName == null) {
                return null;
            }
            if (!extendedDataDict.TryGet(appIdByName, out ExtendedData extendedDataValue)) {
                return null;
            }
            if (!string.IsNullOrEmpty(entryName) && extendedDataValue.Data[0].Value.ToString() != entryName) {
                return null;
            }
            return extendedDataValue;
        }


        /// <summary>
        /// Converts an ACad <see cref="Entity"/> into a SVG-element object.
        /// This may be be a primitive such as <i>path</i> or <i>circle</i>.
        /// Complex entities convert into a SVG group (<i>g</i>) containing
        /// primitives.
        /// </summary>
        /// <returns>A <see cref="SvgElementBase"/>.</returns>
        public abstract SvgElementBase ToSvgElement();


        /// <summary>
        /// Retuns the SVG element converted from the respective ACad Entity
        /// as string.
        /// </summary>
        /// <returns></returns>
        public virtual string ToSvg() {
            return ToSvgElement().ToString();
        }


        /// <summary>
        /// Formats a double value with a decimal dot as needed in numerical attribute
        /// of a SVG element.
        /// </summary>
        /// <param name="val"></param>
        /// <returns>A string containing the formatted double value.</returns>
        protected static string Cd(double val) {
            return SvgElementBase.Cd(val);
        }


        /// <summary>
        /// Set the <see cref="Entity.Handle"/> value of the <see cref="Entity"/> as
        /// value for the <i>id</i> attribute when the Use-handle-as-id" option is set.
        /// Set the name of the <see cref="Entity.Layer"/> as value for the <i>class</i>
        /// attribute.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ctx"></param>
        protected void SetStandardIdAndClassIf(Entity entity, ConversionContext ctx) {
            if (ctx.ConversionOptions.ExportHandleAsID) {
                ID = entity.Handle.ToString("X");
            }
            Class = string.Empty;
            if (ctx.ConversionOptions.ExportLayerAsClass) {
                string className = Utils.CleanBlockName(entity.Layer.Name);
                Class += "L_" + className;
            }
            if (ctx.ConversionOptions.ExportObjectTypeAsClass) {
                string objectType = Utils.GetObjectType(entity);
                if (!string.IsNullOrEmpty(Class)) {
                    Class += " ";
                }
                Class += objectType;
            }
        }


        /// <summary>SVG-converter Factory: creates a entity-specific object implementing
        /// the conversion of a AutoCAD-DWG <see cref="Entity" /> into SVG text.
        /// The created object type is determined by the <see cref="CadObject.ObjectType"/>
        /// property.
        /// </summary>
        /// <param name="entity">The AutoCAD-DWG entity to be converterd.</param>
        /// <param name="ctx">The conversion context provides several options and data required for the conversion.</param>
        /// <returns>An entity-specific converter object or null if the entity to be
        /// converted is not supported.</returns>
        /// 
        public static EntitySvg CreateEntitySvg(Entity entity, ConversionContext ctx) {
            switch (entity.ObjectType) {
            case ObjectType.ATTDEF:
                return new AttributeDefinitionSvg(entity, ctx);

            case ObjectType.POINT:
                return new PointSvg(entity, ctx);

            case ObjectType.LINE:
                return new LineSvg(entity, ctx);

            case ObjectType.LWPOLYLINE:
                return new LwPolylineSvg(entity, ctx);
                
            case ObjectType.POLYLINE_2D:
                return new Polyline2DSvg(entity, ctx);

            case ObjectType.POLYLINE_3D:
                return new Polyline3DSvg(entity, ctx);

            case ObjectType.CIRCLE:
                return new CircleSvg(entity, ctx);

            case ObjectType.ELLIPSE:
                return new EllipseSvg(entity, ctx);

            case ObjectType.ARC:
                return new ArcSvg(entity, ctx);

            case ObjectType.SPLINE:
                return new SplineSvg(entity, ctx);

            case ObjectType.TEXT:
                return new TextEntitySvg(entity, ctx);

            case ObjectType.MTEXT:
                return new MTextSvg(entity, ctx);

            case ObjectType.INSERT:
                return new InsertSvg(entity, ctx);
           
            case ObjectType.DIMENSION_LINEAR:
                return new DimensionLinearSvg(entity, ctx);

            case ObjectType.DIMENSION_ANG_3_Pt:
                return new DimensionAngular3PtSvg(entity, ctx);

            case ObjectType.LEADER:
                return new LeaderSvg(entity, ctx);

            case ObjectType.HATCH:
                return new HatchSvg(entity, ctx);

            case ObjectType.SOLID:
                return new SolidSvg(entity, ctx);

            case ObjectType.UNLISTED:
                switch (entity.ObjectName) {
                case "MLEADER":
                    return new MultiLeaderSvg(entity, ctx);
                default:
                    return null;
                }

            default:
                return null;
            }
        }
    }
}