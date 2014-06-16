// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// FragmentEntity.cs
//
// Modified By      YYYY-MM-DD
// kevinjong        2013-06-10 - Creation


using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Kuick.Data;

namespace Kuick.Builtin.Security
{
	[DataContract]
	[EntitySpec]
	[EntityIndex(false, Schema.AppID, Schema.FeatureID)]
	public class FragmentEntity
		: ObjectEntity<FragmentEntity>, IFragment
	{
		#region Constants
		public class Schema
		{
			public const string TableName = "KUICK_FRAGMENT";

			public const string AppID = "App_ID";
			public const string FeatureID = "FEATURE_ID";
			public const string FragmentID = "FRAGMENT_ID";
			public const string Title = "TITLE";
			public const string Description = "DESCRIPTION";
		}
		#endregion

		#region Constructors
		public FragmentEntity()
			: base()
		{
		}
		#endregion

		#region Properties
		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string AppID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string FeatureID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey, 50)]
		[ColumnInitiate(InitiateValue.Uuid)]
		public string FragmentID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string Title { get; set; }

		[DataMember]
		[ColumnSpec]
		public string Description { get; set; }
		#endregion

		#region IEntity
		#endregion

		#region Class Level
		#endregion

		#region Instance Level
		private IApp _App;
		public IApp App
		{
			get
			{
				if(null == _App) {
					_App = AppEntity.Get(AppID);
				}
				return _App;
			}
		}

		private IFeature _Feature;
		public IFeature Feature
		{
			get
			{
				if(null == _Feature) {
					_Feature = FeatureEntity.Get(FeatureID);
				}
				return _Feature;
			}
		}
		#endregion

		#region Event Handler
		#endregion
	}
}
