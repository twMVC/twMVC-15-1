// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;

namespace Kuicker
{
	public class EnumItem
	{
		public EnumItem() { }

		#region property
		public Type EnumType { get; set; }
		public string Title { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
		#endregion

		public Any ToAny()
		{
			return new Any(Value, Title);
		}
	}

	public class EnumItem<T> : EnumItem
	{
		public EnumItem() : base() { }

		#region property
		public new T Value { get; set; }
		#endregion
	}
}
