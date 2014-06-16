// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DbCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	public class DbCache : TypeCache
	{
		public static bool Add(string key, Type value)
		{
			if(!_Cache.ContainsKey(key.ToLower())) {
				lock(_Lock) {
					if(!_Cache.ContainsKey(key.ToLower())) {
						_Cache.Add(key.ToLower(), value);
						return true;
					}
				}
			}
			return false;
		}
	}
}
