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
	public class NameCache<T>
		: List<T> 
		where T : IName
	{
		private object _Lock = new object();

		#region constructor
		public NameCache()
		{
		}
		#endregion

		#region property
		#endregion

		#region method
		public List<T> FindAll(string name)
		{
			List<T> list = this.Filter(x => x.Name == name);
			return list;
		}
		public T FindFirst(string name)
		{
			T one = this.FilterFirst(x => x.Name == name);
			return one;
		}
		#endregion
	}
}
