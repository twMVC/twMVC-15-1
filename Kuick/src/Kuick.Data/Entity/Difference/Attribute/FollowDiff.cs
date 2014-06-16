// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// FollowDiff.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-09-15 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class FollowDiff : Attribute
	{
		public FollowDiff()
			: this(DiffMethod.Add | DiffMethod.Modify | DiffMethod.Remove)
		{
		}
		public FollowDiff(DiffMethod method)
		{
			this.Add = Checker.Flag.Check((int)method, (int)DiffMethod.Add);
			this.Modify = Checker.Flag.Check((int)method, (int)DiffMethod.Modify);
			this.Remove = Checker.Flag.Check((int)method, (int)DiffMethod.Remove);
		}

		public bool Add { get; set; }
		public bool Modify { get; set; }
		public bool Remove { get; set; }
	}
}
