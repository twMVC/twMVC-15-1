// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NameBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public abstract class NameBase : IName
	{
		#region IName
		public virtual string Name { get; set; }
		#endregion
	}
}
