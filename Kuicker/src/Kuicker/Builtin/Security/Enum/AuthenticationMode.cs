// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.ComponentModel;

namespace Kuicker
{
	[DefaultValue(LogLevel.Info)]
	public enum AuthenticationMode
	{
		ActiveDirectory,
		Database
	}
}
