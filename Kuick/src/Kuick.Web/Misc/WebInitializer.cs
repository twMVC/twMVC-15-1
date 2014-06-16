// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WebInitializer.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;

namespace Kuick.Web
{
	public sealed class WebInitializer : Initializer
	{
		protected override void OnInit()
		{
			using(IntervalLogger il = new IntervalLogger("WebInitializer")) {
				WebConfig.Initialize();
			}
		}
	}
}
