// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ColumnSerializable.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-02-19 - Creation


using System;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ColumnSerializable : Attribute, ICloneable<ColumnSerializable>
	{
		#region constructor
		public ColumnSerializable()
		{
		}
		#endregion

		#region ICloneable<T>
		public ColumnSerializable Clone()
		{
			ColumnSerializable clone = new ColumnSerializable();
			return clone;
		}
		#endregion

		#region property
		#endregion
	}
}
