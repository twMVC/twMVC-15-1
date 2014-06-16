// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NullConfig.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System.Collections.Generic;

namespace Kuick
{
	public class NullConfig : NullBuiltin, IConfig
	{
		public bool Exists(string appID, string category, string path, string name)
		{
			DefaultSetting(appID, category, path);
			if(string.IsNullOrEmpty(name)) { return false; }

			return true;
		}

		public string Read(string appID, string category, string path, string name)
		{
			DefaultSetting(appID, category, path);
			if(string.IsNullOrEmpty(name)) { return string.Empty; }

			return string.Empty;
		}

		public Result Write(string appID, string category, string path, string name, string value)
		{
			DefaultSetting(appID, category, path);
			if(string.IsNullOrEmpty(name)) { return Result.BuildFailure<__KernelError_ConfigName>(); }

			return Result.BuildSuccess();
		}

		public Result Clear(string appID, string category, string path, string name)
		{
			DefaultSetting(appID, category, path);
			if(string.IsNullOrEmpty(name)) { return Result.BuildFailure<__KernelError_ConfigName>(); }

			return Result.BuildSuccess();
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
			return new List<string>();
		}

		public List<string> Categories(string appID)
		{
			return new List<string>();
		}

		public List<string> Paths(string appID, string category)
		{
			return new List<string>();
		}

		public List<string> Names(string appID, string category, string path)
		{
			return new List<string>();
		}

		// private
		private void DefaultSetting(string appID, string category, string path)
		{
			if(string.IsNullOrEmpty(appID)) { appID = Current.AppID; }
			if(string.IsNullOrEmpty(category)) { category = Constants.Default.Category; }
			if(string.IsNullOrEmpty(path)) { path = Constants.Default.Path; }
		}
	}
}
