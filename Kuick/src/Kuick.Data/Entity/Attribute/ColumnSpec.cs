// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ColumnSpec.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ColumnSpec : Attribute, ICloneable<ColumnSpec>
	{
		#region constructor
		public ColumnSpec()
			: this(string.Empty, SqlDataType.Unknown, SpecFlag.None, -1)
		{
		}

		public ColumnSpec(string columnName)
			: this(columnName, SqlDataType.Unknown, SpecFlag.None, -1)
		{
		}

		public ColumnSpec(SqlDataType dbType)
			: this(string.Empty, dbType, SpecFlag.None, -1)
		{
		}

		public ColumnSpec(SpecFlag flag)
			: this(string.Empty, SqlDataType.Unknown, flag, -1)
		{
		}

		public ColumnSpec(int length)
			: this(string.Empty, SqlDataType.Unknown, SpecFlag.None, length)
		{
		}

		public ColumnSpec(string columnName, SqlDataType dbType)
			: this(columnName, dbType, SpecFlag.None, -1)
		{
		}

		public ColumnSpec(string columnName, SpecFlag flag)
			: this(columnName, SqlDataType.Unknown, flag, -1)
		{
		}

		public ColumnSpec(string columnName, int length)
			: this(columnName, SqlDataType.Unknown, SpecFlag.None, length)
		{
		}

		public ColumnSpec(SqlDataType dbType, SpecFlag flag)
			: this(string.Empty, dbType, flag, -1)
		{
		}

		public ColumnSpec(SqlDataType dbType, int length)
			: this(string.Empty, dbType, SpecFlag.None, length)
		{
		}

		public ColumnSpec(SpecFlag flag, int length)
			: this(string.Empty, SqlDataType.Unknown, flag, length)
		{
		}

		public ColumnSpec(string columnName, SqlDataType dbType, SpecFlag flag)
			: this(columnName, dbType, flag, -1)
		{
		}

		public ColumnSpec(string columnName, SqlDataType dbType, int length)
			: this(columnName, dbType, SpecFlag.None, length)
		{
		}

		public ColumnSpec(string columnName, SpecFlag flag, int length)
			: this(columnName, SqlDataType.Unknown, flag, length)
		{
		}

		public ColumnSpec(SqlDataType dbType, SpecFlag flag, int length)
			: this(string.Empty, dbType, flag, length)
		{
		}

		public ColumnSpec(string columnName, SqlDataType dbType, SpecFlag flag, int length)
		{
			this.ColumnName = columnName;
			this.DbType = dbType;
			this.Flag = flag;
			this.Length = length;

			this.NotAllowNull = Checker.Flag.Check((int)flag, (int)SpecFlag.NotAllowNull);
			this.ReadOnly = Checker.Flag.Check((int)flag, (int)SpecFlag.ReadOnly);
			this.Identity = Checker.Flag.Check((int)flag, (int)SpecFlag.Identity);
			this.PrimaryKey = Checker.Flag.Check((int)flag, (int)SpecFlag.PrimaryKey);

			// correcting
			if(this.PrimaryKey) {
				this.NotAllowNull = true;
				this.ReadOnly = true;
			}
		}
		#endregion

		#region ICloneable<T>
		public ColumnSpec Clone()
		{
			return new ColumnSpec(ColumnName, DbType, Flag, Length);
		}
		#endregion

		#region property
		public string ColumnName { get; internal set; }
		public SqlDataType DbType { get; internal set; }
		public SpecFlag Flag { get; internal set; }
		public int Length { get; internal set; }

		// Parent
		public Column Column { get; internal set; }
		#endregion

		#region assistance
		public bool NotAllowNull { get; set; }
		public bool ReadOnly { get; internal set; }
		public bool Identity { get; internal set; }
		public bool PrimaryKey { get; internal set; }

		public bool LengthIsCritical
		{
			get
			{
				return DbType.EnumIn(
					SqlDataType.Char, 
					SqlDataType.Enum, 
					SqlDataType.Uuid,
					SqlDataType.VarChar,
					SqlDataType.VarWChar,
					SqlDataType.WChar
				);
			}
		}

		public SqlDataType CorrectedDBType
		{
			get
			{
				switch(DbType) {
					case SqlDataType.Unknown:
						return SqlDataType.Unknown;
					case SqlDataType.Boolean:
						return DbType;
					case SqlDataType.Char:
						return DbType;
					case SqlDataType.Bit:
						return DbType;
					case SqlDataType.Long:
						return DbType;
					case SqlDataType.Decimal:
						return DbType;
					case SqlDataType.Integer:
						return DbType;
					case SqlDataType.Enum:
						return SqlDataType.VarChar;
					case SqlDataType.MaxVarBinary:
						return DbType;
					case SqlDataType.MaxVarChar:
						return DbType;
					case SqlDataType.MaxVarWChar:
						return DbType;
					case SqlDataType.TimeStamp:
						return DbType;
					case SqlDataType.VarChar:
						return DbType;
					case SqlDataType.VarWChar:
						return DbType;
					case SqlDataType.WChar:
						return DbType;
					case SqlDataType.Uuid:
						return SqlDataType.VarChar;
					case SqlDataType.Identity:
						return SqlDataType.Integer;
					case SqlDataType.Double:
						return DbType;
					default:
						throw new NotImplementedException();
				}
			}
		}
		#endregion
	}
}
