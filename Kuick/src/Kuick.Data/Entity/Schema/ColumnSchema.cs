// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ColumnSchema.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	[Serializable]
	public class ColumnSchema
	{
		public ColumnSchema() { }

		#region property
		public string FieldName { get; set; }
		public SqlDataType DBType { get; set; }
		public bool AllowNull { get; set; }
		public bool PrimaryKey { get; set; }
		public int Length { get; set; }
		#endregion
	}
}
