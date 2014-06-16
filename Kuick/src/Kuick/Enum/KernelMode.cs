// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// KernelMode.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick
{
	[Serializable]
	[DefaultValue(KernelMode.Released)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum KernelMode
	{
		Released,
		Testing,
		Developing
	}
}
