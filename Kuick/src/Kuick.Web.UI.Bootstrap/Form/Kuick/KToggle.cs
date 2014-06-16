// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// KToggle.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-10 - Creation


using System;
using System.Text;
using Kuick.Data;

namespace Kuick.Web.UI.Bootstrap
{
	public class KToggle : BToggle
	{
		public KToggle(Column column)
			: this(column, null)
		{
		}
		public KToggle(Column column, IEntity instance)
			: base()
		{
			this.Column = column;
			base.Title = column.Description.Description;
			object value = instance.GetValue(column);
		}

		public Column Column { get; set; }
	}
}
