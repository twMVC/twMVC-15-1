// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.ComponentModel;

namespace Kuicker
{
	[DefaultValue(DataFormat.Unknown)]
	public enum DataFormat
	{
		Unknown,
		String,
		Char,
		Boolean,
		DateTime,
		Integer,
		Decimal,
		Double,
		Byte,
		Short,
		Long,
		Float,
		Enum,
		Object,
		Objects,
		ByteArray,
		Color,
		Guid,
	}
}
