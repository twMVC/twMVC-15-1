// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MvcGlobal.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-02-20 - Creation


using System;
using System.Web.Mvc;

namespace Kuick.Web.Mvc
{
	public class MvcGlobal : Global
	{
		public override void Application_Start(Object sender, EventArgs e)
		{
			base.Application_Start(sender, e);
			MvcHandler.DisableMvcResponseHeader = true;
			ViewEngines.Engines.Clear();
			ViewEngines.Engines.Add(new RazorViewEngine());
		}
	}
}
