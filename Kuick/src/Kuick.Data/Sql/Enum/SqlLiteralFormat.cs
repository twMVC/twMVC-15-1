// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlLiteralFormat.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Serializable]
	[DefaultValue(SqlLiteralFormat.Command)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum SqlLiteralFormat
	{
		Command,
		DistinctDate
	}
}
