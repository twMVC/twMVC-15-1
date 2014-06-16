// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlSetValueFormat.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick.Data
{
	[Serializable]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum SqlSetValueFormat
	{
		String,
		Integer,
		Decimal,
		Long,
		Float,
		Boolean,
		Char,
		Byte,
		ByteArray,
		DateTime,
		Color,
		Guid,
		Column,
		Sql
	}
}
