// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IRender.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-08-25 - Creation


using System;

namespace Kuick.Data
{
	public interface IRender
	{
		string ToString(object value);
		string ToHtml(object value);
	}
}
