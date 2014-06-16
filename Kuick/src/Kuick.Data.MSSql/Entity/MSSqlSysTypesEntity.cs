// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MSSqlSysTypesEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Runtime.Serialization;

namespace Kuick.Data.MSSql
{
	[DataContract]
	[EntitySpec]
	public class MSSqlSysTypesEntity : Entity<MSSqlSysTypesEntity>
	{
		#region const
		//public const string TABLE_NAME = "systypes";

		public const string NAME = "name";
		public const string XTYPE = "xtype";

		public const string _IMAGE = "image";
		public const string _TEXT = "text";
		public const string _UNIQUEIDENTIFIER = "uniqueidentifier";
		public const string _TINYINT = "tinyint";
		public const string _SMALLINT = "smallint";
		public const string _INT = "int";
		public const string _SMALLDATETIME = "smalldatetime";
		public const string _REAL = "real";
		public const string _MONEY = "money";
		public const string _DATETIME = "datetime";
		public const string _FLOAT = "float";
		public const string _SQL_VARIANT = "sql_variant";
		public const string _NTEXT = "ntext";
		public const string _BIT = "bit";
		public const string _DECIMAL = "decimal";
		public const string _NUMERIC = "numeric";
		public const string _SMALLMONEY = "smallmoney";
		public const string _BIGINT = "bigint";
		public const string _VARBINARY = "varbinary";
		public const string _VARCHAR = "varchar";
		public const string _BINARY = "binary";
		public const string _CHAR = "char";
		public const string _TIMESTAMP = "timestamp";
		public const string _NVARCHAR = "nvarchar";
		public const string _NCHAR = "nchar";
		public const string _XML = "xml";
		public const string _SYSNAME = "sysname";
		#endregion

		#region constructor
		public MSSqlSysTypesEntity()
		{
		}
		#endregion

		#region property
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		public string Name { get; set; }
		[DataMember]
		[ColumnSpec]
		public byte XType { get; set; }
		#endregion

		#region IEntity
		public override string TableName
		{
			get
			{
				return "systypes";
			}
		}

		public override string TitleValue
		{
			get
			{
				return Name;
			}
		}

		public override bool Alterable
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region class level
		public static MSSqlSysTypesEntity GetByXType(int xType)
		{
			return MSSqlSysTypesEntity.QueryFirst(x => x.XType == xType);
		}
		#endregion

		#region instance level
		#endregion
	}
}
