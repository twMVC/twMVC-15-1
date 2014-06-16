// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ApiMode.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-07-20 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Web
{
	[Serializable]
	[DefaultValue(ApiMode.Normal)]
	[XmlType(Namespace = WebConstants.Xml.Namespace)]
	public enum ApiMode
	{
		Normal,
		Develop,
		Document
	}
}
