// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// AppEntity.cs
//
// Modified By      YYYY-MM-DD
// kevinjong        2013-06-10 - Creation


using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Kuick.Data;
using Kuick.Web;
using System.Collections.Generic;

namespace Kuick.Builtin.Security
{
	[DataContract]
	[EntitySpec]
	public class AppEntity
		: ObjectEntity<AppEntity>, IApp
	{
		#region Constants
		public class Schema
		{
			public const string TableName = "KUICK_APP";

			public const string AppID = "APP_ID";
			public const string Title = "TITLE";
			public const string Platform = "PLATFORM";
			public const string SiteUrlOrExeName = "SITE_URL_OR_EXE_NAME";
		}
		#endregion

		#region Constructors
		public AppEntity()
			: base()
		{
		}
		#endregion

		#region Properties
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		[ColumnInitiate(InitiateValue.Uuid)]
		public string AppID { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 50)]
		public string Title { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 8)]
		public AppPlatform Platform { get; set; }

		[DataMember]
		[ColumnSpec(SpecFlag.NotAllowNull, 400)]
		[ColumnVisual(VisualFlag.HideInList, VisualSize.XXLarge)]
		public string SiteUrlOrExeName { get; set; }
		#endregion

		#region IEntity
		#endregion

		#region Class Level
		public static AppEntity GetCurrentApp()
		{
			AppEntity app = AppEntity.Get(Current.AppID);
			if(null == app) {
				app = new AppEntity() {
					AppID = Current.AppID,
					Title = Current.AppID,
					Platform = Current.IsWebApplication 
						? AppPlatform.Web : AppPlatform.Windows,
					SiteUrlOrExeName = WebTools.GetWebRoot()
				};
				app.Add();
			}
			return app;
		}
		#endregion

		#region Instance Level
		private List<IFeature> _Features;
		public List<IFeature> Features 
		{
			get {
				if(null == _Features) {
					_Features = FeatureEntity
						.Query(x => x.AppID == AppID)
						.ConvertAll<IFeature>(x => x as IFeature);
				}
				return _Features;
			}
		}

		public IFeature GetCurrentFeature(object o)
		{
			PageBase page = o as PageBase;
			if(null == page) { return null; }

			FeatureEntity feature = FeatureEntity.QueryFirst(x =>
				x.AppID == Current.AppID
				&
				x.RelativeUrl == WebTools.GetPathWithoutApplication()
			);
			if(null == feature) {
				feature = new FeatureEntity() {
					AppID = AppID,
					Title = Formator.AirBagToString(
						page.PageAuthorize.Title,
						WebTools.GetCurrentAspx()
					),
					RelativeUrl = WebTools.GetPathWithoutApplication()
				};
				feature.Add();
			}
			return feature;
		}
		#endregion

		#region Event Handler
		#endregion
	}
}
