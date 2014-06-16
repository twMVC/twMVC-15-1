// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Encryption.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Kuick
{
	[Serializable]
	[DefaultValue(Encryption.Symmetry)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum Encryption
	{
		/// <summary>
		/// Reversible
		/// </summary>
		Symmetry,

		/// <summary>
		/// Irreversible
		/// </summary>
		Asymmetry
	}
}
