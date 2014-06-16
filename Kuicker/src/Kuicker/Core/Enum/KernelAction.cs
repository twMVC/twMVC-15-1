// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.ComponentModel;

namespace Kuicker
{
	[DefaultValue(KernelAction.Shutdown)]
	public enum KernelAction
	{
		Shutdown,
		Restart,
	}
}
