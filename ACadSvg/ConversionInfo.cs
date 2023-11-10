#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Text;

using ACadSharp.Entities;


namespace ACadSvg {

	/// <summary>
	/// Collects statistical and logging information during a conversion
	/// process.
	/// </summary>
	public class ConversionInfo {

		private StringBuilder _logSb = new StringBuilder();


		/// <summary>
		/// Specifies possible status of Entity conversion.
		/// </summary>
		public enum ConversionStatus {
			/// <summary>
			/// The conversion was successfully processed.
			/// </summary>
			Successful,
			/// <summary>
			/// The conversion failed due to an exception.
			/// </summary>
			Failed,
			/// <summary>
			/// The conversion of an entity has been skipped due to the value
			/// of the <see cref="EntitySvg"/>.<see cref="EntitySvg.Skip"/>
			/// property. Ths may be due to a condition evaluated by the
			/// entity and the Skip settings of the application.
			/// </summary>
			Skipped,
			/// <summary>
			/// The conversion of an entity could not be supressed because
			/// the entity is not supported by ACadSharp.
			/// </summary>
			NotSupported
        }


		/// <summary>
		/// Gets the number of entities that have been tried to be converted.
		/// This counter is inceremented by calls of the <see cref="RegisterConversion"/>
		/// method.
		/// </summary>
		public int TotalEntities { get; private set; } = 0;


		/// <summary>
		/// Gets the number of entities that have been successfully converted.
		/// This counter is inceremented by calls of the <see cref="RegisterConversion"/>
		/// method with status=<see cref="ConversionStatus.Successful"/>.
		/// </summary>
		public int SuccessfulEntityConversions { get; private set; } = 0;


		/// <summary>
		/// Receives the list of entity types and status occurring in the conversion.
		/// This list is filled with every call of the <see cref="RegisterConversion"/>
		/// method.
		/// </summary>
		public SortedSet<string> OccurringEntities { get; } = new SortedSet<string>();


		/// <summary>
		/// Gets the complete log from the last converion.
		/// </summary>
		/// <returns></returns>
		public string GetLog() {
			return _logSb.ToString();
		}


        internal void RegisterConversion(Entity entity, ConversionStatus status = ConversionStatus.Successful) {
			TotalEntities++;
			switch (status) {
			case ConversionStatus.Successful:
				string layerName = entity.Layer == null ? "not set" : entity.Layer.Name;
				string extendedDataInfo = EntitySvg.GetEntityExtendedDataInfo(entity);
				_logSb.AppendLine($"{DateTime.Now} {entity.Handle.ToString("X")}: Converted: {Utils.GetObjectType(entity)}, Layer: {layerName}");
				if (!string.IsNullOrEmpty(extendedDataInfo)) {
					_logSb.AppendLine($"  ExtendedData: {extendedDataInfo}");
				}
				OccurringEntities.Add(Utils.GetObjectType(entity));
				SuccessfulEntityConversions++;
				break;
			case ConversionStatus.Failed:
				break;
			case ConversionStatus.Skipped:
				OccurringEntities.Add(Utils.GetObjectType(entity) + " (skipped)");
				_logSb.AppendLine($"{DateTime.Now} {entity.Handle.ToString("X")}: Not supported: {Utils.GetObjectType(entity)}");
				break;
			case ConversionStatus.NotSupported:
				OccurringEntities.Add(Utils.GetObjectType(entity) + " (not supported)");
				break;
            }
		}


		/// <summary>
		/// Adds an entry with the specified message to the log. The log cen be
		/// retrieved with the <see cref="GetLog"/> method.
		/// </summary>
		/// <param name="message">The message to be added to the log.</param>
		internal void Log(string message) {
			_logSb.AppendLine($"{DateTime.Now} {message}");
		}
	}
}
