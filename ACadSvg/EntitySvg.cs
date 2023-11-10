#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Globalization;
using System.Text;

using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables.Collections;
using SvgElements;


namespace ACadSvg {

    /// <summary>
    /// The base class for classes representing converted ACad Entities.
    /// </summary>
    public abstract class EntitySvg {

        public string ID { get; set; }


        public string Class { get; set; }


        public virtual bool Skip {
            get { return false; }
        }


        public string Comment { get; set; }


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
            //var appId = appIds["AcDbBlockRepETag"];
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
            if (ctx.ConversionOptions.ExportLayerAsClass) {
                string className = Utils.CleanBlockName(entity.Layer.Name);
                Class = className;
            }
        }


        /// <summary>SVG-converter Factory: creates a entity-specific object implementing
        /// the conversion of a AutoCAD-DWG <see cref="Entity" /> into SVG text.
        /// The created object type is determined by the <see cref="CadObject.ObjectType"/>
        /// property.
        /// </summary>
        /// <param name="entity">The AutoCAD-DWG entity to be converterd.</param>
        /// <param name="groupsInDefs">The list of blocks and patterns can be used by
        /// <see cref="Hatch"/> entities.</param>
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


        public static SvgElementBase CreateSVG(
            ConversionContext ctx,
            bool strokeEnabled,
            string stroke,
            double strokeWidth,
            bool fillEnabled,
            string fill) {

            SvgElementBase svgElement = new SvgElement()
                .WithViewbox(
					ctx.ViewboxData.MinX,
					ctx.ViewboxData.MinY,
					ctx.ViewboxData.Width,
					ctx.ViewboxData.Height)
				.WithID("svg-element");

            if (strokeEnabled) {
                svgElement.WithStroke(stroke, strokeWidth);
            }

            if (fillEnabled) {
                svgElement.WithFill(fill);
            }
            else {
                svgElement.WithFill("none");
            }

            return svgElement;
        }
    }
}