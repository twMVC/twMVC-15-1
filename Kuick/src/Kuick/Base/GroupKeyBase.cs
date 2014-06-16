// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// GroupKeyBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public abstract class GroupKeyBase : IGroupKey
	{
		#region constructor
		public GroupKeyBase() 
		{
		}
		#endregion

		#region IGroupKey
		public virtual string Group { get; set; }
		public virtual string Key { get; set; }

		public bool Equals(string group, string key)
		{
			return Group == group && Key == key;
		}
		#endregion

		#region IEquatable<IGroupKey>
		public bool Equals(IGroupKey other)
		{
			return Equals(other.Group, other.Key);
		}
		#endregion
	}
}
