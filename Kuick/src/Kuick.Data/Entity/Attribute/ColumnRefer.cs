// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ColumnRefer.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class ColumnRefer : Attribute, ICloneable<ColumnRefer>
	{
		#region constructor
		public ColumnRefer(Type type)
		{
			this.Type = type;
			this.Style = ReferValue.None;
		}
		public ColumnRefer(ReferValue style)
		{
			this.Style = style;
		}
		#endregion

		#region ICloneable<T>
		public ColumnRefer Clone()
		{
			ColumnRefer clone = new ColumnRefer(Type);
			clone.Type = Type;
			clone.Style = Style;
			clone.Column = Column;
			return clone;
		}
		#endregion

		#region property
		public Type Type { get; internal set; }
		public ReferValue Style { get; internal set; }
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
