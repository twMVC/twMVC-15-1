// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ResponseType.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Kuick
{
	[Serializable]
	[DefaultValue(ResponseType.Xml)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum ResponseType
	{
		Xml,
		Json
	}
}
