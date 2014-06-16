// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// KeyBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public abstract class KeyBase : IKey
	{
		#region IKey
		public virtual string Key { get; set; }

		public bool Equals(string key)
		{
			return Key == key;
		}

		#endregion

		#region IEquatable<IKey>
		public bool Equals(IKey other)
		{
			return Equals(other.Key);
		}
		#endregion
	}
}
