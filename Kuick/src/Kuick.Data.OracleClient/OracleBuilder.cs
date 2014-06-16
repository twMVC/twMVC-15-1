// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleBuilder.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-10-25 - Creation


using System;
using System.Data;
using System.Data.OracleClient;

namespace Kuick.Data.OracleClient
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
		public static OracleType ToDbType(SqlDataType type)
		{
			OracleType dbType = OracleType.VarChar;
			switch(type) {
				case SqlDataType.Unknown:
					dbType = OracleType.VarChar;
					break;
				case SqlDataType.Bit:
					dbType = OracleType.Int32;
					break;
				case SqlDataType.Boolean:
					dbType = OracleType.VarChar;
					break;
				case SqlDataType.Char:
					dbType = OracleType.Char;
					break;
				case SqlDataType.Long:
					dbType = OracleType.Int32;
					break;
				case SqlDataType.Decimal:
					dbType = OracleType.Double;
					break;
				case SqlDataType.Double:
					dbType = OracleType.Double;
					break;
				case SqlDataType.Integer:
					dbType = OracleType.Int32;
					break;
				case SqlDataType.Enum:
					dbType = OracleType.VarChar;
					break;
				case SqlDataType.MaxVarBinary:
					dbType = OracleType.Blob;
					break;
				case SqlDataType.MaxVarChar:
					dbType = OracleType.Clob;
					break;
				case SqlDataType.MaxVarWChar:
					dbType = OracleType.Clob;
					break;
				case SqlDataType.TimeStamp:
					dbType = OracleType.DateTime;
					break;
				case SqlDataType.VarChar:
					dbType = OracleType.VarChar;
					break;
				case SqlDataType.VarWChar:
					dbType = OracleType.NVarChar;
					break;
				case SqlDataType.WChar:
					dbType = OracleType.NChar;
					break;
				case SqlDataType.Uuid:
					dbType = OracleType.VarChar;
					break;
				case SqlDataType.Identity:
					dbType = OracleType.Int32;
					break;
				default:
					dbType = OracleType.NVarChar;
					break;
			}
			return dbType;
		}
		#endregion
	}
}
