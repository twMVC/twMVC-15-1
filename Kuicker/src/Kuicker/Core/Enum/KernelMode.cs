// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.ComponentModel;

namespace Kuicker
{
	[DefaultValue(KernelMode.Released)]
	public enum KernelMode
	{
		Released,  // R, r
		Testing,   // T, t
		Debugging, // D, d
	}
}
