// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MSSqlSysIndexesEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Kuick.Data.MSSql
{
	[DataContract]
	[EntitySpec]
	public class MSSqlSysIndexesEntity : Entity<MSSqlSysIndexesEntity>
	{
		#region const
		//public const string TABLE_NAME = "sysindexes";

		public const string NAME = "name";
		public const string ID = "id";
		public const string CLUSTERED = "indid";
		#endregion

		#region constructor
		public MSSqlSysIndexesEntity()
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
		[ColumnSpec]
		public int Clustered { get; set; }
		#endregion

		#region IEntity
		public override string TableName
		{
			get
			{
				return "sysindexes";
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
		public static List<MSSqlSysIndexesEntity> GetIndexes(int tableID)
		{
			return MSSqlSysIndexesEntity
				.Sql()
				.Where(x => x.Id == tableID & x.Clustered != 1)
				.Query();
		}
		#endregion

		#region instance level
		#endregion
	}
}
