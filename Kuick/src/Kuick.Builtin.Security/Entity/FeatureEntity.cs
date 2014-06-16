// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// FeatureEntity.cs
//
// Modified By      YYYY-MM-DD
// kevinjong        2013-06-10 - Creation


using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Kuick.Data;
using System.Collections.Generic;

namespace Kuick.Builtin.Security
{
	[DataContract]
	[EntitySpec]
	[EntityIndex(true, Schema.AppID, Schema.RelativeUrl)]
	public class FeatureEntity
		: ObjectEntity<FeatureEntity>, IFeature
	{
		#region Constants
		public class Schema
		{
			public const string TableName = "KUICK_FEATURE";

			public const string AppID = "App_ID";
			public const string FeatureID = "FEATURE_ID";
			public const string Title = "TITLE";
			public const string RelativeUrl = "RELATIVE_URL";
		}
		#endregion

		#region Constructors
		public FeatureEntity()
			: base()
		{
		}
		#endregion

		#region Properties
		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string AppID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey, 50)]
		[ColumnInitiate(InitiateValue.Uuid)]
		[ColumnVisual(VisualFlag.HideInList)]
		public string FeatureID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string Title { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 100)]
		public string RelativeUrl { get; set; }
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

		private List<IFragment> _Fragments;
		public List<IFragment> Fragments
		{
			get
			{
				if(null == _Fragments) {
					_Fragments = FragmentEntity
						.Query(x => x.AppID == AppID & x.FeatureID == FeatureID)
						.ConvertAll<IFragment>(x => x as IFragment);
				}
				return _Fragments;
			}
		}

		public string AbsolutelyUrl
		{
			get
			{
				if(null == App) {
					return RelativeUrl;
				} else {
					AppEntity app = App as AppEntity;
					return string.Concat(app.SiteUrlOrExeName, RelativeUrl);
				}
			}
		}

		public IFragment GetCurrentFragment(string title, string description)
		{
			if(string.IsNullOrEmpty(title)) { return null; }

			FragmentEntity fragment = FragmentEntity.QueryFirst(x =>
				x.AppID == AppID
				&
				x.FeatureID == FeatureID
				&
				x.Title == title
			);
			if(null == fragment) {
				fragment = new FragmentEntity() {
					AppID = AppID,
					FeatureID = FeatureID,
					Title = title,
					Description = description
				};
				fragment.Add();
			}
			return fragment;
		}
		#endregion

		#region Event Handler
		#endregion
	}
}
