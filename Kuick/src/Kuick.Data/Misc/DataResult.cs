// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DataResult.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-09-26 - Creation


using System;

namespace Kuick.Data
{
	public class DataResult : Result<DataResult>
	{
		#region Constructor
		public DataResult()
			: this(true)
		{
		}

		public DataResult(bool success)
			: this(success, string.Empty)
		{
		}

		public DataResult(bool success, string message)
			: base(success, message)
		{
			this.EntityName = string.Empty;
			this.KeyValue = string.Empty;
			this.SqlCommand = string.Empty;
		}
		#endregion


		#region property
		public int AffectedCount { get; set; }
		public string EntityName { get; set; }
		public string KeyValue { get; set; }
		public string SqlCommand { get; set; }
		#endregion
	}
}
