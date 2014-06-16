// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleBuilder.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using Kuick;
using System.Data;
using System.Text;

namespace Kuick.Data.Oracle
{
	public abstract class OracleBuilder : SqlBuilder
	{

		public readonly string[] _KEY_WORDS = new String[] { "rownum" };

		public override string Vender { get { return OracleConstants.Vender; } }
		public override string OpenTag { get { return OracleConstants.Symbol.Quotation; } }
		public override string CloseTag { get { return OracleConstants.Symbol.Quotation; } }
		public override string Wildcard { get { return OracleConstants.Symbol.Percent; } }
		public override string UnicodeTag { get { return string.Empty; } }
		public override string AssignNullMaxVarChar { get { return "empty_clob()"; } }
		public override string ParameterPrefix { get { return OracleConstants.Symbol.Colon; } }
		public override string ParameterSuffix { get { return string.Empty; } }
		public override string BeforeCommand { get { return "set def off;"; } }
		public override string AfterCommand { get { return string.Empty; } }


		public override string BuildLiteral(SqlLiteral literal)
		{
			switch(literal.Format) {
				case SqlLiteralFormat.Command:
				case SqlLiteralFormat.DistinctDate:
					return literal.CommandText;
				default:
					throw new NotImplementedException();
			}
		}

		public override string BuildValue(DateTime value)
		{
			string dt = String.Format(
				"{0}-{1}-{2} {3}:{4}:{5}.{6}",
				value.Year, value.Month, value.Day, 
				value.Hour, value.Minute, value.Second, value.Millisecond
			);
			return string.Format("to_timestamp('{0}', 'YYYY-MM-DD HH24:MI:SS.FF3')", dt);
		}

		public override string BuildTopCommand(string command, int top)
		{
			string sql = command.ToUpper().Trim();

			int i = sql.IndexOf(OracleConstants.Sql.Select);
			if(i < 0) {
				throw new Exception(string.Format(
					"Keyword '{0}' was not found.", OracleConstants.Sql.Select
				));
			}

			return string.Format(
@"SELECT * FROM (
{0}
) WHERE ROWNUM < {1}", 
				command, 
				top + 1
			);
		}

		public override string BuildCreateTableCommandText(IEntity schema)
		{
			DbSetting setting = DbSettingCache.Get(schema.EntityName);
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat(
@"CREATE TABLE {0}",
				Tag(schema.TableName)
			);

			bool firstTime = true;
			sb.Append("(" + Environment.NewLine);
			foreach(Column column in schema.Columns) {
				// Check Concurrency
				//if(schema.SkipInDataLayer(column.Spec.ColumnName)) { continue; }

				sb.AppendFormat(
@"	{0}{1}{2}",
					(firstTime ? " " : ","),
					ColumnSqlCommand(column.Spec),
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
@"CREATE {0}INDEX {1} on {2}",
				(index.Unique ? "UNIQUE " : string.Empty),
				Tag(index.IndexName),
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

		public override string BuildDropIndexCommandText(string tableName, string indexName)
		{
			string sql = String.Format(
				"DROP INDEX {0};",
				Tag(indexName)
			);
			return sql;
		}

		public override string BuildAlterColumnCommandText(Column column)
		{
			// skip pk
			if(column.Spec.PrimaryKey) { return string.Empty; }

			StringBuilder sb = new StringBuilder();
			if(column.Spec.DbType == SqlDataType.Bit) {
				// To ensure that the data format conversion is correct, 
				// processing data before correction.
				sb.AppendFormat(
@"UPDATE {0} SET {1} = '1' WHERE {1} = 'True';
UPDATE {0} SET {1} = '0';", // where {1} = 'False'
					Tag(column.TableName),
					Tag(column.Spec.ColumnName)
				);
				sb.AppendLine();
			}
			sb.AppendFormat(
@"ALTER TABLE {0} MODIFY {1};",
				Tag(column.TableName),
				ColumnSqlCommand(column.Spec)
			);

			string sql = sb.ToString();
			return sql;
		}

		public override string BuildAddColumnCommandText(Column column)
		{
			// skip pk
			if(column.Spec.PrimaryKey) { return string.Empty; }

			string sql = String.Format(
@"ALTER TABLE {0} ADD {1}",
				Tag(column.TableName),
				ColumnSqlCommand(column.Spec)
			);

			return sql;
		}


		#region private
		private string ColumnSqlCommand(ColumnSpec column)
		{
			string str = Tag(column.ColumnName);
			int len = column.Length;
			switch(column.DbType) {
				case SqlDataType.Bit:
					str += " NUMBER(1)";
					break;
				case SqlDataType.Long:
					str += " NUMBER(38)";
					break;
				case SqlDataType.Integer:
					str += " NUMBER(38)";
					break;
				case SqlDataType.Boolean:
					str += " VARCHAR2(" + 5 + ")";
					break;
				case SqlDataType.Enum:
					str += " VARCHAR2(" + 128 + ")";
					break;
				case SqlDataType.Char:
					str += " CHAR(" + len + ")";
					break;
				case SqlDataType.WChar:
					str += " NCHAR(" + len + ")";
					break;
				case SqlDataType.VarChar:
					str += " VARCHAR2(" + len + ")";
					break;
				case SqlDataType.VarWChar:
					str += " NVARCHAR2(" + len + ")";
					break;
				case SqlDataType.MaxVarChar:
				case SqlDataType.MaxVarWChar:
					str += " LONG VARCHAR";
					break;
				case SqlDataType.Decimal:
					str += " DECIMAL  (" + len + ", 3)";
					break;
				case SqlDataType.TimeStamp:
					str += " DATE";
					break;
				case SqlDataType.MaxVarBinary:
					str += " LONG RAW";
					break;
				default:
					throw new NotImplementedException(string.Format(
						"OracleBuilder.ColumnSqlCommand: {0}",
						column.DbType.ToString()
					));
			}
			str += !column.NotAllowNull ? " NULL " : " NOT NULL ";
			str += column.PrimaryKey ? " primary key " : string.Empty;
			return str;
		}
		#endregion
	}
}
