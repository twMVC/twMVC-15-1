// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// CompressAttribute.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;

namespace Kuick.Web
{

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class CompressAttribute : Attribute, ICloneable<CompressAttribute>
	{
		#region constructor
		public CompressAttribute()
		{
		}
		#endregion

		#region ICloneable<T>
		public CompressAttribute Clone()
		{
			return new CompressAttribute();
		}
		#endregion

		public override string ToString()
		{
			return "Compress response stream.";
		}
	}
}
