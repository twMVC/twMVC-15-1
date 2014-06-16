// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ICache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	/// <summary>
	/// One cache for all applications.
	/// </summary>
	public interface ICache : IBuiltin
	{
		Result Update(string group, string name);
		bool NeedUpdate(string group, string name, DateTime cacheDateTime);

		List<string> Groups();
		List<string> Names(string group);
	}
}
