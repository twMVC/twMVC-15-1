// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SecureAttribute.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;

namespace Kuick.Web
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class SecureAttribute : Attribute
	{
		public SecureAttribute()
		{
		}
		public SecureAttribute(string host)
		{
			if(Uri.CheckHostName(host) == UriHostNameType.Unknown) {
				throw new ArgumentException(
					String.Format("\"{0}\" is not a valid host", host)
				);
			}
			Host = host;
		}
		public string Host { get; private set; }
	}
}
