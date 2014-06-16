// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MySQLDatabase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-09-22 - Creation


using System;
using System.Linq;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Data;

namespace Kuick.Data.MySQL
{
	[Database("MySQL")]
	public class MySQLDatabase : SqlDatabase<
		MySqlConnection,
		MySqlTransaction,
		MySqlCommand,
		MySqlDataAdapter,
		MySqlDataReader,
		MySQLReader>
	{
		#region constructor
		public MySQLDatabase(ConfigDatabase config)
			: base(config, new MySQLBuilder())
		{
			ParseConnectionString();
		}
		#endregion

		public override MySqlCommand BuildCommand()
		{
			return new MySqlCommand();
		}

		public override MySqlDataAdapter BuildAdapter()
		{
			return new MySqlDataAdapter();
		}

		public override MySQLReader BuildReader(MySqlDataReader reader)
		{
			return new MySQLReader(reader);
		}

		public override string GetDatabaseVersion(IntervalLogger il)
		{
			using(MySqlCommand cmd = new MySqlCommand("select @@version", Connection)) {
				cmd.CommandTimeout = 10;
				cmd.Transaction = Transaction;
				object version = cmd.ExecuteScalar();
				return version.ToString();
			}
		}

		public override List<string> GetTableNames(IntervalLogger il)
		{
			return (from x in GetTables(il) select x.TableName).ToList();
		}

		public override List<string> GetViewNames(IntervalLogger il)
		{
			return (from x in GetViews(il) select x.ViewName).ToList();
		}

		public override List<string> GetIndexNames(IntervalLogger il, IEntity schema)
		{
			return (from x in GetIndexes(il, schema) select x.IndexName).ToList();
		}

		public override bool CheckTableExists(IntervalLogger il, IEntity schema)
		{
			return GetTables(il).Exists(x => x.TableName == schema.TableName);
		}

		public override bool HasDBColumn(Column column)
		{
			IEntity entity = EntityCache.Get(column.EntityName);
			MySqlDataReader reader = null;
			bool exist = false;
			string sql = String.Format(
@"SHOW columns from {0} where Field = `{1}`", 
				entity.TableName, 
				column.Spec.ColumnName
			);
			MySqlCommand cmd = new MySqlCommand(sql, Connection);
			try {
				reader = cmd.ExecuteReader();
				if(reader.Read()) {
					exist = true;
				}
			} catch(MySqlException ex) {
				Logger.Error(
					"MySQLDatabase.HasDBColumn",
					"Failed to populate index",
					ex,
					new Any("TableName", column.TableName),
					new Any("ColumnName", column.Spec.ColumnName)
				);
			} finally {
				if(null != reader) {
					reader.Close();
				}
			}
			return exist;
		}

		public override bool AlterableColumn(Column column)
		{
			// TODO: AlterableColumn
			return true;
		}

		public override bool ColumnSpecChanged(Column column)
		{
			// TODO: ColumnSpecChanged
			return true;
		}

		private List<TableSchema> _Tables = new List<TableSchema>();
		public override List<TableSchema> GetTables(IntervalLogger il)
		{
			if(null != _Tables && _Tables.Count > 0) { return _Tables; }

			MySqlDataReader reader = null;
			MySqlDataReader readerInner = null;
			List<string> tableNames = new List<string>();
			List<TableSchema> tables = new List<TableSchema>();

			string sql = String.Format(
@"select TABLE_NAME from Information_Schema.Tables where TABLE_SCHEMA='{0}' and TABLE_TYPE='BASE TABLE';", 
				ConnectionStringDatabase
			);
			MySqlCommand cmd = new MySqlCommand(sql, Connection);
			try {
				reader = cmd.ExecuteReader();
				while(reader.Read()) {
					tableNames.Add(reader.GetString("TABLE_NAME"));
				}
			} catch(MySqlException ex) {
				Logger.Error(
					"MySQLDatabase Information_Schema.Tables has something wrong !",
					ex,
					new Any("columnName", sql)
				);
			} finally {
				if(null != reader) {
					reader.Close();
				}
			}

			foreach(string tableName in tableNames) {
				TableSchema table = new TableSchema();
				table.TableName = tableName;
				List<string> primaryKeys = GetPrimaryKeyColumnNames(il, table.TableName);
				// Columns
				sql = String.Format(
@"Select COLUMN_NAME,DATA_TYPE,CASE (IS_NULLABLE) WHEN 'YES' then 'true' else 'false' end as IN_NULLABLE,CASE (COLUMN_KEY) WHEN '' then 'false' else 'true' end as COLUMN_KEY,CASE  ISNULL (CHARACTER_OCTET_LENGTH) When true then (CASE ISNULL(NUMERIC_PRECISION) When true then '0' else NUMERIC_PRECISION end ) else CHARACTER_OCTET_LENGTH end as Length from Information_Schema.Columns where TABLE_SCHEMA='{0}' and TABLE_NAME='{1}' ;",
					ConnectionStringDatabase,
					table.TableName
				);

				MySqlCommand cmdInner = new MySqlCommand(sql, Connection);
				try {
					readerInner = cmdInner.ExecuteReader();
					if(readerInner.Read()) {
						ColumnSchema column = new ColumnSchema();
						column.FieldName = readerInner.GetString("COLUMN_NAME");
						column.DBType = FromXType(readerInner.GetString("DATA_TYPE"));
						column.AllowNull = readerInner.GetBoolean("IN_NULLABLE");
						column.PrimaryKey = readerInner.GetBoolean("COLUMN_KEY");
						column.Length = readerInner.GetInt32("LENGTH");
						table.Columns.Add(column);
					}
				} catch(MySqlException ex) {
					Logger.Error(
					"MySQLDatabase Information_Schema.Tables is something wrong !",
					ex,
					new Any("columnName", sql)
					);
				} finally {
					if(null != readerInner) {
						readerInner.Close();
					}
				}
				// Indexes
				IEntity schema = EntityCache.GetByTableName(table.TableName);
				if(null != schema) {
					table.Indexes.AddRange(GetIndexes(il, schema));
				}

				tables.Add(table);
			}

			return tables;
		}

		private List<ViewSchema> _Views = new List<ViewSchema>();
		public override List<ViewSchema> GetViews(IntervalLogger il)
		{
			if(null != _Views && _Views.Count > 0) { return _Views; }

			MySqlDataReader reader = null;
			MySqlDataReader readerInner = null;
			List<ViewSchema> views = new List<ViewSchema>();

			string sql = String.Format(
@"select TABLE_NAME from Information_Schema.Tables where TABLE_SCHEMA='{0}' and TABLE_TYPE='VIEW';", 
				ConnectionStringDatabase
			);
			MySqlCommand cmd = new MySqlCommand(sql, Connection);
			try {
				reader = cmd.ExecuteReader();
				while(reader.Read()) {
					ViewSchema view = new ViewSchema();
					view.ViewName = reader.GetString("TABLE_NAME");
					List<string> primaryKeys = GetPrimaryKeyColumnNames(il, view.ViewName);

					// Columns
					sql = String.Format(
@"Select COLUMN_NAME,DATA_TYPE,CASE (IS_NULLABLE) WHEN 'YES' then 'true' else 'false' end as IN_NULLABLE,CASE (COLUMN_KEY) WHEN '' then 'false' else 'true' end as COLUMN_KEY,CASE  ISNULL (CHARACTER_OCTET_LENGTH) When true then (CASE ISNULL(NUMERIC_PRECISION) When true then '0' else NUMERIC_PRECISION end ) else CHARACTER_OCTET_LENGTH end as Length from Information_Schema.Columns where TABLE_SCHEMA='{0}' and TABLE_NAME='{1}' ;", 
						ConnectionStringDatabase,
						view.ViewName
					);

					MySqlCommand cmdInner = new MySqlCommand(sql, Connection);
					try {
						readerInner = cmdInner.ExecuteReader();
						if(readerInner.Read()) {
							ColumnSchema column = new ColumnSchema();
							column.FieldName = readerInner.GetString("COLUMN_NAME");
							column.DBType = FromXType(readerInner.GetString("DATA_TYPE"));
							column.AllowNull = readerInner.GetBoolean("IS_NULLABLE");
							column.PrimaryKey = readerInner.GetBoolean("COLUMN_KEY");
							column.Length = readerInner.GetInt32("LENGTH");
							view.Columns.Add(column);
						}
					} catch(MySqlException ex) {
						Logger.Error(
						"MySQLDatabase Information_Schema.Views is something wrong !",
						ex,
						new Any("columnName", sql)
						);
					} finally {
						if(null != readerInner) {
							readerInner.Close();
						}
					}
					views.Add(view);
				}
			} catch(MySqlException ex) {
				Logger.Error(
					"MySQLDatabase Information_Schema.Views has something wrong !",
					ex,
					new Any("columnName", sql)
				);
			} finally {
				if(null != reader) {
					reader.Close();
				}
			}
			return views;
		}

		private List<IndexSchema> _Indexes = new List<IndexSchema>();
		public override List<IndexSchema> GetIndexes(IntervalLogger il, IEntity schema)
		{
			if(null != _Indexes && _Indexes.Count > 0) { return _Indexes; }

			string sql = string.Format(@"show indexes from {0}", schema.TableName);

			DataSet ds = null;
			try {
				ds = ExecuteQuery(il, sql);
			} catch(Exception ex) {
				if(!ex.Message.StartsWith("Can't find file")) { throw; }
			}
			if(null == ds) { return _Indexes; }

			DataTable dt = ds.Tables[0];
			string indexName = string.Empty;
			bool unigue = false;
			List<string> columns = new List<string>();
			for(int i = 0; i < dt.Rows.Count; i++) {
				if(
					indexName != string.Empty
					&&
					indexName != dt.Rows[i]["Key_name"].ToString()) {
						_Indexes.Add(
							new IndexSchema() {
								TableName = schema.TableName, 
								Unique = unigue, 
								IndexName = indexName,
								ColumnNames = columns 
							}
						);

					// reset
					indexName = string.Empty;
					unigue = false;
					columns = new List<string>();
				}
				indexName = dt.Rows[i]["Key_name"].ToString();
				unigue = dt.Rows[i]["Non_unique"].ToString() == "1";
				columns.Add(dt.Rows[i]["Column_name"].ToString());
			}
			if(dt.Rows.Count > 0) {
				if(!_Indexes.Exists(x => x.IndexName == indexName)) {
					_Indexes.Add(
						new IndexSchema() {
							TableName = schema.TableName,
							Unique = unigue,
							IndexName = indexName,
							ColumnNames = columns
						}
					);
				}
			}

			return _Indexes;
		}

		#region private
		private string ConnectionStringServer = string.Empty;
		private string ConnectionStringDatabase = string.Empty;
		private string ConnectionStringUid = string.Empty;
		private string ConnectionStringPwd = string.Empty;

		private void ParseConnectionString()
		{
			Anys anys = Config.ConnectionString.ToAnys();
			foreach(Any any in anys) {
				switch(any.Name.ToLower()) {
					case "server":
						ConnectionStringServer = any.ToString();
						break;
					case "database":
						ConnectionStringDatabase = any.ToString();
						break;
					case "uid":
						ConnectionStringUid = any.ToString();
						break;
					case "pwd":
						ConnectionStringPwd = any.ToString();
						break;
					default:
						Logger.Track(
							"MySQLDatabase.ParseConnectionString",
							"This block is not yet parse in MySQL Database connection string.",
							any
						);
						break;
				}
			}
		}

		private List<string> GetPrimaryKeyColumnNames(IntervalLogger il, string tableName)
		{
			DataSet ds = base.ExecuteQuery(
				il,
				string.Format(
@"Select COLUMN_NAME from Information_Schema.Columns where TABLE_SCHEMA='{0}' and TABLE_NAME='{1}' and COLUMN_KEY<>'';",
					ConnectionStringDatabase, 
					tableName
				)
			);
			DataTable dt = ds.Tables[0];
			List<string> list = new List<string>();
			for(int i = 0; i < dt.Rows.Count; i++) {
				list.Add(dt.Rows[i]["COLUMN_NAME"].ToString());
			}
			return list;
		}

		private SqlDataType FromXType(string xType)
		{
			switch(xType.ToLower()) {
				case "integer":
					return SqlDataType.Integer;
				case "tinyint":
					return SqlDataType.Integer;
				case "smallint":
					return SqlDataType.Integer;
				case "timestamp":
					return SqlDataType.TimeStamp;
				case "mediumint":
					return SqlDataType.Integer;
				case "int":
					return SqlDataType.Integer;
				case "bigint":
					return SqlDataType.Long;
				case "decimal":
					return SqlDataType.Decimal;
				case "numeric":
					return SqlDataType.Decimal;
				case "float":
					return SqlDataType.Decimal;
				case "real":
					return SqlDataType.Decimal;
				case "double":
					return SqlDataType.Double;
				case "datetime":
					return SqlDataType.TimeStamp;
				case "date":
					return SqlDataType.TimeStamp;
				case "time":
					return SqlDataType.TimeStamp;
				case "year":
					return SqlDataType.TimeStamp;
				case "char":
					return SqlDataType.Char;
				case "varchar":
					return SqlDataType.VarChar;
				case "binary":
					return SqlDataType.MaxVarBinary;
				case "varbinary":
					return SqlDataType.MaxVarBinary;
				case "blob":
					return SqlDataType.MaxVarWChar;
				case "text":
					return SqlDataType.MaxVarWChar;
				case "tinyblob":
					return SqlDataType.MaxVarWChar;
				case "tinytext":
					return SqlDataType.MaxVarWChar;
				case "mediumblob":
					return SqlDataType.MaxVarWChar;
				case "mediumtext":
					return SqlDataType.MaxVarWChar;
				case "longblob":
					return SqlDataType.MaxVarWChar;
				case "longtext":
					return SqlDataType.MaxVarWChar;
				case "enum":
					return SqlDataType.Enum;
				case "set":
					return SqlDataType.Enum;
				default:
					return SqlDataType.Unknown;
			}
		}
		#endregion
	}
}
