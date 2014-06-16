// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DbSettingCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System.Configuration;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class DbSettingCache : StaticCache<DbSetting>
	{
		internal static Dictionary<string, string> _Versions = 
			new Dictionary<string, string>();
		internal static Dictionary<string, List<string>> _TableNames = 
			new Dictionary<string, List<string>>();

		public static void Clear()
		{
			lock(_Lock) {
				_Cache.Clear();
			}
		}

		public static void Add(DbSetting setting)
		{
			if(null == setting) { return; }

			lock(_Lock) {
				if(!_Cache.ContainsKey(setting.Name)) {
					_Cache.Add(setting.Name.ToLower(), setting);
				}
			}
		}

		public new static DbSetting Get(string key)
		{
			DbSetting setting;
			if(TryGet(key.ToLower(), out setting)) {
				return setting;
			}
			if(TryGet(DataConstants.Default.Name.ToLower(), out setting)) {
				return setting;
			}
			if(Count == 1) {
				foreach(DbSetting x in Values) {
					return x;
				}
			}

			throw new ConfigurationErrorsException(string.Format(
				"Cannot find the DbSetting of {0}",
				key
			));
		}

	}
}
