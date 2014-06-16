// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// GroupBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public abstract class GroupBase : IGroup
	{
		#region IGroup
		public virtual string Group { get; set; }
		#endregion
	}
}
