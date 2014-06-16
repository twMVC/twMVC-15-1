// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ColumnDataFormat.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Serializable]
	[DefaultValue(ColumnDataFormat.String)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum ColumnDataFormat
	{
		Undefined,
		String,
		Integer,
		Int64,
		Decimal,
		Long,
		Short,
		Double,
		Float,
		Boolean,
		Char,
		Enum,
		Byte,
		ByteArray,
		DateTime,
		Color,
		Guid
	}
}
