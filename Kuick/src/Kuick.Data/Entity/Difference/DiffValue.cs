// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DiffValue.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-09-14 - Creation


using System;
using System.Runtime.Serialization;

namespace Kuick.Data
{
	public class DiffValue
	{
		public string ColumnName { get; set; }

		public string OriginalValue { get; set; }
		public bool OriginalIsNull { get; set; }

		public string NewValue { get; set; }
		public bool NewIsNull { get; set; }
	}
}
