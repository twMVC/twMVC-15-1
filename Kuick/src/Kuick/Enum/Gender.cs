// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Gender.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-08-28 - Creation


using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Kuick
{
	[Serializable]
	[DefaultValue(Gender.Unknown)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum Gender
	{
		[Description("Unknown")]
		Unknown = 0,

		[Description("Male")]
		Male = 1,

		[Description("Female")]
		Female = 2
	}
}
