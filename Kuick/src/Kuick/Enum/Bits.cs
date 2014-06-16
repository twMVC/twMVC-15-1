// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Bits.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick
{
	[Serializable]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum Bits
	{
		x86,
		x64
	}
}
