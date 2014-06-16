// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MSSqlSysObjectsEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Data;

namespace Kuick.Data.MSSql
{
	[DataContract]
	[EntitySpec]
	public class MSSqlSysObjectsEntity : Entity<MSSqlSysObjectsEntity>
	{
		#region const
		//public const string TABLE_NAME = "sysobjects";

		public const string NAME = "name";
		public const string ID = "id";
		public const string XTYPE = "xtype";

		//public const string TABLE = "U";
		public const string VIEW = "V";
		#endregion

		#region constructor
		public MSSqlSysObjectsEntity()
			: base()
		{
		}
		#endregion

		#region property
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey, 256)]
		public string Name { get; set; }
		[DataMember]
		[ColumnSpec]
		public int Id { get; set; }
		[DataMember]
		[ColumnSpec(64)]
		public string XType { get; set; }
		#endregion

		#region IEntity
		public override string TableName
		{
			get
			{
				return "sysobjects";
			}
		}

		public override string TitleValue
		{
			get
			{
				return Name;
			}
		}

		public override bool Alterable
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region class level
		public static MSSqlSysObjectsEntity GetByTableName(string tableName)
		{
			return MSSqlSysObjectsEntity
				.Sql()
				.Where(x => x.Name == tableName)
				.QueryFirst();
		}

		public static List<MSSqlSysObjectsEntity> GetAllTables()
		{
			return MSSqlSysObjectsEntity
				.Sql()
				.Where(x => x.XType == "U")
				.Query();
		}

		public static List<MSSqlSysObjectsEntity> GetAllViews()
		{
			return MSSqlSysObjectsEntity
				.Sql()
				.Where(x => x.XType == "V")
				.Query();
		}

		public static List<string> GetPrimaryKeyColumnNames(string tableName)
		{
			DataSet ds = Api.GetNew<MSSqlSysObjectsEntity>().ExecuteQuery(
				string.Format("Exec sp_pkeys '{0}'", tableName)
			);
			DataTable dt = ds.Tables[0];
			List<string> list = new List<string>();
			for(int i = 0; i < dt.Rows.Count; i++) {
				list.Add(dt.Rows[i]["COLUMN_NAME"].ToString());
			}
			return list;
		}
		#endregion

		#region instance level
		public string SchemaEntityName { get; set; }

		private List<MSSqlSysColumnsEntity> _SysColumns;
		public List<MSSqlSysColumnsEntity> SysColumns
		{
			get
			{
				if(Checker.IsNull(_SysColumns)) {
					_SysColumns = MSSqlSysColumnsEntity.GetColumns(Id);
				}
				return _SysColumns;
			}
		}

		public MSSqlSysColumnsEntity GetSysColumn(string columnName)
		{
			return SysColumns.Find(x => x.Name == columnName);
		}

		public bool HasDBColumn(string columnName)
		{
			return !Checker.IsNull(GetSysColumn(columnName));
		}

		private List<MSSqlSysIndexesEntity> _SysIndexes;
		public List<MSSqlSysIndexesEntity> SysIndexes
		{
			get
			{
				if(Checker.IsNull(_SysIndexes)) {
					_SysIndexes = MSSqlSysIndexesEntity.GetIndexes(Id);
				}
				return _SysIndexes;
			}
		}

		public bool HasSysIndex(string name)
		{
			return SysIndexes.Exists(x => x.Name == name);
		}
		#endregion
	}
}
