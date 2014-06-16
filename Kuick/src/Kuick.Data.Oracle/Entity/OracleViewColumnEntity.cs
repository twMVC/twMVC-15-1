// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OracleViewColumnEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Kuick.Data.Oracle
{
	[Serializable]
	[DataContract]
	[EntitySpec]
	public class OracleViewColumnEntity : Entity<OracleViewColumnEntity>
	{
		// table name
		public const string TABLE_NAME  = "COL";
		// fields
		public const string NAME        = "CNAME";
		public const string VIEW        = "TNAME";
		public const string COLNO       = "COLNO";
		public const string COLTYPE     = "COLTYPE";
		public const string WIDTH       = "WIDTH";
		public const string NULLS       = "NULLS";

		public OracleViewColumnEntity()
		{
		}

		#region property
		[DataMember]
		[ColumnSpec(NAME, SpecFlag.PrimaryKey, 32)]
		public string Name { get; set; }
		[DataMember]
		[ColumnSpec(VIEW, 32)]
		public string ViewName { get; set; }
		[DataMember]
		[ColumnSpec(COLNO)]
		public int Colno { get; set; }
		[DataMember]
		[ColumnSpec(COLTYPE, 64)]
		public string DataType { get; set; }
		[DataMember]
		[ColumnSpec(WIDTH)]
		public int DataLength { get; set; }
		[DataMember]
		[ColumnSpec(NULLS)]
		public string Nulls { get; set; }
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
		public static List<OracleViewColumnEntity> GetByViewName(string viewName)
		{
			return OracleViewColumnEntity
				.Sql()
				.Where(x => x.ViewName == viewName)
				.Query();
		}

		internal static List<OracleViewColumnEntity> GetAllInOneView(Api api, string viewName)
		{
			return OracleViewColumnEntity
				.Sql()
				.Where(x => x.ViewName == viewName)
				.Query();
		}
		#endregion

		#region instance level
		#endregion
	}
}
