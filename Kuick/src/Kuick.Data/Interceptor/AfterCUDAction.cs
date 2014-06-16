// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// AfterCUDAction.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	internal class AfterCUDAction<T> 
		: GroupBase
		where T : IEntity
	{
		internal AfterCUDAction(string group, Action<string, T, Result> action) 
		{
			this.Group = group;
			this.Action = action;
		}

		internal Action<string, T, Result> Action { get; private set; }
	}
}
