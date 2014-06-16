// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EnumItem.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-11-20 - Creation


using System;

namespace Kuick
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
		public EnumItem(): base() { }

		#region property
		public new T Value { get; set; }
		#endregion
	}
}
