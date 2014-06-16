// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// GroupNameCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public class GroupNameCache<T>
		: List<T>
		where T : IGroupName
	{
		private object _Lock = new object();

		#region constructor
		public GroupNameCache()
		{
		}
		#endregion

		#region property
		#endregion

		#region method
		public List<T> FindAllByName(string name)
		{
			List<T> list = this.Filter(x => 
				x.Name.ToLower() == name.ToLower()
			);
			return list;
		}
		public List<T> FindAll(string group)
		{
			List<T> list = this.Filter(x => 
				x.Group.ToLower() == group.ToLower()
			);
			return list;
		}
		public List<T> FindAll(string group, string name)
		{
			List<T> list = this.Filter(x => 
				x.Group.ToLower() == group.ToLower() & 
				x.Name.ToLower() == name.ToLower()
			);
			return list;
		}
		public T FindFirst(string group, string name)
		{
			T one = this.FilterFirst(x => 
				x.Group.ToLower() == group.ToLower() & 
				x.Name.ToLower() == name.ToLower()
			);
			return one;
		}
		#endregion
	}
}
