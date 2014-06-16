// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// PascalToUpperCasingConvention.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Reflection;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class PascalToUpperCasingConvention : ISchemaNamingConvention
	{
		private static object _TableLocking = new object();
		private static object _ColumnLocking = new object();
		private static Dictionary<string, string> _Tables = 
			new Dictionary<string, string>();
		private static Dictionary<string, string> _Columns = 
			new Dictionary<string, string>();

		#region ISchemaNamingConvention
		/// <summary>
		/// To get entity class table name.
		/// </summary>
		/// <param name="entityType">entity class type</param>
		/// <returns>table name</returns>
		public string ToTableName(Type entityType)
		{
			string key = entityType.Name;
			string tableName;

			if(!_Tables.TryGetValue(key, out tableName)) {
				lock(_TableLocking) {
					if(!_Tables.TryGetValue(key, out tableName)) {
						// convention
						// 1. Field: TABLE_NAME
						FieldInfo[] fis = entityType.GetFields();
						foreach(FieldInfo fi in fis) {
							bool liked = Checker.Like(
								DataConstants.Entity.TableName, fi.Name
							);
							if (liked) {
								tableName = fi.GetValue(null) as string;
								if(null != tableName) {
									_Tables.Add(key, tableName);
									return tableName;
								}
							}
						}

						// 2. Inner Class Property: Schema.TableName
						try {
							Type[] innerTypes = entityType.GetNestedTypes(
								BindingFlags.Public | BindingFlags.Instance
							);
							if(!Checker.IsNull(innerTypes)) {
								foreach(Type innerType in innerTypes) {
									if(innerType.Name == DataConstants.Entity.Schema) {
										FieldInfo[] fields = innerType.GetFields();
										foreach(FieldInfo field in fields) {
											bool liked = Checker.Like(
												DataConstants.Entity.TableName, 
												field.Name
											);
											if(liked) {
												tableName = 
													field.GetValue(null) as string;
												if(null != tableName) {
													_Tables.Add(key, tableName);
													return tableName;
												}
											}
										}
									}
								}
							}
						} catch(Exception ex) {
							Logger.Error(
								"PascalToUpperCasingConvention.ToTableName", ex
							);
						}

						// 3. Pascal naming
						tableName = string.Concat(
							"T_",
							Formator.PascalToUpperCasing(
								entityType.Name.TrimEnd(DataConstants.Entity.Suffix)
							)
						);
						_Tables.Add(key, tableName);
						//Logger.Track(
						//	"PascalToUpperCasingConvention.ToTableName",
						//	"Did not set the table name, the system automatically assign default value.",
						//	new Any("Entity Name", entityType.Name),
						//	new Any("Table Name", tableName)
						//);
					}
				}
			}
			return tableName;
		}

		/// <summary>
		/// To get entity class column name.
		/// </summary>
		/// <param name="entityType">entity class type</param>
		/// <param name="propertyInfo">entity property info</param>
		/// <returns>column name</returns>
		public string ToColumnName(Type entityType, PropertyInfo propertyInfo)
		{
			string key = string.Concat(entityType.Name, ":", propertyInfo.Name);
			string columnName;

			if(!_Columns.TryGetValue(key, out columnName)) {
				lock(_ColumnLocking) {
					if(!_Columns.TryGetValue(key, out columnName)) {
						// convention
						// 1. Field
						FieldInfo[] fis = entityType.GetFields();
						foreach(FieldInfo fi in fis) {
							if(Checker.Like(propertyInfo.Name, fi.Name)) {
								columnName = fi.GetValue(null) as string;
								if(null != columnName) {
									_Columns.Add(key, columnName);
									return columnName; 
								}
							}
						}

						try {
							Type[] innerTypes = entityType.GetNestedTypes(
								BindingFlags.Public | BindingFlags.Instance
							);
							if(!Checker.IsNull(innerTypes)) {
								foreach(Type innerType in innerTypes) {
									if(innerType.Name == DataConstants.Entity.Schema) {
										FieldInfo[] fields = innerType.GetFields();
										foreach(FieldInfo field in fields) {
											bool liked = Checker.Like(
												propertyInfo.Name, field.Name
											);
											if(liked) {
												columnName = 
													field.GetValue(null) as string;
												if(null != columnName) {
													_Columns.Add(key, columnName);
													return columnName;
												}
											}
										}
									}
								}
							}
						} catch(Exception ex) {
							Logger.Error(
								"PascalToUpperCasingConvention.ToColumnName",
								ex,
								new Any("entityType", entityType.Name),
								new Any("propertyInfo", propertyInfo.Name)
							);
						}

						// 3. Pascal naming
						columnName = Formator.PascalToUpperCasing(propertyInfo.Name);
						_Columns.Add(key, columnName);
						//Logger.Track(
						//	"PascalToUpperCasingConvention.ToColumnName",
						//	"Did not set the column name, the system automatically assign default value.",
						//	new Any("Entity Name", entityType.Name),
						//	new Any("property Name", propertyInfo.Name),
						//	new Any("Column Name", columnName)
						//);
					}
				}
			}

			return columnName;
		}
		#endregion
	}
}
