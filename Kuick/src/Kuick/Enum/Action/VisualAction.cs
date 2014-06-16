// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// VisualAction.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Kuick
{
	[DefaultValue(VisualAction.Preset)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum VisualAction
	{
		Preset,
		Read,
		Write,
		Edit,
		Clear
	}
}
