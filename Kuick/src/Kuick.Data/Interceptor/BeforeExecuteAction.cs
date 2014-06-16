// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BeforeExecuteAction.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;

namespace Kuick.Data
{
	internal class BeforeExecuteAction
		: GroupBase
	{
		internal BeforeExecuteAction(string group, Action<string, string, IDbDataParameter[]> action) 
		{
			this.Group = group;
			this.Action = action;
		}

		internal Action<string, string, IDbDataParameter[]> Action { get; private set; }
	}
}
