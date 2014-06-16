// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// DeviceEntity.cs
//
// Modified By      YYYY-MM-DD
// kevinjong        2013-06-10 - Creation


using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web;
using Kuick.Data;
using Kuick.Web;

namespace Kuick.Builtin.Security
{
	[DataContract]
	[EntitySpec]
	[EntityIndex(
		true, 
		Schema.IP, 
		Schema.IsMobile, 
		Schema.Type, 
		Schema.BrowserName, 
		Schema.BrowserType
	)]
	public class DeviceEntity
		: ObjectEntity<DeviceEntity>
	{
		#region Constants
		public class Schema
		{
			public const string TableName = "KUICK_DEVICE";

			public const string DeviceID = "DEVICE_ID";
			public const string IP = "IP";
			public const string IsMobile = "IS_MOBILE";
			public const string Type = "TYPE";
			public const string BrowserName = "BROWSER_NAME";
			public const string BrowserType = "BROWSER_TYPE";
			public const string Verified = "VERIFIED";
		}
		#endregion

		#region Constructors
		public DeviceEntity()
			: base()
		{
		}
		#endregion

		#region Properties
		[DataMember]
		[ColumnSpec(SpecFlag.PrimaryKey)]
		[ColumnInitiate(InitiateValue.Uuid)]
		[ColumnVisual(VisualFlag.SystemColumn)]
		public string DeviceID { get; set; }

		[DataMember]
		[ColumnSpec(50)]
		public string IP { get; set; }

		[DataMember]
		[ColumnSpec]
		public bool IsMobile { get; set; }

		[DataMember]
		[ColumnSpec(10)]
		public DeviceType Type { get; set; }

		[DataMember]
		[ColumnSpec(20)]
		public string BrowserName { get; set; }

		[DataMember]
		[ColumnSpec(20)]
		public string BrowserType { get; set; }

		[DataMember]
		[ColumnSpec]
		[ColumnInitiate(false)]
		public bool Verified { get; set; }
		#endregion

		#region IEntity
		#endregion

		#region Class Level
		public static DeviceEntity GetCurrentDevice()
		{
			if(!Current.IsWebApplication){ return null; }

			string deviceID = CookieManager.Get(Constants.Client.DeviceID);
			DeviceEntity device = DeviceEntity.Get(deviceID);
			if(null != device) { return device; }

			string ip = WebTools.GetClientIP();
			DeviceType type = DeviceType.Others;
			bool isMobile = false;

			WebBase web = new WebBase();
			HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;

			if(web.IsMobileClient) {
				isMobile = true;
				if(web.IsiPodClient) {
					type = DeviceType.iPod;
				} else if(web.IsiPhoneClient) {
					type = DeviceType.iPhone;
				} else if(web.IsiPadClient) {
					type = DeviceType.iPad;
				} else if(web.IsiOSClient) {
					type = DeviceType.iOS;
				} else {
					type = DeviceType.Mobile;
				}
			} else {
				isMobile = false;
				if(!browser.Win16 && !browser.Win32) {
					type = DeviceType.NonWindows;
				} else {
					type = DeviceType.Windows;
				}
			}

			device = DeviceEntity.QueryFirst(x =>
				x.IP == WebTools.GetClientIP()
				&
				x.IsMobile == isMobile
				&
				x.Type == type
				&
				x.BrowserName == browser.Browser 
				&
				x.BrowserType == browser.Type
			);
			if(null == device) {
				device = new DeviceEntity() {
					IP = WebTools.GetClientIP(),
					IsMobile = isMobile,
					Type = type,
					BrowserName = browser.Browser,
					BrowserType = browser.Type
				};
				device.Add();
			}

			CookieManager.Set(
				Constants.Client.DeviceID, 
				device.DeviceID, 
				DateTime.Now.AddYears(10)
			);

			return device;
		}
		#endregion

		#region Instance Level
		#endregion

		#region Event Handler
		#endregion
	}
}
