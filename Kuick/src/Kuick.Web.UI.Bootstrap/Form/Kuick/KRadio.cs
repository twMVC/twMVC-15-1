// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// KRadio.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-10 - Creation


using System;
using System.Text;
using Kuick.Data;

namespace Kuick.Web.UI.Bootstrap
{
	public class KRadio: BRadio
	{
		public KRadio(Column column)
			: this(column, null)
		{
		}
		public KRadio(Column column, IEntity instance)
			: base()
		{
			this.Column = column;
			object value = instance.GetValue(column);
			EnumReference er = EnumCache.Get(Column.Property.PropertyType);
			foreach(EnumItem item in er.Items) {
				base.AddItem(item.Name);
			}
		}

		public Column Column { get; set; }
	}
}
