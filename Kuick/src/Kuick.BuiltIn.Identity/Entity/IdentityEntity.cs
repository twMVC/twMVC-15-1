// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IdentityEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-12-08 - Creation


using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Kuick.Data;

namespace Kuick.Builtin.Identity
{
	[DataContract]
	[EntitySpec]
	public class IdentityEntity
		: ObjectEntity<IdentityEntity>
	{
		#region Schema
		public class Schema
		{
			// table name
			public const string TableName = "T_IDENTITY";

			// fields
			public const string IdentityID = "IDENTITY_ID";
			public const string Frequency = "FREQUENCY";
			public const string Start = "START";
			public const string Incremental = "INCREMENTAL";
			public const string CurrentValue = "CURRENT_VALUE";
		}
		#endregion

		#region Constructor
		public IdentityEntity()
		{
		}
		#endregion

		#region Property
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		public string IdentityID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.ReadOnly)]
		public Frequency Frequency { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.ReadOnly)]
		[ColumnInitiate(1)]
		public int Start { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.ReadOnly)]
		[ColumnInitiate(1)]
		public int Incremental { get; set; }

		[DataMember]
		[ColumnSpec]
		[ColumnInitiate(1)]
		public int CurrentValue { get; set; }
		#endregion

		#region IEntity
		#endregion

		#region class level
		#endregion

		#region instance level
		#endregion

		#region Event Handler
		#endregion
	}
}
