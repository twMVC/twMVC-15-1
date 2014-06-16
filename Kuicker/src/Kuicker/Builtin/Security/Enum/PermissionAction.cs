// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.ComponentModel;

namespace Kuicker
{
	[DefaultValue(LogLevel.Info)]
	public enum PermissionAction
	{
		Non = 0,
		Read = 1,
		Write = Read | 2,
		Add = Read | 4,
		Delete = Read | 8,
		Export = Read | 16,
		Execute = Read | 32,
		Admin = Read | Write | Add | Delete | Export | Execute
	}
}
