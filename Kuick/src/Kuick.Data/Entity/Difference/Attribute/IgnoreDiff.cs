// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IgnoreDiff.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-09-15 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class IgnoreDiff : Attribute
	{
	}
}
