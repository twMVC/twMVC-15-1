// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlMode.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-07-22 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Serializable]
	[DefaultValue(SqlMode.Batch)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum SqlMode
	{
		[Description("Batch Mode")]
		Batch,

		[Description("Iteration Mode")]
		Iteration
	}
}
