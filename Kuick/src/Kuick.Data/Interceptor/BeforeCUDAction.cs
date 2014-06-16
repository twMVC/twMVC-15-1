// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BeforeCUDAction.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	internal class BeforeCUDAction<T> 
		: GroupBase
		where T : IEntity
	{
		internal BeforeCUDAction(string group, Action<string, T> action) 
		{
			this.Group = group;
			this.Action = action;
		}

		internal Action<string, T> Action { get; private set; }
	}
}
