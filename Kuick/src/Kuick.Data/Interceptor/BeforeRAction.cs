// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BeforeRAction.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	internal class BeforeRAction : GroupBase
	{
		internal BeforeRAction(string group, Action<string, Sql> action) 
		{
			this.Group = group;
			this.Action = action;
		}

		internal Action<string, Sql> Action { get; private set; }
	}
}
