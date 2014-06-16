// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// LogBlock.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-10-09 - Creation


using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Kuick
{
	public class LogBlock
	{
		public LogBlock()
		{
			this.Datas = new Anys();
		}

		public string Uuid { get; set; }
		public long Index { get; set; }
		public DateTime TimeStamp { get; set; }
		public LogLevel Level { get; set; }
		public string Title { get; set; }
		public string Message { get; set; }
		public string Detail { get; set; }
		public Anys Datas { get; set; }
		public Exception Exception { get; set; }
	}
}
