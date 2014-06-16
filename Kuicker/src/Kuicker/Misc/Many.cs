// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;

namespace Kuicker
{
	public class Many : Any
	{
		public Many()
			: this(string.Empty)
		{
		}
		public Many(string group)
			: this(group, string.Empty)
		{
		}
		public Many(string group, string name)
			: this(group, name, string.Empty)
		{
		}
		public Many(string group, string name, params object[] vals)
			: base(name, vals)
		{
			if(group.IsNullOrEmpty()) { group = "Default"; }
			this.Group = group;
		}

		public string Group { get; set; }

		#region ICloneable
		public new object Clone()
		{
			Many clone = new Many();

			clone.Group = this.Group;
			clone.Name = this.Name;
			clone.Value = this.Value;
			clone.Values = this.Values;

			return clone;
		}
		#endregion

		public Any ToAny()
		{
			Any any = this as Any;
			if(Group.IsNullOrEmpty() || Name.IsNullOrEmpty()) {
				any.Name = string.Concat(Group.AirBag(), Name.AirBag());
			} else {
				any.Name = string.Concat(Group, ".", Name);
			}
			return any;
		}
	}
}
