// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DynamicData.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Data;
using System.Dynamic;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class DynamicData : DynamicObject, IDynamicData
	{
		public DynamicData()
			: base()
		{
			this.AsAny = false;
			this.EnableDynamic = true;
		}

		public DataRow Row { get; set; }
		public DataTable DataTable { get; set; }
		public Anys Record { get; set; }
		public bool AsAny { get; set; }
		public virtual bool EnableDynamic { get; set; }

		public override bool TryGetMember(
			GetMemberBinder binder, out object result)
		{
			return TryGetValue(binder.Name, out result);
		}

		public bool TryGetValue(string name, out object result)
		{
			if(null == Row && null == Record) {
				Logger.Error(
					"Entity.TryGetValue",
					"DataRow and IDataRecord are null reference!"
				);
				result = AsAny ? new Any(name, null) : null;
				return false;
			}

			bool success = false;

			if(null != Row) {
				success = Row.Table.Columns.Contains(name);
				if(success) {
					object value = Row.IsNull(name) ? null : Row[name];
					result = AsAny ? new Any(name, value) : value;
					if(!(result is Any)) {
						if(null != result) {
							result = result.ToString().Trim();
						}
					}
					return true;
				}

				// Remove Underscore
				foreach(DataColumn column in Row.Table.Columns) {
					string flatColumnName = column.ColumnName.Replace(
						DataConstants.Symbol.UnderScore, string.Empty
					);
					if(flatColumnName.Equals(
						name, StringComparison.OrdinalIgnoreCase)) {
						object value = Row.IsNull(column) ? null : Row[column];
						result = AsAny ? new Any(name, value) : value;
						if(!(result is Any)) {
							if(null != result) {
								result = result.ToString().Trim();
							}
						}
						return true;
					}
				}
			}

			if(null != Record) {
				success = Record.Exists(name);
				if(success) {
					object value = Record.GetValue(name);
					result = AsAny ? new Any(name, value) : value;
					if(!(result is Any)) {
						if(null != result) {
							result = result.ToString().Trim();
						}
					}
					return true;
				}

				// Remove Underscore
				foreach(Any any in Record) {
					string flatColumnName = any.Name.Replace(
						DataConstants.Symbol.UnderScore, string.Empty
					);
					if(flatColumnName.Equals(
						name, StringComparison.OrdinalIgnoreCase)) {
						object value = any.Value;
						result = AsAny ? new Any(name, value) : value;
						if(!(result is Any)) {
							if(null != result) {
								result = result.ToString().Trim();
							}
						}
						return true;
					}
				}
			}

			//
			result = AsAny ? new Any(name, null) : null;
			return success;
		}

		public dynamic AsDynamic()
		{
			AsAny = true;
			return this as dynamic;
		}
	}
}
