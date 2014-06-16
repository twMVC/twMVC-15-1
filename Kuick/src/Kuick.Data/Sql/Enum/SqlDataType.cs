// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlDataType.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick.Data
{
	[Serializable]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum SqlDataType
	{
		Unknown,
		Bit,
		Boolean,
		//Byte, //
		//Short, //
		Char,
		Long,
		//Float, //
		Decimal,
		Double,
		Integer,
		Int64,
		Enum,
		MaxVarBinary,
		MaxVarChar,
		MaxVarWChar,
		TimeStamp,
		VarChar,
		VarWChar,
		WChar,
		Uuid,
		Identity
	}
}
