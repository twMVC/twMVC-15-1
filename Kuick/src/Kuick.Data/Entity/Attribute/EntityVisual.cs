// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EntityVisual.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class EntityVisual : Attribute, ICloneable<EntityVisual>
	{
		#region constructor
		public EntityVisual()
			: this(true, DataCurrent.Data.PageSize)
		{
		}
		public EntityVisual(bool visible)
			: this(visible, DataCurrent.Data.PageSize)
		{
		}
		public EntityVisual(int pageSize)
			: this(true, pageSize)
		{
		}
		public EntityVisual(bool visible, int pageSize)
		{
			this.Visible = visible;
			this.PageSize = pageSize;
		}
		#endregion

		#region ICloneable<T>
		public EntityVisual Clone()
		{
			return new EntityVisual(Visible, PageSize);
		}
		#endregion

		#region property
		public bool Visible { get; internal set; }
		public int PageSize { get; internal set; }

		// Parent
		public Table Table { get; internal set; }
		#endregion
	}
}
