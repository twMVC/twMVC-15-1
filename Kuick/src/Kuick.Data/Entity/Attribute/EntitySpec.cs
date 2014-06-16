// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EntitySpec.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class EntitySpec : Attribute, ICloneable<EntitySpec>
	{
		#region constructor
		public EntitySpec()
		{
		}
		#endregion

		#region ICloneable<T>
		public EntitySpec Clone()
		{
			return new EntitySpec();
		}
		#endregion

		#region property

		// Parent
		public Table Table { get; internal set; }
		#endregion
	}
}
