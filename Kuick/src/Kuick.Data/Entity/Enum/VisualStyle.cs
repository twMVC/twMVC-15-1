// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// VisualStyle.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Serializable]
	[DefaultValue(VisualStyle.Normal)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum VisualStyle
	{
		Normal,
		Highlight,
		Slight,
		Alert
	}
}
