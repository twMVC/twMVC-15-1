// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Generic;
using System.IO;

namespace Kuicker
{
	public sealed class KernelLifeCycle : LifeCycle, IKernelLifeCycle
	{
		public override void DoBeforeStart()
		{
			//Logger.Message(
			//	"RunTime",
			//	new Any("UserDomainName", RunTime.UserDomainName),
			//	new Any("UserName", RunTime.UserName),
			//	new Any("OperatingSystemBits", RunTime.OperatingSystemBits),
			//	new Any("ProcessorBits", RunTime.ProcessorBits),
			//	new Any("OSVersion", Environment.OSVersion.VersionString),
			//	new Any("IsWebApp", RunTime.IsWebApp),
			//	new Any("BinFolder", RunTime.BinFolder)
			//);

			//Logger.Message(
			//	"Kernel",
			//	new Any("AppID", Config.Kernel.AppID),
			//	new Any("Debug", Config.Kernel.Debug),
			//	new Any(
			//		"SkipAssemblyPrefixes", 
			//		Config.Kernel.SkipAssemblyPrefixes.Join()
			//	),
			//	new Any(
			//		"OnlyAssemblyPrefixes", 
			//		Config.Kernel.OnlyAssemblyPrefixes.Join()
			//	)
			//);

			//Logger.Message(
			//	"Log",
			//	new Any("Enable", Config.Log.Enable),
			//	new Any("Folder", Config.Log.Folder),
			//	new Any("RefreshSize", Config.Log.RefreshSize)
			//);

			//Logger.Message(
			//	"Builtin",
			//	new Any("Log.Enable", Config.Log.Enable),
			//	new Any("Log.Folder", Config.Log.Folder),
			//	new Any("Log.RefreshSize", Config.Log.RefreshSize)
			//);

			//Logger.Message(
			//	"Database",
			//	Config.Current.DatabaseSection.ToAnys()
			//);
			//Logger.Message(
			//	"Plugin",
			//	Config.Current.PluginSection.ToAnys()
			//);
			//Logger.Message(
			//	"Application",
			//	Config.Current.BuiltinSection.ToAnys()
			//);

		}

		public void DoBuiltinStart()
		{
		}

		public void DoPluginStart()
		{
		}

		public void DoPluginStop()
		{
		}

		public void DoBuiltinStop()
		{
		}
	}
}
