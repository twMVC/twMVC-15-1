// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IGroupKey.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick
{
	public interface IGroupKey : IEquatable<IGroupKey>
	{
		string Group { get; set; }
		string Key { get; set; }

		bool Equals(string group, string key);
	}
}
