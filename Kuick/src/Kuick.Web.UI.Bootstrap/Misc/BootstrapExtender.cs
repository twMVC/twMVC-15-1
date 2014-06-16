// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BootstrapExtender.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-19 - Creation


using System;
using Kuick.Data;
using System.Web.UI;

namespace Kuick.Web.UI.Bootstrap
{
	public static class BootstrapExtender
	{
		public static Editor<T> Editor<T>(this T instance, Page page)
			where T : class, IEntity, new()
		{
			return new Editor<T>(page, instance);
		}
	}
}
