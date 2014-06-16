// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EntityMapping.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class EntityMapping : Attribute, ICloneable<EntityMapping>
	{
		#region constructor
		public EntityMapping(params Type[] members)
		{
			if(Checker.IsNull(members)) {
				throw new ArgumentNullException("members");
			}

			this.Members = members;
		}
		#endregion

		#region ICloneable<T>
		public EntityMapping Clone()
		{
			return new EntityMapping(Members);
		}
		#endregion

		#region property
		public Type[] Members { get; internal set; }

		// Parent
		public Table Table { get; internal set; }
		#endregion
	}
}
