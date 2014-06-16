// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IconSize.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-23 - Creation


using System;
using System.ComponentModel;

namespace Kuick.Web.UI
{
	[DefaultValue(IconSize._16x16)]
	public enum IconSize
	{
		[Description("小圖")]
		_16x16,

		[Description("中圖")]
		_24x24,

		[Description("大圖")]
		_64x64,
	}
}
