// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BoundaryInteger.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Serializable]
	[DefaultValue(BoundaryInteger.None)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum BoundaryInteger
	{
		None,
		Between,
		Besides,
		GreaterThan,
		SmallerThan,
		In,
		Exclude,
		IsEven,
		IsOdd
	}
}
