// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Config.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Configuration;

namespace Kuick
{
	internal class Config
	{
		#region field
		private static object _Lock = new object();
		private static Config _Singleton;
		#endregion

		#region constructor
		internal Config() 
		{
			this.Kernel = new ConfigSection<ConfigSetting>();
			this.Database = new ConfigSection<ConfigDatabase>();
			this.Application = new ConfigSection<ConfigSetting>();
		}
		#endregion

		#region static
		internal static Config Singleton
		{
			get
			{
				if(null == _Singleton) {
					lock(_Lock) {
						if(null == _Singleton) {
							_Singleton = (Config)ConfigurationManager.GetSection("Kuick");
							if(null == _Singleton) { _Singleton = new Config(); }
						}
					}
				}
				return _Singleton;
			}	
		}
		#endregion

		#region property
		internal ConfigSection<ConfigSetting> Kernel { get; private set; }
		internal ConfigSection<ConfigDatabase> Database { get; private set; }
		internal ConfigSection<ConfigSetting> Application { get; private set; }
		#endregion

		#region inner constants class
		internal class Xml
		{
			internal class Tag
			{
				internal const string Kernel = "kernel";
				internal const string Database = "database";
				internal const string Application = "application";
				internal const string Add = "add";
			}

			internal class Attribute
			{
				internal const string Group = "group";
				internal const string Name = "name";
				internal const string Value = "value";
			}
		}
		#endregion
	}
}
