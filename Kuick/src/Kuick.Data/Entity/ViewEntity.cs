// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ViewEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	public class ViewEntity<T>
		: Entity<T>
		where T : ViewEntity<T>, new()
	{
		public override bool IsView
		{
			get
			{
				return true;
			}
		}
	}
}
