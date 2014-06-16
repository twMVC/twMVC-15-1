// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MvcExtender.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-02-20 - Creation


using System;
using System.Web.Mvc;
using System.Web;

namespace Kuick.Web.Mvc
{
	public static class MvcExtender
	{
		public static HtmlString ToHtml(this string value)
		{
			return new HtmlString(value?? string.Empty);
		}
	}
}
