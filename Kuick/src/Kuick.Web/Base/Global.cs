// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Global.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web;

namespace Kuick.Web
{
	public class Global : HttpApplication
	{
		public virtual void Application_Start(Object sender, EventArgs e)
		{
			Platform.Start();
		}

		public virtual void Application_End(Object sender, EventArgs e)
		{
			Platform.Terminate();
		}

		protected void Session_Start(Object sender, EventArgs e)
		{
			Platform.Start();
		}
	}
}
