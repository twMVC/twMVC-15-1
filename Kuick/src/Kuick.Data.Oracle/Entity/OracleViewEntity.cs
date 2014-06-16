// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleViewEntity.cs
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
	public class OracleViewEntity : Entity<OracleViewEntity>
	{
		// table name
		public const string TABLE_NAME = "USER_VIEWS";
		// fields
		public const string VIEW      = "VIEW_NAME";

		public OracleViewEntity()
		{
		}

		#region property
		[DataMember]
		[ColumnSpec(VIEW, SpecFlag.PrimaryKey, 32)]
		public string Name { get; set; }
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
			sql.Ascending(VIEW);
			base.Interceptor(sql);
		}
		#endregion

		#region class level
		#endregion

		#region instance level
		private List<OracleViewColumnEntity> _DBColumns;
		public List<OracleViewColumnEntity> DBColumns
		{
			get
			{
				if(Checker.IsNullReference(_DBColumns)) {
					_DBColumns = OracleViewColumnEntity.GetByViewName(Name);
				}
				return _DBColumns;
			}
		}

		public OracleViewColumnEntity GetDBColumn(string dbColumnName)
		{
			List<OracleViewColumnEntity> dbColumns = this.DBColumns;
			if(Checker.IsNullReference(dbColumns)) {
				return null;
			}

			foreach(OracleViewColumnEntity dbColumn in dbColumns) {
				if(dbColumn.Name.Equals(dbColumnName, StringComparison.OrdinalIgnoreCase)) {
					return dbColumn;
				}
			}
			return null;
		}

		public bool HasDBColumn(string dbColumnName)
		{
			List<OracleViewColumnEntity> dbColumns = this.DBColumns;
			if(Checker.IsNullReference(dbColumns)) {
				return false;
			}

			foreach(OracleViewColumnEntity dbColumn in dbColumns) {
				if(dbColumn.Name.Equals(dbColumnName, StringComparison.OrdinalIgnoreCase)) {
					return true;
				}
			}
			return false;
		}
		#endregion
	}
}
