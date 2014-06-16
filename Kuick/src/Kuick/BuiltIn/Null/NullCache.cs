// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NullCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public class NullCache : NullBuiltin, ICache
	{
		private readonly string CATEGORY = typeof(NullCache).Name;

		public Result Update(string group, string name)
		{
			return Builtins.Config.GlobalWrite(CATEGORY, group, name, Formator.ToString17());
		}

		public bool NeedUpdate(string group, string name, DateTime cacheDateTime)
		{
			string value = Builtins.Config.GlobalRead(CATEGORY, group, name);
			DateTime date = value.AirBagToDateTime(Constants.Null.Date);
			return date > cacheDateTime;
		}

		public List<string> Groups()
		{
			return Builtins.Config.Paths(Constants.Global.AppID, CATEGORY);
		}

		public List<string> Names(string group)
		{
			return Builtins.Config.Names(Constants.Global.AppID, CATEGORY, group);
		}
	}
}
