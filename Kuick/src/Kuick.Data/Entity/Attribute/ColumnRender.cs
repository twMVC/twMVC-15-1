// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ColumnRender.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-08-25 - Creation


using System;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ColumnRender: Attribute, ICloneable<ColumnRender>
	{
		#region constructor
		public ColumnRender(Type type)
		{
			this.Type = type;
		}
		#endregion

		#region ICloneable<T>
		public ColumnRender Clone()
		{
			ColumnRender clone = new ColumnRender(Type);
			clone.Type = Type;
			return clone;
		}
		#endregion

		#region property
		public Type Type { get; internal set; }
		public IEntity Schema 
		{
			get
			{
				IEntity schema = EntityCache.GetFirst(Type);
				if(null == schema) {
					throw new ArgumentException("Only IEntity type allowded!");
				}
				return schema;
			}
		}

		public bool HasType 
		{ 
			get 
			{
				return null != Type;
			} 
		}

		// Parent
		public Column Column { get; internal set; }
		#endregion
	}
}
