// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// AfterRAction.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	internal class AfterRAction<T> 
		: GroupBase
		where T : IEntity
	{
		internal AfterRAction(string group, Action<string, Sql, T> action) 
		{
			this.Group = group;
			this.Action = action;
		}

		internal Action<string, Sql, T> Action { get; private set; }
	}
}
