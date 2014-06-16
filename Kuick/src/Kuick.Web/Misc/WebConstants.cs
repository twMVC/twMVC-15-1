// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WebConstants.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-07-20 - Creation


using System;

namespace Kuick.Web
{
	public class WebConstants : Kuick.Constants
	{
		public class UserAgent
		{
			public static readonly string[] Mobiles = new[] {
				"midp", "j2me", "avant", "docomo", "novarra", "palmos", "palmsource", 
				"240x320", "opwv", "chtml", "pda", "windows ce", "mmp/", 
				"blackberry", "mib/", "symbian", "wireless", "nokia", "hand", "mobi",
				"phone", "cdm", "up.b", "audio", "SIE-", "SEC-", "samsung", "HTC", 
				"mot-", "mitsu", "sagem", "sony", "alcatel", "lg", "eric", "vx", 
				"NEC", "philips", "mmm", "xx", "panasonic", "sharp", "wap", "sch",
				"rover", "pocket", "benq", "java", "pt", "pg", "vox", "amoi", 
				"bird", "compal", "kg", "voda", "sany", "kdd", "dbt", "sendo", 
				"sgh", "gradi", "jb", "dddi", "moto", "iphone"
			};

			public static readonly string[] iOSs = new[] { "ipod", "iphone", "ipad" };
			public static string iPod = "ipod";
			public static string iPhone = "iphone";
			public static string iPad = "ipad";
			public static string WindowsPhone = "windows phone";
		}

		public class PageName
		{
			public const string Signin = "signin.aspx";
			public const string Signout = "signout.aspx";
		}

		public class WebServerName
		{
			public const string VisualStudioDefaultWebServer = "Request";
			public const string VisualStudio2012 = "IIS7WorkerRequest";
			public const string IIS6 = "ISAPIWorkerRequestInProcForIIS6";
			public const string IIS7 = "ISAPIWorkerRequestInProcForIIS7";
		}

		public class Js
		{
			public const string Void = "javascript:void(0);";

			public class Event
			{
				public const string Click = "click";
				public const string Focus = "focus";
				public const string touchstart = "touchstart";
			}
		}

		public class Target
		{
			public const string Blank = "_blank";
			public const string Parent = "_parent";
			public const string Self = "_self";
			public const string Top = "_top";
		}
	}
}
