// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Kuicker
{
	public class Config
	{
		#region inner class
		public class Xml
		{
			public const string Kuicker = "Kuicker";
			public const string Kernel = "Kernel";
			public const string Builtin = "Builtin";
			public const string Plugin = "Plugin";
			public const string Application = "Application";
		}
		#endregion

		#region field
		private static object _Lock = new object();
		private static Config _Current;
		#endregion

		#region constructor
		internal Config()
		{
			this.KernelSection = new List<Any>();
			this.BuiltinSection = new List<Many>();
			this.PluginSection = new List<Many>();
			this.ApplicationSection = new List<Many>();
		}
		#endregion

		#region static
		internal static Config Current
		{
			get
			{
				if(null == _Current) {
					lock(_Lock) {
						if(null == _Current) {
							_Current = ConfigurationManager.GetSection(
								Xml.Kuicker
							) as Config;
							if(null == _Current) {
								_Current = new Config();
								//Logger.Error("Config setting error!");
							}
						}
					}
				}
				return _Current;
			}
		}
		#endregion

		#region property
		public List<Any> KernelSection { get; internal set; }
		public List<Many> BuiltinSection { get; internal set; }
		public List<Many> PluginSection { get; internal set; }
		public List<Many> ApplicationSection { get; internal set; }
		#endregion

		#region Config
		public class Kernel
		{
			private static string _AppID;
			public static string AppID
			{
				get
				{
					if(null == _AppID) {
						_AppID = Current.KernelSection.ToString("AppID");
					}
					return _AppID;
				}
			}

			public static bool Debug
			{
				get
				{
					return Current.KernelSection.ToBoolean("Debug");
				}
			}

			private static string[] _SkipAssemblyPrefixes;
			public static string[] SkipAssemblyPrefixes
			{
				get
				{
					if(null == _SkipAssemblyPrefixes) {
						_SkipAssemblyPrefixes = Current
							.KernelSection
							.ToStrings("SkipAssemblyPrefixes");
					}
					return _SkipAssemblyPrefixes;
				}
			}

			private static string[] _OnlyAssemblyPrefixes;
			public static string[] OnlyAssemblyPrefixes
			{
				get
				{
					if(null == _OnlyAssemblyPrefixes) {
						_OnlyAssemblyPrefixes = Current
							.KernelSection
							.ToStrings("OnlyAssemblyPrefixes");
					}
					return _OnlyAssemblyPrefixes;
				}
			}
		}
		#endregion

		//#region Log
		//public class Log
		//{
		//	public static bool Enable
		//	{
		//		get
		//		{
		//			return Current.LogSection.ToBoolean("Enable", true);
		//		}
		//	}

		//	public static bool Enable
		//	{
		//		get
		//		{
		//			return Current.LogSection.ToBoolean("Enable", true);
		//		}
		//	}

		//	//public static string FolderName
		//	//{
		//	//	get
		//	//	{
		//	//		return Current.LogSection.ToString("FolderName", "log");
		//	//	}
		//	//}

		//	//public static bool SeperateFolder
		//	//{
		//	//	get
		//	//	{
		//	//		return Current.LogSection.ToBoolean("SeperateFolder", false);
		//	//	}
		//	//}

		//	//public static bool CompactMode
		//	//{
		//	//	get
		//	//	{
		//	//		return Current.LogSection.ToBoolean("CompactMode", false);
		//	//	}
		//	//}

		//	///// <summary>
		//	///// Log file split critical size (in KB)
		//	///// </summary>
		//	//public static int RefreshSize
		//	//{
		//	//	get
		//	//	{
		//	//		return Current.LogSection.ToInteger("RefreshSize", 1000);
		//	//	}
		//	//}
		//}
		//#endregion

		//#region Database
		//public class Database
		//{
		//	public static DbConfig Get(string name)
		//	{
		//		return Current.DatabaseSection.FirstOrDefault(x =>
		//			x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
		//		);
		//	}

		//	public static IEnumerable<DbConfig> ByProvider(string provider)
		//	{
		//		return Current.DatabaseSection.Where(x =>
		//			x.Provider.Equals(
		//				provider,
		//				StringComparison.OrdinalIgnoreCase
		//			)
		//		);
		//	}
		//}
		//#endregion
	}
}
