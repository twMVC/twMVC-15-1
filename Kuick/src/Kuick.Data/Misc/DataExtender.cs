// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DataExtender.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using System.Reflection;
using System.Text;

namespace Kuick.Data
{
	public static class DataExtender
	{
		#region String
		public static string Replace(this string template, params Entity[] instances)
		{
			StringBuilder sb = new StringBuilder(template);
			if(!Checker.IsNull(instances)) {
				foreach(Entity instance in instances) {
					foreach(Column column in instance.Columns) {
						string value = instance.GetValue(column).ToString();
						sb.Replace( // {T_USER.NAME}
							string.Concat(
								Constants.Symbol.OpenBrace, 
								column.FullName, 
								Constants.Symbol.CloseBrace
							), 
							value
						);
						sb.Replace( // {UserEntity.Name}
							string.Concat(
								Constants.Symbol.OpenBrace,
								column.PropertyFullName,
								Constants.Symbol.CloseBrace
							),
							value
						);
					}
				}
			}
			return sb.ToString();
		}

		public static string Replace(this string template, params Any[] anys)
		{
			StringBuilder sb = new StringBuilder(template);
			if(!Checker.IsNull(anys)) {
				foreach(Any any in anys) {
					sb.Replace( // {Name}
						string.Concat(
							Constants.Symbol.OpenBrace,
							any.Name, 
							Constants.Symbol.CloseBrace
						), 
						any.ToString()
					);
				}
			}
			return sb.ToString();
		}
		#endregion

		#region IEntity[]
		public static List<string> PrimaryKeys(this IEntity[] instances)
		{
			List<string> pks = new List<string>();
			foreach(IEntity x in instances) {
				pks.Add(x.KeyValue);
			}
			return pks;
		}

		public static string ToJson(this List<IEntity> objs)
		{
			if(objs == null || objs.Count == 0) { return "[]"; }
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			foreach(IEntity obj in objs) {
				if(sb.Length > 1) { sb.Append(","); }
				if(obj == null) { sb.Append("{}"); }
				sb.Append(obj.ToJson());
			}
			sb.Append("]");
			return sb.ToString();
		}
		#endregion

		#region SqlDataType
		public static bool IsDBCS(this SqlDataType dbType)
		{
			return dbType.EnumIn(
				SqlDataType.MaxVarWChar,
				SqlDataType.VarWChar,
				SqlDataType.WChar
			);
		}
		#endregion

		#region DataSet, DataTable, DataRow
		public static List<DynamicData> ToDynamic(this DataSet ds)
		{
			if(null == ds || ds.Tables.Count == 0) {
				return new List<DynamicData>();
			}
			return ds.Tables[0].ToDynamic();
		}
		public static List<DynamicData> ToDynamic(this DataTable table)
		{
			if(null == table || table.Rows.Count == 0) {
				return new List<DynamicData>();
			}

			List<DynamicData> list = new List<DynamicData>();
			foreach(DataRow row in table.Rows) {
				list.Add(row.ToDynamic());
			}

			return list;
		}

		public static DynamicData ToDynamic(this DataRow row)
		{
			DynamicData data = new DynamicData();
			data.Row = row;
			return data;
		}

		public static List<T> ToEntity<T>(this DataSet ds)
			where T : class, IEntity, new()
		{
			if(null == ds || ds.Tables.Count < 1) { return null; }
			DataTable dt = ds.Tables[0];
			return dt.ToEntity<T>();
		}

		public static List<T> ToEntity<T>(this DataTable dt)
			where T : class, IEntity, new()
		{
			try {
				if(null == dt || dt.Rows.Count == 0) { return new List<T>(); }

				List<T> list = new List<T>();
				foreach(DataRow row in dt.Rows) {
					T one = row.ToEntity<T>();
					list.Add(one);
				}
				return list;
			} catch {
				return new List<T>();
			}
		}

		public static T ToEntity<T>(this DataRow row)
			where T : class, IEntity, new()
		{
			#region current
			if(null == row) { return default(T); }
			T one = new T();
			one.Row = row;
			foreach(Column column in one.Columns) {
				try {
					one.SetValue(column, row[column.Spec.ColumnName]);
				} catch {
					// swallow
				}
			}
			return one;
			#endregion

			#region obosolete
			//if(null == row) { return default(T); }

			//Hashtable dic = new Hashtable();
			//// User_Name >> USERNAME
			//string propName = null;
			//string colName = null;
			//foreach(DataColumn col in row.Table.Columns) {
			//    colName = col.ColumnName;
			//    propName = colName.Replace("_", "");
			//    propName = propName.ToUpper();
			//    dic.Add(propName, colName);
			//}

			//T one = new T();
			//Type type = typeof(T);
			//PropertyInfo[] infos = type.GetProperties();
			//foreach(PropertyInfo info in infos) {
			//    if(!info.CanWrite) { continue; }
			//    colName = (string)dic[info.Name.ToUpper()];
			//    if(null == colName) { continue; }

			//    Type propType = info.PropertyType;
			//    Object val = row[colName];
			//    if(null == val) { continue; }

			//    val = Convert.ChangeType(val, propType);
			//    if(propType.IsBoolean()) { val = val.Equals(Constants.StringBoolean.True); }

			//    Reflector.SetValue(one, info, val);
			//}

			//Reflector.SetValue(one, "Row", row);
			//return one;
			#endregion
		}
		#endregion

		#region ToDataTable
		public static DataTable ToDataTable<T>(this List<T> instances)
			where T : IEntity
		{
			if(null == instances) { return null; }
			if(instances.Count == 0) { return new DataTable(); }
			return instances[0].DataTable;

			// old
			//DataTable dt = new DataTable();
			//foreach(T instance in instances) {
			//    dt.Rows.Add(instance.Row);
			//}
			//return dt;
		}
		#endregion
	}
}
