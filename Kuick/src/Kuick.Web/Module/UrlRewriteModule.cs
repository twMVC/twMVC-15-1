// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// UrlRewriteModule.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web;

namespace Kuick.Web
{
	/// <summary>
	/// URL rewrite, Friendly URL
	/// 1. file names use hyphens (-)
	/// 2. Use lower case and try to address case sensitivity issues
	/// 3. Add guessable entry point URLs, e.g. http://www.xyz.com/jobs
	/// 4. 
	/// </summary>
	public class UrlRewriteModule : ModuleBase
	{
		#region IHttpModule
		public new void Init(HttpApplication context)
		{
			base.Init(context);

			// register event handler
		}

		public new void Dispose()
		{
			base.Dispose();
		}
		#endregion

		#region proiperty
		#endregion

		#region event handler
		private void OnXxx(object sender, EventArgs e)
		{
		}
		#endregion
	}
}
