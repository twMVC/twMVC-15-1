// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// CookieManager.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web;

namespace Kuick.Web
{
	public class CookieManager
	{
		public static bool Exists(string cookieName)
		{
			if(!WebCurrent.IsWebApplication) { return false; }

			if(null == HttpContext.Current && null == HttpContext.Current.Request) {
				return false;
			}
			HttpCookie c = HttpContext.Current.Request.Cookies[cookieName];
			return null != c;
		}

		public static string Get(string cookieName)
		{
			if(!WebCurrent.IsWebApplication) { return string.Empty; }

			if(null == HttpContext.Current && null == HttpContext.Current.Request) {
				return string.Empty;
			}
			HttpCookie c = HttpContext.Current.Request.Cookies[cookieName];
			return null == c
				? null
				: WebChecker.IsNull(c.Value)
					? null
					: c.Value;
		}

		public static void Set(string cookieName, string value)
		{
			Set(cookieName, value, DateTime.MinValue);
		}

		public static void Set(string cookieName, string value, DateTime dateTime)
		{
			bool httpOnly = false;
			Set(cookieName, value, dateTime, httpOnly);
		}

		public static void Set(
			string cookieName,
			string value,
			DateTime dateTime,
			bool httpOnly)
		{
			if(!WebCurrent.IsWebApplication) { return; }

			if(WebChecker.IsNull(HttpContext.Current.Request)) { return; }
			HttpCookie c = HttpContext.Current.Request.Cookies[cookieName];
			if(null == c) { c = new HttpCookie(cookieName); }
			if(DateTime.MinValue != dateTime) { c.Expires = dateTime; }
			c.HttpOnly = httpOnly;
			c.Value = value;
			HttpContext.Current.Response.Cookies.Add(c);
		}

		public static void Clear(string cookieName)
		{
			if(!WebCurrent.IsWebApplication) { return; }

			if(WebChecker.IsNull(HttpContext.Current.Request)) { return; }
			HttpCookie cookie = new HttpCookie(cookieName, null);
			cookie.Expires = DateTime.Now.AddYears(-1);
			HttpContext.Current.Response.Cookies.Add(cookie);
		}
	}
}
