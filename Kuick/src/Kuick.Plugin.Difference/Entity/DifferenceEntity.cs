// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DifferenceEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-09-15 - Creation


using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Kuick.Data;

namespace Kuick.Plugin.Difference
{
	[DataContract]
	[EntitySpec]
	[EntityIndex(Schema.Who, Schema.What, Schema.How)]
	public class DifferenceEntity
		: ObjectEntity<DifferenceEntity>
	{
		#region Schema
		public class Schema
		{
			// table name
			public const string TableName = "T_PLUGIN_DIFFERENCE";

			// fields
			public const string ErrorID = "Difference_ID";
			public const string Title = "Title";
			public const string Who = "WHO";      // UserName
			public const string What = "WHAT";    // EntityName
			public const string What2 = "WHAT_2"; // KeyValue
			public const string How = "HOW";      // Method
			public const string Which = "WHICH";  // Json
		}
		#endregion

		#region Constructor
		public DifferenceEntity()
		{
		}
		#endregion

		#region Property
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		[ColumnInitiate(InitiateValue.Uuid)]
		public string ErrorID { get; set; }

		[DataMember]
		[Description("Title")]
		[ColumnSpec(SpecFlag.NotAllowNull, 100)]
		public string Title { get; set; }

		[DataMember]
		[Description("User Name")]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string Who { get; set; }

		[DataMember]
		[Description("Entity Name")]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string What { get; set; }

		[DataMember]
		[Description("Key Value")]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string What2 { get; set; }

		[DataMember]
		[Description("Method")]
		[ColumnSpec(SpecFlag.NotAllowNull, 6)]
		public DiffMethod How { get; set; }

		[DataMember]
		[Description("Difference Json")]
		[ColumnSpec(SqlDataType.MaxVarWChar, SpecFlag.NotAllowNull)]
		public string Which { get; set; }
		#endregion

		#region IEntity
		public override string TitleValue
		{
			get
			{
				return Title;
			}
		}

		public override void Interceptor(Sql<DifferenceEntity> sql)
		{
			sql.Descending(x => x.CreateDate);
			base.Interceptor(sql);
		}
		#endregion

		#region class level
		#endregion

		#region instance level
		#endregion

		#region Event Handler
		#endregion
	}
}
