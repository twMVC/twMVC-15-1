// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WebCurrent.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-07-20 - Creation


using System;
using System.Web;

namespace Kuick.Web
{
	public class WebCurrent : Kuick.Current
	{
		public static bool IsMobileClient
		{
			get
			{
				if(!Current.IsWebApplication) { return false; }
				string userAgent = HttpContext
					.Current
					.Request
					.ServerVariables["HTTP_USER_AGENT"]
					.ToLower();
				foreach(string agent in WebConstants.UserAgent.Mobiles) {
					if(userAgent.Contains(agent.ToLower())) { return true; }
				}
				return false;
			}
		}

		public static bool IsWindowsPhone
		{
			get
			{
				if(!Current.IsWebApplication) { return false; }
				string userAgent = HttpContext
					.Current
					.Request
					.ServerVariables["HTTP_USER_AGENT"]
					.ToLower();
				return userAgent.Contains(WebConstants.UserAgent.WindowsPhone);
			}
		}

		public static string JsClickEventName
		{
			get
			{
				return IsMobileClient
					? IsWindowsPhone
						? WebConstants.Js.Event.Click
						: WebConstants.Js.Event.touchstart
					: WebConstants.Js.Event.Click;
			}
		}

		public static string JsFocusEventName
		{
			get
			{
				return IsMobileClient
					? IsWindowsPhone
						? WebConstants.Js.Event.Focus
						: WebConstants.Js.Event.touchstart
					: WebConstants.Js.Event.Focus;
			}
		}

		public class Web
		{
			private const string Group = "Web";

			public static string FileRoot
			{
				get
				{
					return WebCurrent.Application.GetString(Group, "FileRoot", "/upload");
				}
			}

			public static bool EnableAuthorize
			{
				get
				{
					return WebCurrent.Application.GetBoolean(Group, "EnableAuthorize", true);
				}
			}

			public static bool CheckSsl
			{
				get
				{
					return WebCurrent.Application.GetBoolean(Group, "CheckSsl", false);
				}
			}

			public static string jQueryFile
			{
				get
				{
					return WebTools.RegulateUriProtocol(
							WebCurrent.Application.GetString(
							Group,
							"jQueryFile",
							WebChecker.IsNetworkAvailable
								? "//ajax.googleapis.com/ajax/libs/jquery/1/jquery.min.js"
								: WebTools.ToApplicationFilePath("/js/jquery.min.js")
						)
					);
				}
			}
		}
	}
}
