// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ResizeWay.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick.Web
{
	[Serializable]
	[XmlType(Namespace = WebConstants.Xml.Namespace)]
	public enum ResizeWay
	{
		/// <summary>
		/// Falsify Shape
		/// </summary>
		FalsifyShape = 0,

		/// <summary>
		/// Cut Over Side
		/// </summary>
		CutOverSide = 1,

		/// <summary>
		/// Empty Lack Side
		/// </summary>
		EmptyLackSide = 2,

		/// <summary>
		/// ResizedFitIn
		/// </summary>
		ResizedFitIn = 3
	}
}
