// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// DeviceType.cs
//
// Modified By      YYYY-MM-DD
// kevinjong        2013-06-10 - Creation


using System;
using System.ComponentModel;

namespace Kuick.Builtin.Security
{
	[DefaultValue(DeviceType.Others)]
	public enum DeviceType
	{
		iPod,
		iPhone,
		iPad,
		iOS,
		Mobile,
		Windows,
		NonWindows,
		Others
	}
}
