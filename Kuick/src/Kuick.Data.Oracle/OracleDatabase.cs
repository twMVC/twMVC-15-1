// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleDatabaseBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;

namespace Kuick.Data.Oracle
{
	public abstract class OracleDatabase<C, T, M, A, R, RX> 
		: SqlDatabase<C, T, M, A, R, RX>
		where C : IDbConnection, new()
		where T : IDbTransaction
		where M : IDbCommand, new()
		where A : IDbDataAdapter, new()
		where R : IDataReader
		where RX : SqlReader<R>, IDisposable
	{
		#region constructor
		public OracleDatabase(ConfigDatabase config, SqlBuilder builder)
			: base(config, builder)
		{
			ParseConnectionString();
		}
		#endregion

		public override A BuildAdapter()
		{
			return new A();
		}

		public override RX BuildReader(R reader)
		{
			return Reflector.CreateInstance<RX>(
				new Type[] { typeof(R) }, 
				new object[] { reader }
			);
		}

		public override string GetDatabaseVersion(IntervalLogger il)
		{
			using(M cmd = BuildCommand()) {
				cmd.CommandText = "select * from v$version";
				cmd.CommandTimeout = 10;
				cmd.Transaction = Transaction;
				cmd.Connection = Connection;
				object version = cmd.ExecuteScalar();
				return version.ToString();
			}
		}

		public override List<string> GetTableNames(IntervalLogger il)
		{
			List<string> list = new List<string>();
			OracleTableEntity.GetAll().ForEach(x => list.Add(x.Name));
			return list;
		}

		public override List<string> GetViewNames(IntervalLogger il)
		{
			List<string> list = new List<string>();
			OracleViewEntity.GetAll().ForEach(x => list.Add(x.Name));
			return list;
		}

		public override List<string> GetIndexNames(IntervalLogger il, IEntity schema)
		{
			OracleTableEntity instance = OracleTableEntity.Get(schema.TableName);
			List<string> list = new List<string>();
			OracleIndexEntity.GetIndexes(il, this, instance.Name).ForEach(x => list.Add(x.IndexName));
			return list;
		}

		public override bool CheckTableExists(IntervalLogger il, IEntity schema)
		{
			return OracleTableEntity.Exists(schema.TableName);
		}

		public override bool HasDBColumn(Column column)
		{
			OracleTableEntity sysTable = GetOracleTable(column.TableName);
			if(null == sysTable) { return false; }

			return sysTable.HasDBColumn(column.Spec.ColumnName);
		}

		public override bool AlterableColumn(Column column)
		{
			OracleTableEntity sysTable = GetOracleTable(column.TableName);
			if(null == sysTable) { return false; }

			OracleColumnEntity sysColumn = sysTable.GetDBColumn(column.Spec.ColumnName);
			if(null == sysColumn) { return false; }

			return sysColumn.AlterableField(column);
		}

		public override bool ColumnSpecChanged(Column column)
		{
			OracleTableEntity sysTable = GetOracleTable(column.TableName);
			if(null == sysTable) { return false; }

			OracleColumnEntity sysColumn = sysTable.GetDBColumn(column.Spec.ColumnName);
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
		private static object _OracleTableLocking = new object();
		private static List<OracleTableEntity> _OracleTable = new List<OracleTableEntity>();
		private OracleTableEntity GetOracleTable(string tableName)
		{
			lock(_OracleTableLocking) {
				OracleTableEntity one = _OracleTable.Find(x => x.Name == tableName);
				if(null != one) { return one; }
				one = OracleTableEntity.Get(tableName);
				if(null != one) { _OracleTable.Add(one); }
				return one;
			}
		}

		private string ConnectionDataSource = string.Empty;
		private string ConnectionStringUserID = string.Empty;
		private string ConnectionStringPassword = string.Empty;

		private void ParseConnectionString()
		{
			Anys anys = Config.ConnectionString.ToAnys();
			foreach(Any any in anys) {
				switch(any.Name.ToLower()) {
					case "data source":
						ConnectionDataSource = any.ToString();
						break;
					case "user id":
						ConnectionStringUserID = any.ToString();
						break;
					case "password":
						ConnectionStringPassword = any.ToString();
						break;
					default:
						Logger.Track(
							"OracleDatabase.ParseConnectionString",
							"This block is not yet parse in Oracle Database connection string.",
							any
						);
						break;
				}
			}
		}
		#endregion
	}
}
