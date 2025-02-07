#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;
using ACadSharp.Tables.Collections;
using ACadSharp.XData;


namespace ACadSvg.Extensions {

    /// <summary>
    /// Base class for property classes. Propery classes provide entity properties
    /// having a standard value defined by the associated style object byt may be
    /// overridden either by explicit properties of the entity object or by the
    /// entity's extended data.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides methods to read entity's extended data. Extended data have to be
    /// interpreted as follows:
    /// </para>
    /// <para>
    /// Extended data is a <see cref="ExtendedDataDictionary"/>. Entries of type <see cref="ExtendedData"/>
    /// refer to an <see cref="AppId"/>. An <see cref="ExtendedData"/> entry contains a list
    /// of <see cref="ExtendedDataRecord"/> records. The first record contains the name
    /// of a "type", e.g., <i>DSTYLE</i> indicating that the values of the <see cref="ExtendedData"/>
    /// provide property values to override values of the <see cref="DimensionStyle"/> object.
    /// </para>
    /// <para>
    /// An <see cref="ExtendedDataRecord"/> contains a <see cref="ExtendedDataRecord.Code"/>
    /// and a <see cref="ExtendedDataRecord.Value"/>. The Code indicates the type of the value.
    /// The record provides no information about the meaning of the value. We assume that the
    /// preceding record contains a <i>group code</i> defining the meaning of the value of
    /// this record. The <i>group code</i> matches the <i>group code</i> of the property to
    /// be overridden.
    /// </para><para>
    /// Example:
    /// </para><para>
    /// When in AutoCAD for a LEADER a custom arrowsize is set, the value appears in an
    /// <see cref="ExtendedData"/> entry associated with the <see cref="AppId"/> key having the
    /// name <i>ACAD</i>. If an entry with the "type" record containing <i>DSTYLE</i> is found
    /// and a record with the <i>group code</i> <b>41</b> appears the following record contains
    /// the arrow-size value.
    /// </para>
    /// </remarks>
    internal abstract class EntityProperties {

        /// <summary>
        /// Tries to read a record from an <see cref="ExtendedData"/> entry with the specified
        /// <see cref="AppId"/> (<paramref name="appIdName"/>) and <paramref name="entryName"/>,
        /// then tries to find a record containing the group code specified by the <paramref name="field"/>
        /// parameter. If the specified record was found the value of the next record is returned.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="appIdName"></param>
        /// <param name="entryName"></param>
        /// <param name="field"></param>
        /// <param name="defaultValue"></param>
        /// <returns>
        /// The value from the specified record, when it was found; otherwise, the specified default value.
        /// </returns>
        public static T GetExtendedDataValue<T>(Entity entity, string appIdName, string entryName, short field, T defaultValue) {
            ExtendedDataRecord rec = getExtendedDataRecord(entity, appIdName, entryName, field);
            if (rec != null) {
                Type type = typeof(T);
                if (type.IsEnum) {
                    Type intType = type.GetEnumUnderlyingType();
                    var value = Convert.ChangeType(((ExtendedDataRecord<T>)rec).Value, intType);
                    if (type.IsEnumDefined(value)) {
                        return (T)value;
                    }
                }
                if (type == typeof(bool)) {
                    T tbValue = (T)Convert.ChangeType(((ExtendedDataRecord<short>)rec).Value != 0, typeof(bool));
                    return tbValue;
                }
                if (((ExtendedDataRecord<object>)rec).Value is T tValue) {
                    return tValue;
                }
            }
            return defaultValue;
        }


        /// <summary>
        /// Tries to read a record from an <see cref="ExtendedData"/> entry with the specified
        /// <see cref="AppId"/> (<paramref name="appIdName"/>) and <paramref name="entryName"/>,
        /// then tries to find a record containing the group code specified by the <paramref name="field"/>
        /// parameter. If the specified record was found the value of the next record is a handle.
        /// The the respective <see cref="CadObject"/> is looked up and returned.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="appIdName"></param>
        /// <param name="entryName"></param>
        /// <param name="field"></param>
        /// <param name="defaultValue"></param>
        /// <returns>
        /// The <see cref="BlockRecord"/> from the specified record, when it was found; otherwise,
        /// the specified default value. The default value may be null.
        /// </returns>
        public static BlockRecord GetExtendedDataBlockReference(Entity entity, string appIdName, string entryName, short field, BlockRecord defaultBlockRecord) {
            var rec = getExtendedDataRecord(entity, appIdName, entryName, field);
            if (rec == null) {
                return defaultBlockRecord;
            }
            var handle = ((ExtendedDataRecord<ulong>)rec).Value;
            if (!entity.Document.TryGetCadObject<BlockRecord>(handle, out BlockRecord blockReference)) {
                return defaultBlockRecord;
            }
            return blockReference;
        }


        /// <summary>
        /// Tries to read a record from an <see cref="ExtendedData"/> entry with the specified
        /// <see cref="AppId"/> <paramref name="appIdName"/> and the entry names passed in the
        /// <paramref name="entryName"/> parameter optionally suffixed with "_TC" The "_TC" suffix
        /// indicates the true-color value are provided. The suffixed entry, if present, is read
        /// first. 
        /// Then tries to find a record containing the group code specified by the <paramref name="field"/>
        /// parameter. If the specified record was found the value of the next record is returned.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="appIdName"></param>
        /// <param name="entryName"></param>
        /// <param name="field"></param>
        /// <param name="defaultValue"></param>
        /// <returns>
        /// The <see cref="Color"/> from the specified record, when it was found; otherwise,
        /// the specified default value.
        /// </returns>
        public static Color GetExtendedDataColor(Entity entity, string appIdName, string entryName, short field, Color defaultColor) {
            var recTc = getExtendedDataRecord(entity, appIdName + "_TC", entryName + "_TC", field);
            if (recTc != null) {
                byte[] bytes = ((ExtendedDataRecord<byte[]>)recTc).Value;
                var r = bytes[10];
                var g = bytes[9];
                var b = bytes[8];
                return new Color(r, g, b);
            }
            var rec = getExtendedDataRecord(entity, appIdName, entryName, field);
            if (rec != null) {
                return new Color(((ExtendedDataRecord<short>)rec).Value);
            }
            return defaultColor;
        }


        /// <summary>
        /// Gets a record from an <see cref="ExtendedData"/> entry with the specified
        /// <see cref="AppId"/> name and 
        /// </summary>
        private static ExtendedDataRecord getExtendedDataRecord(Entity entity, string appIdName, string entryName, short field) {
            ExtendedData extendedData = getExtendedData(entity, appIdName, entryName);
            if (extendedData == null) {
                return null;
            }
            bool fieldFound = false;
            foreach (ExtendedDataRecord record in extendedData.Records) {
                if (record.Code == DxfCode.ExtendedDataInteger16) {
                    if (((ExtendedDataRecord<short>)record).Value == field) {
                        fieldFound = true;
                    }
                }
                else if (fieldFound) {
                    //  If preceding record matched.
                    return record;
                }
            }
            return null;
        }


        private static ExtendedData getExtendedData(Entity entity, string appIdName, string entryName) {
            ExtendedDataDictionary extendedDataDict = entity.ExtendedData;
            var doc = entity.Document;
            AppIdsTable appIds = doc.AppIds;

            AppId appIdByName = null;
            foreach (var appId in appIds) {
                if (appId.Name.ToLower() == appIdName.ToLower()) {
                    appIdByName = appId;
                    break;
                }
            }
            if (appIdByName == null) {
                return null;
            }
            if (!extendedDataDict.TryGet(appIdByName, out ExtendedData extendedData)) {
                return null;
            }
            if (!string.IsNullOrEmpty(entryName) && ((ExtendedDataRecord<string>)extendedData.Records[0]).Value != entryName) {
                return null;
            }
            return extendedData;
        }
    }
}