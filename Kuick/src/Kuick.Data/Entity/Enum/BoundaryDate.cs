// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BoundaryDate.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Serializable]
	[DefaultValue(BoundaryDate.None)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum BoundaryDate
	{
		None,
		Between,
		Besides,
		GreaterThan,
		SmallerThan
	}
}
