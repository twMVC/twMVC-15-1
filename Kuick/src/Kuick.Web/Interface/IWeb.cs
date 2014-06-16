// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IWeb.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Collections.Specialized;
using System.Web;

namespace Kuick.Web
{
	public interface IWeb
	{
		IParameter Parameter { get; }

		HttpContext GetCurrentContext();
		NameValueCollection GetRequestNVs();
		IParameter GetParameter();
		string RequestValue(string name);
		string RequestValue(string name, string airBag);

		string UserAgent { get; }
		bool IsMobileClient { get; }
		bool IsiOSClient { get; }
		bool IsiPodClient { get; }
		bool IsiPhoneClient { get; }
		bool IsiPadClient { get; }
		Anys Errors { get; set; }

		bool Authenticated { get; }
	}
}
