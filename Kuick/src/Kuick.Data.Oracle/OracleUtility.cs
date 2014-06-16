// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleUtility.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data.Oracle
{
	public class OracleUtility
	{
		public static string GetOracleDbTypeString(SqlDataType type)
		{
			string dbType = "VARCHAR2";
			switch(type) {
				case SqlDataType.Bit:
					dbType = "NUMBER(1)"; // need confirm!
					break;
				case SqlDataType.Long:
				case SqlDataType.Integer:
					dbType = "NUMBER";
					break;
				case SqlDataType.TimeStamp:
					dbType = "DATE";
					break;
				case SqlDataType.MaxVarChar:
				case SqlDataType.MaxVarWChar:
					dbType = "CLOB";
					break;
				case SqlDataType.MaxVarBinary:
					dbType = "BLOB";
					break;
				case SqlDataType.Char:
				case SqlDataType.WChar:
				case SqlDataType.Decimal:
				case SqlDataType.VarWChar:
				case SqlDataType.VarChar:
				case SqlDataType.Enum:
				case SqlDataType.Uuid:
				default:
					dbType = "VARCHAR2";
					break;
			}
			return dbType;
		}
	}
}
