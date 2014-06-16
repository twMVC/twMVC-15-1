// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IndexSchema.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	[Serializable]
	public class IndexSchema
	{
		public IndexSchema() 
		{
			this.ColumnNames = new List<string>();
		}

		#region property
		public string IndexName { get; set; }
		public bool Unique { get; set; }
		public string TableName { get; set; }
		public List<string> ColumnNames { get; set; }
		#endregion
	}
}
