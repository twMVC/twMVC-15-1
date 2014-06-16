// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Pointer.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public class Pointer
	{
		public Pointer()
		{
			this.Present = 0;
			this.Peak = 0;
		}

		public int Present { get; set; }
		public int Peak { get; set; }
	}
}
