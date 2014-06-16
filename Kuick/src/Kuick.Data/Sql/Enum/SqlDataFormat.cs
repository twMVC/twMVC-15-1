// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlDataFormat.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick.Data
{
	[Serializable]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum SqlDataFormat
	{
		String,
		Integer,
		Decimal,
		Long,
		Float,
		Boolean,
		Char,
		Byte,
		DateTime,
		StringArray,
		IntegerArray,
		EntityArray,
		Color,
		Column,
		Sql
	}
}
