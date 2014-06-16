// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MSSqlDatabase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Kuick;
using System.Data;

namespace Kuick.Data.MSSql
{
	[Database("MSSql")]
	public class MSSqlDatabase : SqlDatabase<
		SqlConnection,
		SqlTransaction,
		SqlCommand,
		SqlDataAdapter,
		SqlDataReader,
		MSSqlReader>
	{
		#region constructor
		public MSSqlDatabase(ConfigDatabase config)
			: base(config, new MSSqlBuilder())
		{
			ParseConnectionString();
		}
		#endregion

		public override SqlCommand BuildCommand()
		{
			return new SqlCommand();
		}

		public override SqlDataAdapter BuildAdapter()
		{
			return new SqlDataAdapter();
		}

		public override MSSqlReader BuildReader(SqlDataReader reader)
		{
			return new MSSqlReader(reader);
		}

		public override string GetDatabaseVersion(IntervalLogger il)
		{
			using(SqlCommand cmd = new SqlCommand("select @@version", Connection)) {
				cmd.CommandTimeout = 10;
				cmd.Transaction = Transaction;
				object version = cmd.ExecuteScalar();
				return version.ToString();
			}
		}

		public override List<string> GetTableNames(IntervalLogger il)
		{
			List<string> list = new List<string>();
			MSSqlSysObjectsEntity.GetAllTables().ForEach(x => list.Add(x.Name));
			return list;
		}

		public override List<string> GetViewNames(IntervalLogger il)
		{
			List<string> list = new List<string>();
			MSSqlSysObjectsEntity.GetAllViews().ForEach(x => list.Add(x.Name));
			return list;
		}

		public override List<string> GetIndexNames(IntervalLogger il, IEntity schema)
		{
			MSSqlSysObjectsEntity instance = MSSqlSysObjectsEntity.GetByTableName(
				schema.TableName
			);

			List<string> list = new List<string>();
			MSSqlSysIndexesEntity.GetIndexes(instance.Id).ForEach(x => list.Add(x.Name));
			return list;
		}

		public override bool CheckTableExists(IntervalLogger il, IEntity schema)
		{
			MSSqlSysObjectsEntity instance = MSSqlSysObjectsEntity.GetByTableName(
				schema.TableName
			);
			if(null == instance) { return false; }
			instance.SchemaEntityName = schema.EntityName;
			return !Checker.IsNull(instance);
		}

		public override bool HasDBColumn(Column column)
		{
			MSSqlSysObjectsEntity sysTable = GetSysObjectsEntity(column.TableName);
			if(null == sysTable) { return false; }

			return sysTable.HasDBColumn(column.Spec.ColumnName);
		}

		public override bool AlterableColumn(Column column)
		{
			MSSqlSysObjectsEntity sysTable = GetSysObjectsEntity(column.TableName);
			if(null == sysTable) { return false; }

			MSSqlSysColumnsEntity sysColumn = sysTable.GetSysColumn(column.Spec.ColumnName);
			if(null == sysColumn) { return false; }

			return sysColumn.AlterableField();
		}

		public override bool ColumnSpecChanged(Column column)
		{
			MSSqlSysObjectsEntity sysTable = GetSysObjectsEntity(column.TableName);
			if(null == sysTable) { return false; }

			MSSqlSysColumnsEntity sysColumn = sysTable.GetSysColumn(column.Spec.ColumnName);
			if(null == sysColumn) { return false; }

			return sysColumn.SpecChanged(column);
		}

		public override List<TableSchema> GetTables(IntervalLogger il)
		{
			throw new NotImplementedException();
		}

		public override List<ViewSchema> GetViews(IntervalLogger il)
		{
			throw new NotImplementedException();
		}

		public override List<IndexSchema> GetIndexes(IntervalLogger il, IEntity schema)
		{
			throw new NotImplementedException();
		}

		#region private
		private static object _SysObjectsLocking = new object();
		private static List<MSSqlSysObjectsEntity> _SysObjects = new List<MSSqlSysObjectsEntity>(); 
		private MSSqlSysObjectsEntity GetSysObjectsEntity(string tableName)
		{
			lock(_SysObjectsLocking) {
				MSSqlSysObjectsEntity one = _SysObjects.Find(x => x.Name == tableName);
				if(null != one) { return one; }
				one = MSSqlSysObjectsEntity.Get(tableName);
				if(null != one) { _SysObjects.Add(one); }
				return one;
			}
		}

		private string ConnectionStringDataSource = string.Empty;
		private string ConnectionStringInitialCatalog = string.Empty;
		private string ConnectionStringUser = string.Empty;
		private string ConnectionStringPassword = string.Empty;
		private int ConnectionStringMaxPoolSize = 0;

		private void ParseConnectionString()
		{
			Anys anys = Config.ConnectionString.ToAnys();
			foreach(Any any in anys) {
				switch(any.Name.ToLower()) {
					case "data source":
						ConnectionStringDataSource = any.ToString();
						break;
					case "initial catalog":
						ConnectionStringInitialCatalog = any.ToString();
						break;
					case "user id":
						ConnectionStringUser = any.ToString();
						break;
					case "password":
						ConnectionStringPassword = any.ToString();
						break;
					case "max pool size":
						ConnectionStringMaxPoolSize = any.ToInteger();
						break;
					default:
						break;
				}
			}
		}
		#endregion
	}
}
