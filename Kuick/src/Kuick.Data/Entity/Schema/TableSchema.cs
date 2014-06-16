// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// TableSchema.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	[Serializable]
	public class TableSchema
	{
		public TableSchema()
		{
			this.Indexes = new List<IndexSchema>();
			this.Columns = new List<ColumnSchema>();
		}

		#region property
		public string TableName { get; set; }

		public List<IndexSchema> Indexes { get; set; }
		public List<ColumnSchema> Columns { get; set; }
		#endregion
	}
}
