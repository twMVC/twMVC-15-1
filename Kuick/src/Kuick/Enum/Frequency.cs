// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Frequency.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Kuick
{
	[Serializable]
	[DefaultValue(Frequency.Once)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum Frequency
	{
		Once,
		Annual,
		Monthly,
		Weekly,
		Daily,
		Hourly,
		Minutely
	}
}
