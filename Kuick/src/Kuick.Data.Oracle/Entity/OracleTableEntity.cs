// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleTableEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Runtime.Serialization;
using System.Data;
using System.Collections.Generic;

namespace Kuick.Data.Oracle
{
	[Serializable]
	[DataContract]
	[EntitySpec]
	public class OracleTableEntity : Entity<OracleTableEntity>
	{
		// table name
		public const string TABLE_NAME = "USER_TABLES";
		// fields
		public const string TABLE = "TABLE_NAME";
		//public const string OWNER = "OWNER";

		public OracleTableEntity()
		{
		}

		#region property
		[DataMember]
		[ColumnSpec(TABLE, SpecFlag.PrimaryKey, 32)]
		public string Name { get; set; }
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

		public override void Interceptor(Sql sql)
		{
			sql.Ascending(TABLE);
			base.Interceptor(sql);
		}
		#endregion

		#region class level
		internal static string[] GetPrimaryKeyColumnNames(
			SqlDatabase database, string tableName)
		{
			string sql = string.Format(
@"SELECT cols.COLUMN_NAME
FROM all_constraints cons, all_cons_columns cols
WHERE 
cols.table_name = '{0}'
AND cons.constraint_type = 'P'
AND cons.constraint_name = cols.constraint_name
AND cons.owner = cols.owner
ORDER BY cols.table_name, cols.position",
				tableName
			);

			try {
				DataSet ds = database.ExecuteQuery(null , sql);
				DataTable dt = ds.Tables[0];
				List<string> list = new List<string>();
				for(int i = 0; i < dt.Rows.Count; i++) {
					list.Add(dt.Rows[i]["COLUMN_NAME"].ToString());
				}
				return list.ToArray();
			} catch(Exception ex) {
				Logger.Error(
					"OracleTableEntity.GetPrimaryKeyColumnNames",
					ex.ToAny(new Any("SQL Command String", sql))
				);
				return new string[0];
			}
		}
		#endregion

		#region instance level
		private List<OracleIndexEntity> _DBIndexes;
		public List<OracleIndexEntity> DBIndexes
		{
			get
			{
				if(Checker.IsNullReference(_DBIndexes)) {
					_DBIndexes = OracleIndexEntity.GetByTableName(Name);
				}
				return _DBIndexes;
			}
		}

		public bool HasDBIndex(string dbIndexName)
		{
			List<OracleIndexEntity> dbIndexes = this.DBIndexes;
			if(Checker.IsNullReference(dbIndexes)) {
				return false;
			}

			foreach(OracleIndexEntity dbIndex in dbIndexes) {
				if(dbIndex.Name.Equals(dbIndexName, StringComparison.OrdinalIgnoreCase)) {
					return true;
				}
			}
			return false;
		}

		private List<OracleColumnEntity> _DBColumns;
		public List<OracleColumnEntity> DBColumns
		{
			get
			{
				if(Checker.IsNullReference(_DBColumns)) {
					_DBColumns = OracleColumnEntity.GetByTableName(Name);
				}
				return _DBColumns;
			}
		}

		public OracleColumnEntity GetDBColumn(string dbColumnName)
		{
			List<OracleColumnEntity> dbColumns = this.DBColumns;
			if(Checker.IsNullReference(dbColumns)) {
				return null;
			}

			foreach(OracleColumnEntity dbColumn in dbColumns) {
				if(dbColumn.Name.Equals(dbColumnName, StringComparison.OrdinalIgnoreCase)) {
					return dbColumn;
				}
			}
			return null;
		}

		public bool HasDBColumn(string dbColumnName)
		{
			List<OracleColumnEntity> dbColumns = this.DBColumns;
			if(Checker.IsNullReference(dbColumns)) {
				return false;
			}

			foreach(OracleColumnEntity dbColumn in dbColumns) {
				if(dbColumn.Name.Equals(dbColumnName, StringComparison.OrdinalIgnoreCase)) {
					return true;
				}
			}
			return false;
		}
		#endregion
	}
}
