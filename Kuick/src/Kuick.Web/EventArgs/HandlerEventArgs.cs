// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HandlerEventArgs.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web;

namespace Kuick.Web
{
	public class HandlerEventArgs : EventArgs
	{
		public HandlerEventArgs(HttpContext context)
		{
			this.Context = context;
		}

		public HttpContext Context { get; private set; }
	}
}
