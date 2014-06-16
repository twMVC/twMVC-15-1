// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NameCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public class KeyCache<T> 
		: Dictionary<string, T> 
		where T : IKey
	{
		private object _Lock = new object();

		#region constructor
		public KeyCache()
		{
		}
		#endregion

		#region property
		#endregion

		#region method
		public bool Add(T item)
		{
			if(!ContainsKey(item.Key)) {
				lock(_Lock) {
					if(!ContainsKey(item.Key)) {
						Add(item.Key, item);
						return true;
					}
				}
			}
			return false;
		}
		#endregion
	}
}
