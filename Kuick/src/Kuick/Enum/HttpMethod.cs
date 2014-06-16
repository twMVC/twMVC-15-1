// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HttpMethod.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-09-28 - Creation


using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Kuick
{
	[Serializable]
	[DefaultValue(HttpMethod.Get)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum HttpMethod
	{
		Get,
		Head,
		Post,
		Put,
		Delete,
		Trace,
		Options
	}
}
