// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// GroupValues.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-05-08 - Creation


using System;

namespace Kuick
{
	public class GroupValues : GroupBase
	{
		public GroupValues()
		{
			this.Values = new Anys();
		}

		public Anys Values { get; set; }
	}
}
