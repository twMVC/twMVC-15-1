// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleIndexEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Data;

namespace Kuick.Data.Oracle
{
	[Serializable]
	[DataContract]
	[EntitySpec]
	public class OracleIndexEntity : Entity<OracleIndexEntity>
	{
		// table name
		public const string TABLE_NAME = "USER_INDEXES";
		// fields
		public const string NAME       = "INDEX_NAME";
		public const string TABLE      = "TABLE_NAME";
		//public const string OWNER      = "OWNER";

		public OracleIndexEntity()
		{
		}

		#region property
		[DataMember]
		[ColumnSpec(NAME, SpecFlag.PrimaryKey, 32)]
		public string Name { get; set; }
		[DataMember]
		[ColumnSpec(TABLE, 32)]
		public new string Table { get; set; }
		//[DataMember]
		//[ColumnSpec(OWNER, 64)]
		//public string Owner { get; set; }
		#endregion

		#region IEntity
		public override bool Alterable
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region class level
		public static List<OracleIndexEntity> GetByTableName(string tableName)
		{
			return OracleIndexEntity
				.Sql()
				.Where(x => x.Table == tableName)
				.Query();
		}

		internal static List<IndexSchema> GetIndexes(
			IntervalLogger il, SqlDatabase database, string tableName)
		{
			string sql = string.Format(
@"select 
  a.index_name as INDEX_NAME
, a.uniqueness as IS_UNIQUE -- UNIQUE, NONUNIQUE
, b.column_name as COLUMN_NAME
from ALL_INDEXES a
left outer join user_ind_columns b
on a.index_name = b.index_name
where a.table_name = '{0}' and a.generated = 'N'
order by a.index_name, b.column_position",
				tableName
			);

			DataSet ds = database.ExecuteQuery(il, sql);
			DataTable dt = ds.Tables[0];
			List<IndexSchema> list  =new List<IndexSchema>();
			string indexName = string.Empty;
			bool unigue = false;
			List<string> columns = new List<string>();
			for(int i = 0; i < dt.Rows.Count; i++) {
				if(
					indexName != string.Empty
					&&
					indexName != dt.Rows[i]["INDEX_NAME"].ToString()) {
					list.Add(GetIndex(tableName, indexName, unigue, columns));

					// reset
					indexName = string.Empty;
					unigue = false;
					columns = new List<string>();
				}
				indexName = dt.Rows[i]["INDEX_NAME"].ToString();
				unigue = dt.Rows[i]["IS_UNIQUE"].ToString() == "UNIQUE";
				columns.Add(dt.Rows[i]["COLUMN_NAME"].ToString());
			}
			if(dt.Rows.Count > 0) {
				if(!list.Exists(x => x.IndexName == indexName)) {
					list.Add(GetIndex(tableName, indexName, unigue, columns));
				}
			}

			return list;
		}

		private static IndexSchema GetIndex(
			string tableName, string indexName, bool unigue, List<string> columns)
		{
			IndexSchema index = new IndexSchema();
			index.TableName = tableName;
			index.Unique = unigue;
			index.IndexName = indexName;
			index.ColumnNames = columns;
			return index;
		}
		#endregion

		#region instance level
		#endregion
	}
}
