// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.ComponentModel;

namespace Kuicker
{
	[DefaultValue(KernelPosition.Stopped)]
	public enum KernelPosition
	{
		Stopped,

		BeforeStart,         // ─────────────┐
		BeforeBuiltinStart,  // ──┐          │
		BuiltinStart,        //   │ Builtin  │
		AfterBuiltinStart,   // ──┘          │ Start
		BeforePluginStart,   // ──┐          │
		PluginStart,         //   │ Plugin   │
		AfterPluginStart,    // ──┘          │
		AfterStart,          // ├────────────┘

		Running,

		BeforeStop,          // ─────────────┐
		PluginStop,          // Plugin       │ Stop
		BuiltinStop,         // Builtin      │
		AfterStop,           // ─────────────┘
	}
}
