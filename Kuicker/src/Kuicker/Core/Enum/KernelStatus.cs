// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.ComponentModel;

namespace Kuicker
{
	[DefaultValue(KernelStatus.Stopped)]
	public enum KernelStatus
	{
		Stopped,
		Starting,
		Running,
		Stopping,
	}
}
