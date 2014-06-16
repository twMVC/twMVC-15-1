// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DataFormat.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick
{
	[Serializable]
	[DefaultValue(DataFormat.Unknown)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum DataFormat
	{
		Unknown,
		String,
		Char,
		Boolean,
		DateTime,
		Integer,
		Decimal,
		Double, //
		Byte,
		Short,
		Long,
		Float,
		Enum,
		Object,
		Objects,
		ByteArray, //
		Color,
		Guid
	}
}
