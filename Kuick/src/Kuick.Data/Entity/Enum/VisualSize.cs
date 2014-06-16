// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// VisualSize.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-15 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Serializable]
	[DefaultValue(VisualSize.Medium)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum VisualSize
	{
		Mini,
		Small,
		Medium,
		Large,
		XLarge,
		XXLarge
	}
}
