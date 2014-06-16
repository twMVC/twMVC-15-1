// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// RequestType.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick.Web
{
	/// <summary>
	/// Indicate where request parameter value from.
	/// </summary>
	[Serializable]
	[XmlType(Namespace = WebConstants.Xml.Namespace)]
	public enum RequestType
	{
		/// <summary>
		/// Query string, value from url, GET
		/// </summary>
		Query,
		/// <summary>
		/// Form, value from HTTP Body, POST
		/// </summary>
		Form,

		/// <summary>
		/// Cookie, value from HTTP Header
		/// </summary>
		Cookies,

		/// <summary>
		/// any of it, maybe you sh
		/// </summary>
		Any
	}
}