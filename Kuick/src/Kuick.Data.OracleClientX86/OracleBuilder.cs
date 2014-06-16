// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleDatabase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Data;
using Oracle.DataAccess.Client;

namespace Kuick.Data.OracleClientX86
{
	public class OracleBuilder : Kuick.Data.Oracle.OracleBuilder
	{
		public override void AddDbParameter(
			IDbCommand cmd, Column column, object value)
		{
			OracleCommand oracleCommand = cmd as OracleCommand;
			oracleCommand.Parameters.Add(
				BuildParameterName(column.Spec.ColumnName),
				ToDbType(column.Spec.DbType)
			).Value = value;
		}

		#region private
		public static OracleDbType ToDbType(SqlDataType type)
		{
			OracleDbType dbType = OracleDbType.Varchar2;
			switch(type) {
				case SqlDataType.Unknown:
					dbType = OracleDbType.Varchar2;
					break;
				case SqlDataType.Bit:
					dbType = OracleDbType.Int32;
					break;
				case SqlDataType.Boolean:
					dbType = OracleDbType.Varchar2;
					break;
				case SqlDataType.Char:
					dbType = OracleDbType.Char;
					break;
				case SqlDataType.Long:
					dbType = OracleDbType.Int32;
					break;
				case SqlDataType.Decimal:
					dbType = OracleDbType.Double;
					break;
				case SqlDataType.Double:
					dbType = OracleDbType.Double;
					break;
				case SqlDataType.Integer:
					dbType = OracleDbType.Int32;
					break;
				case SqlDataType.Enum:
					dbType = OracleDbType.Varchar2;
					break;
				case SqlDataType.MaxVarBinary:
					dbType = OracleDbType.Blob;
					break;
				case SqlDataType.MaxVarChar:
					dbType = OracleDbType.Clob;
					break;
				case SqlDataType.MaxVarWChar:
					dbType = OracleDbType.Clob;
					break;
				case SqlDataType.TimeStamp:
					dbType = OracleDbType.Date;
					break;
				case SqlDataType.VarChar:
					dbType = OracleDbType.Varchar2;
					break;
				case SqlDataType.VarWChar:
					dbType = OracleDbType.NVarchar2;
					break;
				case SqlDataType.WChar:
					dbType = OracleDbType.NChar;
					break;
				case SqlDataType.Uuid:
					dbType = OracleDbType.Varchar2;
					break;
				case SqlDataType.Identity:
					dbType = OracleDbType.Int32;
					break;
				default:
					dbType = OracleDbType.NVarchar2;
					break;
			}
			return dbType;
		}
		#endregion
	}
}
