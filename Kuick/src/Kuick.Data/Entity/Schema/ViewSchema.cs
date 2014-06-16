// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ViewSchema.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	[Serializable]
	public class ViewSchema
	{
		public ViewSchema()
		{
			this.Columns = new List<ColumnSchema>();
		}

		#region property
		public string ViewName { get; set; }

		public List<ColumnSchema> Columns { get; set; }
		#endregion
	}
}
