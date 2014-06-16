// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MSSqlSysColumnsEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Kuick.Data.MSSql
{
	[DataContract]
	[EntitySpec]
	public class MSSqlSysColumnsEntity : Entity<MSSqlSysColumnsEntity>
	{
		#region const
		//public const string TABLE_NAME = "syscolumns";

		public const string COLUMN_NAME = "name";
		public const string ID = "id";
		public const string XTYPE = "xtype";
		public const string LENGTH = "length";
		public const string IS_NULLABLE = "isnullable";
		#endregion

		#region constructor
		public MSSqlSysColumnsEntity()
		{
		}
		#endregion

		#region property
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		public string Name { get; set; }
		[DataMember]
		[ColumnSpec]
		public int Id { get; set; }
		[DataMember]
		[ColumnSpec]
		public byte XType { get; set; }
		[DataMember]
		[ColumnSpec]
		public short Length { get; set; } // -1 >> max
		[DataMember]
		[ColumnSpec]
		public int IsNullable { get; set; }
		#endregion

		#region IEntity
		public override string TableName
		{
			get
			{
				return "syscolumns";
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
		public static List<MSSqlSysColumnsEntity> GetColumns(int tableID)
		{
			return MSSqlSysColumnsEntity
				.Sql()
				.Where(x => x.Id == tableID)
				.Query();
		}

		/// XType   Name             Version
		/// ------+----------------+----------
		///  34     image            2000
		///  35     text             2000
		///  36     uniqueidentifier
		///  40     date
		///  41     time
		///  42     datetime2
		///  43     datetimeoffset
		///  48     tinyint
		///  52     smallint
		///  56     int
		///  58     smalldatetime
		///  59     real
		///  60     money
		///  61     datetime
		///  62     float
		///  98     sql_variant
		///  99     ntext            2000
		/// 104     bit
		/// 106     decimal
		/// 108     numeric
		/// 122     smallmoney
		/// 127     bigint
		/// 165     varbinary
		/// 167     varchar
		/// 173     binary
		/// 175     char
		/// 189     timestamp
		/// 231     nvarchar
		/// 231     sysname
		/// 239     nchar
		/// 240     hierarchyid
		/// 240     geometry
		/// 240     geography
		/// 241     xml
		public static SqlDataType FromXType(byte xType)
		{
			return FromXType(xType, 1);
		}

		public static SqlDataType FromXType(byte xType, int length)
		{
			switch(xType) {
				case 34: // image
					return SqlDataType.MaxVarBinary;
				case 35: // text
					return SqlDataType.MaxVarWChar;
				case 36: // uniqueidentifier
					return SqlDataType.Uuid;
				case 40: // date
					return SqlDataType.TimeStamp;
				case 41: // time
					return SqlDataType.TimeStamp;
				case 42: // datetime2
					return SqlDataType.TimeStamp;
				case 43: // datetimeoffset
					return SqlDataType.TimeStamp;
				case 48: // tinyint
					return SqlDataType.Integer;
				case 52: // smallint
					return SqlDataType.Integer;
				case 56: // int
					return SqlDataType.Integer;
				case 58: // smalldatetime
					return SqlDataType.TimeStamp;
				case 59: // real
					return SqlDataType.Decimal;
				case 60: // money
					return SqlDataType.Decimal;
				case 61: // datetime
					return SqlDataType.TimeStamp;
				case 62: // float
					return SqlDataType.Decimal;
				case 99: // ntext
					return SqlDataType.MaxVarWChar;
				case 104: // bit
					return SqlDataType.Bit;
				case 106: // decimal
					return SqlDataType.Decimal;
				case 108: // numeric
					return SqlDataType.Decimal;
				case 122: // smallmoney
					return SqlDataType.Decimal;
				case 127: // bigint
					return SqlDataType.Int64;
				case 165: // varbinary
					return SqlDataType.MaxVarBinary;
				case 167: // varchar
					return (length == -1) ? SqlDataType.MaxVarChar : SqlDataType.VarChar;
				case 173: // binary
					return SqlDataType.MaxVarBinary;
				case 175: // char
					return SqlDataType.Char;
				case 189: // timestamp
					return SqlDataType.TimeStamp;
				case 231: // nvarchar
					return (length == -1) ? SqlDataType.MaxVarWChar : SqlDataType.VarWChar;
				case 239: // nchar
					return SqlDataType.WChar;
				default:
					throw new NotImplementedException();
			}
		}
		#endregion

		#region instance level
		private MSSqlSysTypesEntity _DataType;
		public MSSqlSysTypesEntity DataType
		{
			get
			{
				if(Checker.IsNull(_DataType)) {
					_DataType = MSSqlSysTypesEntity.GetByXType(this.XType);
				}
				return _DataType;
			}
		}

		public bool AlterableField()
		{
			// SQL 2000
			if(this.XType.In(34, 35, 99)) { return false; }

			// SQL 2005/2008
			if(this.XType.In(165, 167, 231) && this.Length == -1) { return false; }

			return true;
		}

		public ColumnSpec ToSpec()
		{
			ColumnSpec spec = new ColumnSpec(this.Name, SqlDataType.WChar, this.Length);
			return spec;
		}

		public bool SpecChanged(Column column)
		{
			if(column.Spec.DbType == SqlDataType.Bit) {
				return
					(column.Spec.DbType != FromXType(XType, Length))
					||
					(column.Spec.NotAllowNull != (IsNullable == 1));
			} else {
				SqlDataType dbType = FromXType(XType, Length);
				if(column.Spec.LengthIsCritical) {
					int len = dbType.IsDBCS() && column.Spec.LengthIsCritical
						? Length / 2 : Length;
					return
						(column.Spec.CorrectedDBType != dbType)
						||
						(column.Spec.NotAllowNull == (IsNullable == 1))
						||
						(column.Spec.Length != len);
				} else {
					return
						(column.Spec.CorrectedDBType != dbType)
						||
						(column.Spec.NotAllowNull == (IsNullable == 1));
				}
			}
		}
		#endregion
	}
}
