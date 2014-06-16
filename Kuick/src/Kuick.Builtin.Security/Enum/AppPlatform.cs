// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// AppPlatform.cs
//
// Modified By      YYYY-MM-DD
// kevinjong        2013-06-10 - Creation


using System;
using System.ComponentModel;

namespace Kuick.Builtin.Security
{
	[DefaultValue(AppPlatform.Web)]
	public enum AppPlatform
	{
		Web,

		Windows
	}
}
