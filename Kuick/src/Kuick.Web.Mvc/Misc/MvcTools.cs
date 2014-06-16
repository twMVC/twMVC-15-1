// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MvcTools.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-02-20 - Creation


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Kuick.Web.Mvc
{
	public class MvcTools
	{
		public static RequestContext RequestContext
		{
			get
			{
				if(
					null == HttpContext.Current 
					|| 
					null == HttpContext.Current.Request) {
					throw new NullReferenceException(
						"System.Web.Routing.RequestContext is null reference."
					);
				}
				return HttpContext.Current.Request.RequestContext;
			}
		}

		public static UrlHelper UrlHelper
		{
			get{
				return new UrlHelper(RequestContext);
			}
		}

		#region RouteValue
		public static RouteData RouteData
		{
			get
			{
				return RequestContext.RouteData;
			}
		}

		public static bool HasRouteValue(string key)
		{
			return RouteData.Values[key] != null;
		}

		public static string GetRouteValue(string key)
		{
			return HasRouteValue(key)
				? RouteData.Values[key].ToString()
				: string.Empty;
		}

		public static string Controller
		{
			get
			{
				return GetRouteValue("controller");
			}
		}

		public static string Action
		{
			get
			{
				return GetRouteValue("action");
			}
		}
		#endregion

		#region QueryString
		public static WebParameter QueryString
		{
			get
			{
				return new WebParameter(HttpContext.Current.Request.QueryString);
			}
		}

		public static bool HasQueryString(string key)
		{
			return QueryString.Exists(key);
		}

		public static string GetQueryString(string key)
		{
			return HasQueryString(key)
				? QueryString.RequestValue(key)
				: string.Empty;
		}
		#endregion
	}
}
