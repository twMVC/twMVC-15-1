// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ConfigService.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-09-23 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Builtin.Config
{
	public class ConfigService : BuiltinBase, IConfig
	{
		#region IBuiltin
		public override bool Default
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region IConfig
		public bool Exists(string appID, string category, string path, string name)
		{
			return ConfigEntity.Exists(x =>
				x.AppID == appID 
				& 
				x.Category == category 
				& 
				x.Path == path 
				& 
				x.Name == name
			);
		}

		public string Read(string appID, string category, string path, string name)
		{
			ConfigEntity config = ConfigEntity.QueryFirst(x =>
				x.AppID == appID &
				x.Category == category &
				x.Path == path &
				x.Name == name
			);
			return null == config 
				? string.Empty 
				: null == config.Value 
					? string.Empty 
					: config.Value;
		}

		public Result Write(
			string appID, 
			string category, 
			string path, 
			string name, 
			string value)
		{
			ConfigEntity config = ConfigEntity.QueryFirst(x =>
				x.AppID == appID &
				x.Category == category &
				x.Path == path &
				x.Name == name
			);
			if(null == config) {
				return new ConfigEntity() {
					AppID = appID,
					Category = category,
					Path = path,
					Name = name,
					Value = value
				}.Add();
			} else {
				config.Value = value;
				return config.Modify();
			}
		}

		public Result Clear(string appID, string category, string path, string name)
		{
			return ConfigEntity.Remove(x =>
				x.AppID == appID &
				x.Category == category &
				x.Path == path &
				x.Name == name
			);
		}

		public bool GlobalExists(string category, string path, string name)
		{
			return Exists(Constants.Global.AppID, category, path, name);
		}

		public string GlobalRead(string category, string path, string name)
		{
			return Read(Constants.Global.AppID, category, path, name);
		}

		public Result GlobalWrite(string category, string path, string name, string value)
		{
			return Write(Constants.Global.AppID, category, path, name, value);
		}

		public Result GlobalClear(string category, string path, string name)
		{
			return Clear(Constants.Global.AppID, category, path, name);
		}

		public List<string> AppIDs()
		{
			return ConfigEntity
				.Sql()
				.Distinct(x => x.AppID)
				.DistinctQuery<string>();
		}

		public List<string> Categories(string appID)
		{
			return ConfigEntity
				.Sql()
				.Distinct(x => x.Category)
				.Where(x => x.AppID == appID)
				.DistinctQuery<string>();
		}

		public List<string> Paths(string appID, string category)
		{
			return ConfigEntity
				.Sql()
				.Distinct(x => x.Path)
				.Where(x => 
					x.AppID == appID & 
					x.Category == category)
				.DistinctQuery<string>();
		}

		public List<string> Names(string appID, string category, string path)
		{
			return ConfigEntity
				.Sql()
				.Distinct(x => x.Name)
				.Where(x => 
					x.AppID == appID & 
					x.Category == category & 
					x.Path == path)
				.DistinctQuery<string>();
		}
		#endregion
	}
}
