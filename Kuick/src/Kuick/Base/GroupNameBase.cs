// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// GroupNameBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public abstract class GroupNameBase : IGroupName
	{
		#region IGroupName
		public virtual string Group { get; set; }
		public virtual string Name { get; set; }
		#endregion
	}
}
