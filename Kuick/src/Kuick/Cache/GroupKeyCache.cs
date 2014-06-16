// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// GroupKeyCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public class GroupKeyCache<T>
		: List<T> 
		where T : IGroupKey
	{
		private object _Lock = new object();

		#region constructor
		public GroupKeyCache()
		{
		}
		#endregion

		#region property
		#endregion

		#region method
		public T Find(string group, string key)
		{
			T item = this.FilterFirst(x => x.Group == group && x.Key == key);
			return item;
		}
		public List<T> FindAll(string group)
		{
			List<T> list = this.Filter(x => x.Group == group);
			return list;
		}
		#endregion
	}
}
