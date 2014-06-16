// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BackEndModule.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web;

namespace Kuick.Web
{
	public class BackEndModule : ModuleBase
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

	}
}
