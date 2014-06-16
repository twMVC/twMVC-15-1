// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// AuthenticationMode.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Kuick
{
	[DefaultValue(AuthenticationMode.Database)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum AuthenticationMode
	{
		ActiveDirectory,
		Database
	}
}
