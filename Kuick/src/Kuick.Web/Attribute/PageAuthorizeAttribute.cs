// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// PageAuthorizeAttribute.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-02-07 - Creation


using System;

namespace Kuick.Web
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class PageAuthorizeAttribute : Attribute
	{
		public PageAuthorizeAttribute(string title)
		{
			this.Title = title;
		}

		public string Title { get; set; }
	}
}
