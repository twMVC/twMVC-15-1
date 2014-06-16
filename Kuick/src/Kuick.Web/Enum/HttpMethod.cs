// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HttpMethod.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick.Web
{
	[Serializable]
	[XmlType(Namespace = WebConstants.Xml.Namespace)]
	public enum HttpMethod
	{
		OPTIONS,
		GET,
		HEAD,
		POST,
		PUT,
		DELETE,
		TRACE,
		CONNECT
	}
}
