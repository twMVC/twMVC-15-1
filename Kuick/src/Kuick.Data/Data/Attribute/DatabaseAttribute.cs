// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DatabaseAttribute.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class DatabaseAttribute : Attribute
	{
		public DatabaseAttribute(string vender) 
		{
			this.Vender = vender;
		}

		public string Vender { get; private set; }
	}
}
