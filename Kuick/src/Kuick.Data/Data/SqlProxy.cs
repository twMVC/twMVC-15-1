// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlProxy.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Text;
using System.Data.Common;
using System.Drawing;
using Kuick;
using System.Data;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class SqlProxy : ISqlDatabase
	{
		#region ISqlDatabase
		public string SessionID { get; private set; }
		public ConfigDatabase Config { get; private set; }
		public SqlBuilder Builder { get; internal set; }
		public Api Api { get; set; }

		public bool Transactional { get; protected set; }

		public void Begin(IsolationLevel isolation)
		{
		}
		public void Rollback()
		{
		}
		public void Commit()
		{
		}
		public void End()
		{
		}


		public string GetDatabaseVersion(IntervalLogger il)
		{
			return string.Empty;
		}
		public List<string> GetTableNames(IntervalLogger il)
		{
			return new List<string>();
		}
		public List<string> GetViewNames(IntervalLogger il)
		{
			return new List<string>();
		}
		public List<string> GetIndexNames(IntervalLogger il, IEntity schema)
		{
			return new List<string>();
		}


		public bool CheckTableExists(IntervalLogger il, IEntity schema)
		{
			return true;
		}
		public bool HasDBColumn(Column column)
		{
			return true;
		}

		public bool AlterableColumn(Column column)
		{
			return true;
		}

		public bool ColumnSpecChanged(Column column)
		{
			return true;
		}
		public void CreateTable(IntervalLogger il, IEntity schema)
		{
		}
		public void CreateIndexes(IntervalLogger il, IEntity schema)
		{
		}
		public void DropIndexes(IntervalLogger il, IEntity schema)
		{
		}
		public void SyncColumns(IntervalLogger il, IEntity schema)
		{
		}


		public List<TableSchema> GetTables(IntervalLogger il)
		{
			return new List<TableSchema>();
		}
		public List<ViewSchema> GetViews(IntervalLogger il)
		{
			return new List<ViewSchema>();
		}
		public List<IndexSchema> GetIndexes(IntervalLogger il, IEntity schema)
		{
			return new List<IndexSchema>();
		}


		public List<IEntity> Select(IntervalLogger il, Sql sql)
		{
			throw new NotImplementedException();
		}

		public List<Anys> AggregateSelect(IntervalLogger il, Sql sql)
		{
			throw new NotImplementedException();
		}

		public List<V> DistinctSelect<V>(IntervalLogger il, Sql sql)
		{
			throw new NotImplementedException();
		}

		public List<DateTime> DistinctDateSelect(IntervalLogger il, Sql sql)
		{
			throw new NotImplementedException();
		}

		public int CountSelect(IntervalLogger il, Sql sql)
		{
			throw new NotImplementedException();
		}

		public DataResult Insert(IntervalLogger il, params IEntity[] instance)
		{
			throw new NotImplementedException();
		}

		public DataResult Update(IntervalLogger il, IEntity instance)
		{
			throw new NotImplementedException();
		}

		public DataResult Update(IntervalLogger il, Sql sql)
		{
			throw new NotImplementedException();
		}

		public DataResult Delete(IntervalLogger il, params IEntity[] instance)
		{
			throw new NotImplementedException();
		}

		public DataResult Delete(IntervalLogger il, Sql sql)
		{
			throw new NotImplementedException();
		}

		public DataResult ExecuteNonQuery(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters)
		{
			throw new NotImplementedException();
		}

		public DataSet ExecuteQuery(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters)
		{
			throw new NotImplementedException();
		}

		public DataSet ExecuteStoredProcedure(
			IntervalLogger il, string sp, params IDbDataParameter[] parameters)
		{
			throw new NotImplementedException();
		}

		public Any ExecuteScalar(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters)
		{
			throw new NotImplementedException();
		}
		#endregion
	}
}
