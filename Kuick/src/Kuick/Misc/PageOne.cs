// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// PageOne.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public class PageOne
	{
		public PageOne()
		{
		}

		public PageOne(int pageIndex, bool active)
		{
			this.PageIndex = pageIndex;
			this.Active = active;
		}

		public int PageIndex { get; set; }
		public bool Active { get; set; }
	}
}
