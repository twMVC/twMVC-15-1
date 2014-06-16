// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// OperationType.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick
{
	[Serializable]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum OperationType
	{
		/// <summary>
		/// An operation section.
		/// </summary>
		Execute,

		/// <summary>
		/// A web page or a winwdows form.
		/// </summary>
		Page,

		/// <summary>
		/// Refers to a series of related operation.
		/// </summary>
		Group,

		/// <summary>
		/// Refers to a particular Entity only.
		/// </summary>
		Entity
	}
}
