// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ISqlDatabase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using Kuick;
using System.Data;
using System.Collections.Generic;
using System.Data.Common;

namespace Kuick.Data
{
	public interface ISqlDatabase
	{
		string SessionID { get; }
		ConfigDatabase Config { get; }
		SqlBuilder Builder { get; }
		Api Api { get; set; }

		bool Transactional { get; }

		void Begin(IsolationLevel isolation);
		void Rollback();
		void Commit();
		void End();

		string GetDatabaseVersion(IntervalLogger il);
		List<string> GetTableNames(IntervalLogger il);
		List<string> GetViewNames(IntervalLogger il);
		List<string> GetIndexNames(IntervalLogger il, IEntity schema);
		bool CheckTableExists(IntervalLogger il, IEntity schema);
		bool HasDBColumn(Column column);
		bool AlterableColumn(Column column);
		bool ColumnSpecChanged(Column column);
		void CreateTable(IntervalLogger il, IEntity schema);
		void CreateIndexes(IntervalLogger il, IEntity schema);
		void DropIndexes(IntervalLogger il, IEntity schema);
		void SyncColumns(IntervalLogger il, IEntity schema);

		List<TableSchema> GetTables(IntervalLogger il);
		List<ViewSchema> GetViews(IntervalLogger il);
		List<IndexSchema> GetIndexes(IntervalLogger il, IEntity schema);

		List<IEntity> Select(IntervalLogger il, Sql sql);
		List<Anys> AggregateSelect(IntervalLogger il, Sql sql);
		List<V> DistinctSelect<V>(IntervalLogger il, Sql sql);
		List<DateTime> DistinctDateSelect(IntervalLogger il, Sql sql);
		int CountSelect(IntervalLogger il, Sql sql);
		DataResult Insert(IntervalLogger il, params IEntity[] instance);
		DataResult Update(IntervalLogger il, IEntity instance);
		DataResult Update(IntervalLogger il, Sql sql);
		DataResult Delete(IntervalLogger il, params IEntity[] instance);
		DataResult Delete(IntervalLogger il, Sql sql);

		DataResult ExecuteNonQuery(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters
		);
		DataSet ExecuteQuery(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters
		);
		DataSet ExecuteStoredProcedure(
			IntervalLogger il, string sp, params IDbDataParameter[] parameters
		);
		Any ExecuteScalar(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters
		);
	}
}
