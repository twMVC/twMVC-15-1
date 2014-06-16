// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ConfigEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-09-23 - Creation


using System;
using System.Runtime.Serialization;
using System.ComponentModel;
using Kuick.Data;
using System.Collections.Generic;

namespace Kuick.Builtin.Config
{
	[DataContract]
	[EntitySpec]
	[EntityIndex(true, Schema.AppId, Schema.Category, Schema.Path, Schema.Name)]
	public class ConfigEntity
		: ObjectEntity<ConfigEntity>
	{
		#region Schema
		public class Schema
		{
			// table name
			public const string TableName = "T_CONFIG";

			// fields
			public const string ConfigID = "CONFIG_ID";
			public const string AppId = "APP_ID";
			public const string Category = "CATEGORY";
			public const string Path = "Path";
			public const string Name = "NAME";
			public const string Value = "VALUE";
		}
		#endregion

		#region Constructor
		public ConfigEntity()
		{
			base.BeforeAdd += new EntityEventHandler(ConfigEntity_BeforeAdd);
		}
		#endregion

		#region Property
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		[ColumnInitiate(InitiateValue.Uuid)]
		public string ConfigID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.ReadOnly)]
		public string AppID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.ReadOnly)]
		[ColumnInitiate(Constants.Default.Category)]
		public string Category { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.ReadOnly)]
		[ColumnInitiate(Constants.Default.Path)]
		public string Path { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.ReadOnly)]
		public string Name { get; set; }

		[DataMember]
		[ColumnSpec]
		public string Value { get; set; }
		#endregion

		#region IEntity
		public override string TitleValue
		{
			get
			{
				return Name;
			}
		}

		public override List<EntityIndex> Indexes
		{
			get
			{
				return base.Indexes;
			}
		}
		#endregion

		#region class level
		#endregion

		#region instance level
		#endregion

		#region Event Handler
		private void ConfigEntity_BeforeAdd(IEntity sender, EntityEventArgs e)
		{
			if(!e.Result.Success) { return; }
			AppID = Current.AppID;
		}
		#endregion
	}
}
