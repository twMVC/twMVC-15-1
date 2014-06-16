// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// CacheDurationEntityNameAttribute.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Reflection;

namespace Kuick.Web
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class CacheDurationEntityNameAttribute : Attribute
	{
		public CacheDurationEntityNameAttribute() { }

		public PropertyInfo PropertyInfo { get; set; }
	}
}
