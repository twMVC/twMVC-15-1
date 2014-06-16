// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MySQLBuilder.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-09-22 - Creation


using System;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data;
using MySql.Data.Common;
using MySql.Data.Types;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Properties;
using System.Data;

namespace Kuick.Data.MySQL
{
	public class MySQLBuilder : SqlBuilder
	{
		public override string Vender { get { return MySQLConstants.Vender; } }
		public override string OpenTag { get { return MySQLConstants.Symbol.BackQuoter; } }
		public override string CloseTag { get { return MySQLConstants.Symbol.BackQuoter; } }
		public override string Wildcard { get { return MySQLConstants.Symbol.Asterisk; } }
		public override string ParameterPrefix { get { return MySQLConstants.Symbol.At; } }
		public override string ParameterSuffix { get { return string.Empty; } }

		public override string BuildLiteral(SqlLiteral literal)
		{
			//?
			const string PATTERN = "DISTINCT CONVERT(VARCHAR({0}), {1}, 21) AS {2}";
			switch(literal.Format) {
				case SqlLiteralFormat.Command:
					return literal.CommandText;
				case SqlLiteralFormat.DistinctDate:
					string taggedColumnName = Tag(literal.ColumnName);
					string taggedAsName = Tag(literal.AsName);
					switch(literal.Range) {
						case SqlDistinctDate.Year:
							return string.Format(PATTERN, 4, taggedColumnName, taggedAsName);
						case SqlDistinctDate.YearMonth:
							return string.Format(PATTERN, 7, taggedColumnName, taggedAsName);
						case SqlDistinctDate.YearMonthDay:
							return string.Format(PATTERN, 10, taggedColumnName, taggedAsName);
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
				"DATE_FORMAT('{0}', '%Y-%m-%d %T.%f')",
				value
			);
		}

		public override string BuildTopCommand(string command, int top)
		{
			string sql = command.ToUpper().Trim();

			int i = sql.IndexOf(MySQLConstants.Sql.Select);
			if(i < 0) {
				throw new Exception(string.Format(
					"Keyword '{0}' was not found.", MySQLConstants.Sql.Select
				));
			}
			return string.Concat(command, " LIMIT  ", top);
		}

		public override void AddDbParameter(IDbCommand cmd, Column column, object value)
		{
			MySqlCommand sqlCommand = cmd as MySqlCommand;
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
					"	{0}{1}",
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
@"ALTER TABLE {0} MODIFY COLUMN {1}",
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
			int len = column.Spec.Length;
			StringBuilder sb = new StringBuilder();
			sb.Append("`");
			sb.Append(column.Spec.ColumnName);
			if(column.Spec.PrimaryKey && len > 255) {len = 255; }
			sb.Append("`");
			sb.Append(" ");

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
					sb.Append(" LONGBLOB");
					break;
				case SqlDataType.MaxVarChar:
					sb.Append(" LONGTEXT");
					break;
				case SqlDataType.MaxVarWChar:
					sb.Append(" LONGTEXT");
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
			sb.Append(column.Spec.NotAllowNull ? " NOT NULL" : " NULL");
			sb.Append(column.Spec.PrimaryKey ? " PRIMARY KEY" : string.Empty);
			sb.Append(column.Spec.Identity ? " IDENTITY(1, 1)" : string.Empty);

			return sb.ToString();
		}

		private MySqlDbType ToDbType(SqlDataType dbType)
		{
			switch(dbType) {
				case SqlDataType.Unknown:
					return MySqlDbType.VarChar;
				case SqlDataType.Boolean:
					return MySqlDbType.Bit;
				case SqlDataType.Char:
					return MySqlDbType.VarChar;
				case SqlDataType.Bit:
					return MySqlDbType.Bit;
				case SqlDataType.Long:
					return MySqlDbType.Int64;
				case SqlDataType.Decimal:
					return MySqlDbType.Decimal;
				case SqlDataType.Double:
					return MySqlDbType.Float;
				case SqlDataType.Integer:
					return MySqlDbType.Int32;
				case SqlDataType.Enum:
					return MySqlDbType.VarChar;
				case SqlDataType.MaxVarBinary:
					return MySqlDbType.VarBinary;
				case SqlDataType.MaxVarChar:
					return MySqlDbType.Text;
				case SqlDataType.MaxVarWChar:
					return MySqlDbType.VarString;
				case SqlDataType.TimeStamp:
					return MySqlDbType.DateTime;
				case SqlDataType.VarChar:
					return MySqlDbType.VarChar;
				case SqlDataType.VarWChar:
					return MySqlDbType.Text;
				case SqlDataType.WChar:
					return MySqlDbType.VarChar;
				case SqlDataType.Uuid:
					return MySqlDbType.VarChar;
				case SqlDataType.Identity:
					return MySqlDbType.Int32;
				default:
					throw new NotImplementedException();
			}
		}

		//private MSSqlVersion ToVersion(DbSetting setting)
		//{
		//    try {
		//        MSSqlVersion sqlVersion = MSSqlVersion.Sql2000;
		//        string msg = string.Empty;
		//        Regex regex = new Regex(@"\s+", RegexOptions.Compiled);
		//        string version = regex.Replace(setting.Version, string.Empty);

		//        if(version.StartsWith(
		//            "MicrosoftSQLServer2000",
		//            StringComparison.OrdinalIgnoreCase)) {
		//            sqlVersion = MSSqlVersion.Sql2000;
		//            msg = "MSSQL 2000.";
		//        } else if(version.StartsWith(
		//            "MicrosoftSQLServer2005",
		//            StringComparison.OrdinalIgnoreCase)) {
		//            sqlVersion = MSSqlVersion.Sql2005;
		//            msg = "MSSQL 2005.";
		//        } else if(version.StartsWith(
		//            "MicrosoftSQLServer2005Compact",
		//            StringComparison.OrdinalIgnoreCase)) {
		//            sqlVersion = MSSqlVersion.Sql2005CE;
		//            msg = "MSSQL 2005 Compact.";
		//        } else if(version.StartsWith(
		//            "MicrosoftSQLServer2008",
		//            StringComparison.OrdinalIgnoreCase)) {
		//            sqlVersion = MSSqlVersion.Sql2008;
		//            msg = "MSSQL 2008.";
		//        } else if(version.StartsWith(
		//            "MicrosoftSQLServerCompact4",
		//            StringComparison.OrdinalIgnoreCase)) {
		//            sqlVersion = MSSqlVersion.SqlCompact4;
		//            msg = "MSSQL Compact 4.0.";
		//        } else if(version.StartsWith(
		//            "MicrosoftSQLAzure",
		//            StringComparison.OrdinalIgnoreCase)) {
		//            sqlVersion = MSSqlVersion.SqlAzure;
		//            msg = "MSSQL Azure.";
		//        } else {
		//            sqlVersion = MSSqlVersion.Sql2008;
		//            msg = "MSSQL 2005 or above.";
		//        }

		//        return sqlVersion;
		//    } catch {
		//        return MSSqlVersion.Sql2000;
		//    }
		//}

		//private bool IsSQL2000(DbSetting setting)
		//{
		//    return ToVersion(setting) == MSSqlVersion.Sql2000;
		//}
		#endregion
	}
}
