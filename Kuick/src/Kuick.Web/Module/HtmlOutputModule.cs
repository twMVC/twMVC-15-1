// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HtmlOutputModule.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web;

namespace Kuick.Web
{
	public class HtmlOutputModule : ModuleBase
	{
		#region IHttpModule
		public override void Init(HttpApplication context)
		{
			base.Init(context);

			// register event handler
			context.ReleaseRequestState += new EventHandler(this.ReleaseRequestState);
		}
		#endregion

		#region Event Handler
		public void ReleaseRequestState(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			HtmlOutputFilter filter = new HtmlOutputFilter(app.Response.Filter);
			filter.App = app;
			app.Response.Filter = filter;
		}
		#endregion

	}
}
