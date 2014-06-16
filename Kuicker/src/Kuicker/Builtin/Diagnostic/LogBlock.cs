// LogBlock.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Generic;
using log4net.Core;

namespace Kuicker
{
	public class LogBlock
	{
		public LogBlock()
		{
			this.Anys = new Any[0];
		}

		public string ComputerName { get; set; }
		public string FileName { get; set; }
		public string AppID { get; set; }
		public DateTime TimeStamp { get; set; }
		public long Index { get; set; }
		public Level Level { get; set; }
		public string Message { get; set; }
		public Any[] Anys { get; set; }
	}
}
