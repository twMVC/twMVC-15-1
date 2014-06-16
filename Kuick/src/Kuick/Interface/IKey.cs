// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IKey.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public interface IKey : IEquatable<IKey>
	{
		string Key {get; set;}

		bool Equals(string key);
	}
}
