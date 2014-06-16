// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MvcStart.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-02-20 - Creation


using System;
using System.Web.Mvc;
using System.Web;

namespace Kuick.Web.Mvc
{
	public class MvcStart : StartBase
	{
		public static Action<Controller> Interceptor { get; set; }
	}
}
