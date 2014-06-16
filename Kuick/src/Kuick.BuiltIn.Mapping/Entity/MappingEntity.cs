// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MappingEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Kuick.Data;

namespace Kuick.Builtin.Mapping
{
	[DataContract]
	[EntitySpec]
	[EntityIndex(
		true,
		Schema.PreOrderEntityName,
		Schema.PostOrderEntityName,
		Schema.PreOrderID,
		Schema.PostOrderID
	)]
	public class MappingEntity
		: ObjectEntity<MappingEntity>
	{
		#region Schema
		public class Schema
		{
			// table name
			public const string TableName = "T_MAPPING";

			// fields
			public const string MappingID = "MAPPING_ID";
			public const string PreOrderEntityName = "PRE_ORDER_ENTITY_NAME";
			public const string PostOrderEntityName = "POST_ORDER_ENTITY_NAME";
			public const string PreOrderID = "PRE_ORDER_ID";
			public const string PostOrderID = "POST_ORDER_ID";
			public const string PreOrderNo = "PRE_ORDER_NO";
			public const string PostOrderNo = "POST_ORDER_NO";
		}
		#endregion

		#region Constructor
		public MappingEntity()
		{
		}
		#endregion

		#region Property
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		[ColumnInitiate(InitiateValue.Uuid)]
		public string MappingID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.ReadOnly)]
		public string PreOrderEntityName { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.ReadOnly)]
		public string PostOrderEntityName { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.ReadOnly)]
		public string PreOrderID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.ReadOnly)]
		public string PostOrderID { get; set; }

		[DataMember]
		[ColumnSpec]
		public int PreOrderNo { get; set; }

		[DataMember]
		[ColumnSpec]
		public int PostOrderNo { get; set; }
		#endregion

		#region IEntity
		public override string TableName
		{
			get
			{
				return Schema.TableName;
			}
		}

		public override string TitleValue
		{
			get
			{
				return string.Format("{0} <==> {1}", PreOrderID, PostOrderID);
			}
		}

		public override bool EnableConcurrency
		{
			get
			{
				return false;
			}
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
