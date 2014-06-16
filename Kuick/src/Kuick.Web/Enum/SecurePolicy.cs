// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SecurePolicy.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-07-20 - Creation


using System;

namespace Kuick.Web
{
	[Flags]
	public enum SecurePolicy
	{
		Any,
		Http,
		Https
	}
}
