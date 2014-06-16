// Kfsyscc
// Copyright (c) Kfsyscc. All rights reserved.
//
// BrowserType.cs
//
// Modified By      YYYY-MM-DD
// kevinjong        2013-06-10 - Creation


using System;
using System.ComponentModel;

namespace Kuick.Builtin.Security
{
	[DefaultValue(BrowserType.Others)]
	public enum BrowserType
	{
		Chrome,
		IE,
		Mobile,
		Safari,
		Others
	}
}
