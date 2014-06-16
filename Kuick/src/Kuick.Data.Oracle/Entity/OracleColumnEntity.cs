// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleColumnEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Kuick.Data.Oracle
{
	[Serializable]
	[DataContract]
	[EntitySpec]
	public class OracleColumnEntity : Entity<OracleColumnEntity>
	{
		// table name
		public const string TABLE_NAME = "DBA_TAB_COLUMNS";
		// fields
		public const string NAME = "COLUMN_NAME";
		public const string TABLE = "TABLE_NAME";
		public const string OWNER = "OWNER";
		public const string DATA_TYPE = "DATA_TYPE";
		public const string DATA_LENGTH = "DATA_LENGTH";
		public const string NULLABLE = "NULLABLE";

		public OracleColumnEntity()
		{
		}

		#region property
		[DataMember]
		[ColumnSpec(NAME, SpecFlag.PrimaryKey, 32)]
		public string Name { get; set; }
		[DataMember]
		[ColumnSpec(TABLE, 32)]
		public new string Table { get; set; }
		[DataMember]
		[ColumnSpec(OWNER, 64)]
		public string Owner { get; set; }
		[DataMember]
		[ColumnSpec(DATA_TYPE, 64)]
		public string DataType { get; set; }
		[DataMember]
		[ColumnSpec(DATA_LENGTH)]
		public int DataLength { get; set; }
		[DataMember]
		[ColumnSpec(NULLABLE, 8)]
		public string Nullable { get; set; }
		#endregion

		#region IEntity
		public override bool Alterable
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region class level
		public static List<OracleColumnEntity> GetByTableName(string tableName)
		{
			return OracleColumnEntity
				.Sql()
				.Where(x => x.Table == tableName)
				.Query();
		}

		internal static List<OracleColumnEntity> GetAllInOneTable(string tableName)
		{
			return OracleColumnEntity
				.Sql()
				.Where(x => x.Table == tableName)
				.Query();
		}

		public static SqlDataType FromDataType(string dataType)
		{
			return FromXType(dataType, 1);
		}

		public static SqlDataType FromXType(string dataType, int length)
		{
			if(dataType.StartsWith("VARCHAR2")) {
				return SqlDataType.VarChar;
			} else if(dataType.StartsWith("NVARCHAR2")) {
				return SqlDataType.VarWChar;
			} else if(dataType.StartsWith("VARCHAR")) {
				return SqlDataType.VarChar;
			} else if(dataType.StartsWith("CHAR")) {
				return SqlDataType.Char;
			} else if(dataType.StartsWith("NCHAR")) {
				return SqlDataType.WChar;
			} else if(dataType.StartsWith("NUMBER")) {
				return SqlDataType.Integer;
			} else if(dataType.StartsWith("PLS_INTEGER")) {
				return SqlDataType.Integer;
			} else if(dataType.StartsWith("BINARY_INTEGER")) {
				return SqlDataType.Integer;
			} else if(dataType.StartsWith("LONG")) {
				return SqlDataType.MaxVarChar;
			} else if(dataType.StartsWith("DATE")) {
				return SqlDataType.TimeStamp;
			} else if(dataType.StartsWith("TIMESTAMP")) {
				return SqlDataType.TimeStamp;
			} else if(dataType.StartsWith("RAW")) {
				return SqlDataType.MaxVarBinary;
			} else if(dataType.StartsWith("LONG RAW")) {
				return SqlDataType.MaxVarBinary;
			} else if(dataType.StartsWith("ROWID")) {
				return SqlDataType.VarChar;
			} else if(dataType.StartsWith("UROWID")) {
				return SqlDataType.VarChar;
			} else if(dataType.StartsWith("MLSLABEL")) {
				return SqlDataType.MaxVarBinary;
			} else if(dataType.StartsWith("CLOB")) {
				return SqlDataType.MaxVarChar;
			} else if(dataType.StartsWith("NCLOB")) {
				return SqlDataType.MaxVarWChar;
			} else if(dataType.StartsWith("BLOB")) {
				return SqlDataType.MaxVarBinary;
			} else {
				return SqlDataType.Unknown;
			}
		}
		#endregion

		#region instance level
		public bool AlterableField(Column column)
		{
			ColumnSpec spec = column.Spec;

			if(OracleUtility.GetOracleDbTypeString(spec.DbType) != DataType) {
				return true;
			}

			if(
				spec.DbType != SqlDataType.Boolean
				&&
				spec.DbType != SqlDataType.Decimal
				&&
				spec.DbType != SqlDataType.Integer
				&&
				spec.DbType != SqlDataType.Enum
				&&
				spec.DbType != SqlDataType.MaxVarBinary
				&&
				spec.DbType != SqlDataType.MaxVarChar
				&&
				spec.DbType != SqlDataType.MaxVarWChar
				&&
				spec.DbType != SqlDataType.TimeStamp
				&&
				spec.Length != DataLength) {
				return true;
			}

			if(
				(!spec.NotAllowNull && (Nullable == "N"))
				||
				(spec.NotAllowNull && (Nullable == "Y"))) {
				return true;
			}

			return false;
		}

		public bool SpecChanged(Column column)
		{
			ColumnSpec spec = column.Spec;
			return
				(spec.DbType != FromXType(DataType, DataLength))
				||
				(spec.NotAllowNull == (Nullable == "Y"))
				||
				(spec.Length != DataLength);
		}
		#endregion
	}
}
