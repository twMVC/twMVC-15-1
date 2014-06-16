// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ISchemaNamingConvention.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Reflection;

namespace Kuick.Data
{
	public interface ISchemaNamingConvention
	{
		string ToTableName(Type type);
		string ToColumnName(Type type, PropertyInfo info);
	}
}
