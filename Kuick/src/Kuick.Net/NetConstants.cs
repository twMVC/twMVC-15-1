// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NetConstants.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-09-29 - Creation


using System;

namespace Kuick.Net
{
	public class NetConstants : Kuick.Constants
	{
		public class Net
		{
			public static readonly string UserAgent = string.Format(
				"Kuick/1.0 ({0})", Current.OSVersion
			);
		}
	}
}
