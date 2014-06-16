// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// GroupCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public class GroupCache<T> 
		: List<T> 
		where T : IGroup
	{
		private object _Lock = new object();

		#region constructor
		public GroupCache()
		{
		}
		#endregion

		#region property
		#endregion

		#region method
		public List<T> FindAll(string group)
		{
			List<T> list = this.Filter(x => x.Group == group);
			return list;
		}
		#endregion
	}
}
