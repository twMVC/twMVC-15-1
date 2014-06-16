// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HttpVerbs.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-07-20 - Creation


using System;

namespace Kuick.Web
{
	[Flags]
	public enum HttpVerbs
	{
		All = HttpVerbs.Get | HttpVerbs.Post | HttpVerbs.Put | HttpVerbs.Delete | HttpVerbs.Head,
		Get = 1,
		Post = 2,
		Put = 4,
		Delete = 8,
		Head = 16
	}
}
