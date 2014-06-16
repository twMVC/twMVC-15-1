// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MSSqlBuilder.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Kuick.Data.MSSql
{
	public class MSSqlBuilder : SqlBuilder
	{
		public override string Vender
		{
			get
			{
				return MSSqlConstants.Vender;
			}
		}
		public override string OpenTag
		{
			get
			{
				return MSSqlConstants.Symbol.OpenBracket;
			}
		}
		public override string CloseTag
		{
			get
			{
				return MSSqlConstants.Symbol.CloseBracket;
			}
		}
		public override string Wildcard
		{
			get
			{
				return MSSqlConstants.Symbol.Percent;
			}
		}
		public override string ParameterPrefix
		{
			get
			{
				return MSSqlConstants.Symbol.At;
			}
		}
		public override string ParameterSuffix
		{
			get
			{
				return string.Empty;
			}
		}

		public override string BuildLiteral(SqlLiteral literal)
		{
			const string PATTERN = @"DISTINCT CONVERT(VARCHAR({0}), {1}, 21) AS {2}";

			switch(literal.Format) {
				case SqlLiteralFormat.Command:
					return string.Concat(
						literal.CommandText,
						string.IsNullOrEmpty(literal.AsName) 
							? "" 
							: " AS " + literal.AsName
					);
				case SqlLiteralFormat.DistinctDate:
					string taggedColumnName = Tag(literal.ColumnName);
					string taggedAsName = Tag(literal.AsName);
					switch(literal.Range) {
						case SqlDistinctDate.Year:
							return string.Format(
								PATTERN, 4, taggedColumnName, taggedAsName
							);
						case SqlDistinctDate.YearMonth:
							return string.Format(
								PATTERN, 7, taggedColumnName, taggedAsName
							);
						case SqlDistinctDate.YearMonthDay:
							return string.Format(
								PATTERN, 10, taggedColumnName, taggedAsName
							);
						default:
							throw new NotImplementedException();
					}
				default:
					throw new NotImplementedException();
			}
		}

		public override string BuildValue(DateTime value)
		{
			return string.Format(
				@"CONVERT(DATETIME, '{0}', 21)",
				value.ToString("yyyy-MM-dd HH:mm:ss.fff")
			);
		}

		public override string BuildTopCommand(string command, int top)
		{
			string sql = command.ToUpper().Trim();

			int i = sql.IndexOf(MSSqlConstants.Sql.Select);
			if(i < 0) {
				throw new Exception(string.Format(
					"Keyword '{0}' was not found.", MSSqlConstants.Sql.Select
				));
			}
			int j = sql.IndexOf(MSSqlConstants.Sql.Distinct);
			StringBuilder sb = new StringBuilder(sql);
			int k = j < 0
				? i + MSSqlConstants.Sql.Select.Length
				: j + MSSqlConstants.Sql.Distinct.Length;
			sb = sb.Insert(k, String.Format(" TOP {0}", top));
			return sb.ToString();
		}

		public override void AddDbParameter(
			IDbCommand cmd, Column column, object value)
		{
			SqlCommand sqlCommand = cmd as SqlCommand;
			if(null == sqlCommand) { return; }
			sqlCommand.Parameters.Add(
				BuildParameterName(column.Spec.ColumnName),
				ToDbType(column.Spec.DbType)
			).Value = value;
		}


		#region Build CommandText
		public override string BuildCreateTableCommandText(IEntity schema)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(
				@"CREATE TABLE {0}",
				Tag(schema.TableName)
			);

			bool firstTime = true;
			sb.Append("(" + Environment.NewLine);
			foreach(Column column in schema.Columns) {
				if(!schema.DynamicDisplay(column.Spec.ColumnName)) { continue; }

				sb.AppendFormat(
					"\t{0}{1}{2}",
					(firstTime ? " " : ","),
					ColumnSqlCommand(column),
					Environment.NewLine
				);
				if(firstTime) { firstTime = false; }
			}

			// primary key
			sb.AppendFormat(
				@"	{0}CONSTRAINT [{1}_PK] PRIMARY KEY CLUSTERED({2}",
				(firstTime ? " " : ","),
				schema.TableName,
				Environment.NewLine
			);
			bool pkFirstTime = true;
			foreach(Column column in schema.KeyColumns) {
				sb.AppendFormat(
					@"		{0}{1} ASC{2}",
					(pkFirstTime ? " " : ","),
					Tag(column.Spec.ColumnName),
					Environment.NewLine
				);
				if(pkFirstTime) { pkFirstTime = false; }
			}
			sb.AppendFormat("\t){0}", Environment.NewLine);

			sb.Append(")");

			return sb.ToString();
		}

		public override string BuildCreateIndexCommandText(EntityIndex index)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(
				@"CREATE {0}INDEX {1} ON {2}",
				(index.Unique ? "UNIQUE " : string.Empty),
				index.Table.BuildIndexName(index),
				Tag(index.Table.TableName)
			);

			bool firstTime = true;
			sb.Append("(");
			foreach(string columnName in index.ColumnNames) {
				sb.AppendLine();
				sb.AppendFormat(
					@"	{0}{1}",
					(firstTime ? " " : ","),
					Tag(columnName)
				);
				if(firstTime) { firstTime = false; }
			}
			sb.AppendLine();
			sb.Append(")");

			return sb.ToString();
		}

		public override string BuildAlterColumnCommandText(Column column)
		{
			string sql = String.Format(
				@"ALTER TABLE {0} ALTER COLUMN {1}",
				Tag(column.TableName),
				ColumnSqlCommand(column)
			);
			return sql;
		}

		public override string BuildAddColumnCommandText(Column column)
		{
			string sql = String.Format(
				@"ALTER TABLE {0} ADD {1}",
				Tag(column.TableName),
				ColumnSqlCommand(column)
			);
			return sql;
		}
		#endregion

		#region private
		private string ColumnSqlCommand(Column column)
		{
			DbSetting setting = DbSettingCache.Get(column.EntityName);
			bool isSQL2000 = IsSQL2000(setting);
			StringBuilder sb = new StringBuilder();
			sb.Append(Tag(column.Spec.ColumnName));
			sb.Append(" ");

			int len = column.Spec.Length;
			switch(column.Spec.DbType) {
				case SqlDataType.Unknown:
					break;
				case SqlDataType.Boolean:
					sb.AppendFormat("VARCHAR({0})", 5);
					break;
				case SqlDataType.Char:
					sb.AppendFormat("CHAR({0})", len);
					break;
				case SqlDataType.Bit:
					sb.Append("BIT");
					break;
				case SqlDataType.Long:
					sb.Append("BIGINT");
					break;
				case SqlDataType.Decimal:
					sb.AppendFormat("DECIMAL({0},{1})", len + 6, 6);
					break;
				case SqlDataType.Double:
					sb.Append("FLOAT");
					break;
				case SqlDataType.Integer:
					sb.Append("INT");
					break;
				case SqlDataType.Enum:
					sb.AppendFormat("VARCHAR({0})", 128);
					break;
				case SqlDataType.MaxVarBinary:
					sb.Append(isSQL2000 ? "IMAGE" : " VARBINARY(MAX)");
					break;
				case SqlDataType.MaxVarChar:
					sb.Append(isSQL2000 ? "TEXT" : " VARCHAR  (MAX)");
					break;
				case SqlDataType.MaxVarWChar:
					sb.Append(isSQL2000 ? "NTEXT" : " NVARCHAR (MAX)");
					break;
				case SqlDataType.TimeStamp:
					sb.Append("DATETIME");
					break;
				case SqlDataType.VarChar:
					sb.AppendFormat("VARCHAR({0})", len);
					break;
				case SqlDataType.VarWChar:
					sb.AppendFormat("NVARCHAR({0})", len);
					break;
				case SqlDataType.WChar:
					sb.AppendFormat("NCHAR({0})", len);
					break;
				case SqlDataType.Uuid:
					sb.AppendFormat("VARCHAR({0})", 32);
					break;
				case SqlDataType.Identity:
					sb.Append("BIGINT");
					break;
				default:
					throw new NotImplementedException();
			}

			// null
			sb.Append(column.Spec.NotAllowNull ? " NOT NULL" : " NULL");

			// identity
			if(column.Spec.Identity) { sb.Append(" IDENTITY(1, 1)"); }

			return sb.ToString();
		}

		private SqlDbType ToDbType(SqlDataType dbType)
		{
			switch(dbType) {
				case SqlDataType.Unknown:
					return SqlDbType.VarChar;
				case SqlDataType.Boolean:
					return SqlDbType.Bit;
				case SqlDataType.Char:
					return SqlDbType.Char;
				case SqlDataType.Bit:
					return SqlDbType.Bit;
				case SqlDataType.Long:
					return SqlDbType.BigInt;
				case SqlDataType.Decimal:
					return SqlDbType.Decimal;
				case SqlDataType.Double:
					return SqlDbType.Float;
				case SqlDataType.Integer:
					return SqlDbType.Int;
				case SqlDataType.Enum:
					return SqlDbType.VarChar;
				case SqlDataType.MaxVarBinary:
					return SqlDbType.Image;
				case SqlDataType.MaxVarChar:
					return SqlDbType.Text;
				case SqlDataType.MaxVarWChar:
					return SqlDbType.NText;
				case SqlDataType.TimeStamp:
					return SqlDbType.DateTime;
				case SqlDataType.VarChar:
					return SqlDbType.VarChar;
				case SqlDataType.VarWChar:
					return SqlDbType.NVarChar;
				case SqlDataType.WChar:
					return SqlDbType.NChar;
				case SqlDataType.Uuid:
					return SqlDbType.VarChar;
				case SqlDataType.Identity:
					return SqlDbType.Int;
				default:
					throw new NotImplementedException();
			}
		}

		private MSSqlVersion ToVersion(DbSetting setting)
		{
			try {
				MSSqlVersion sqlVersion = MSSqlVersion.Sql2000;
				string msg = string.Empty;
				Regex regex = new Regex(@"\s+", RegexOptions.Compiled);
				string version = regex.Replace(setting.Version, string.Empty);

				if(version.StartsWith(
					"MicrosoftSQLServer2000",
					StringComparison.OrdinalIgnoreCase)) {
					sqlVersion = MSSqlVersion.Sql2000;
					msg = "MSSQL 2000.";
				} else if(version.StartsWith(
					"MicrosoftSQLServer2005",
					StringComparison.OrdinalIgnoreCase)) {
					sqlVersion = MSSqlVersion.Sql2005;
					msg = "MSSQL 2005.";
				} else if(version.StartsWith(
					"MicrosoftSQLServer2005Compact",
					StringComparison.OrdinalIgnoreCase)) {
					sqlVersion = MSSqlVersion.Sql2005CE;
					msg = "MSSQL 2005 Compact.";
				} else if(version.StartsWith(
					"MicrosoftSQLServer2008",
					StringComparison.OrdinalIgnoreCase)) {
					sqlVersion = MSSqlVersion.Sql2008;
					msg = "MSSQL 2008.";
				} else if(version.StartsWith(
					"MicrosoftSQLServerCompact4",
					StringComparison.OrdinalIgnoreCase)) {
					sqlVersion = MSSqlVersion.SqlCompact4;
					msg = "MSSQL Compact 4.0.";
				} else if(version.StartsWith(
					"MicrosoftSQLAzure",
					StringComparison.OrdinalIgnoreCase)) {
					sqlVersion = MSSqlVersion.SqlAzure;
					msg = "MSSQL Azure.";
				} else {
					sqlVersion = MSSqlVersion.Sql2008;
					msg = "MSSQL 2005 or above.";
				}

				return sqlVersion;
			} catch {
				return MSSqlVersion.Sql2000;
			}
		}

		private bool IsSQL2000(DbSetting setting)
		{
			return ToVersion(setting) == MSSqlVersion.Sql2000;
		}
		#endregion
	}
}
