// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ThumbnailWay.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Xml.Serialization;

namespace Kuick.Web
{
	[Serializable]
	[XmlType(Namespace = WebConstants.Xml.Namespace)]
	public enum ThumbnailWay
	{
		/// <summary>
		/// Falsify Shape
		/// </summary>
		FalsifyShape = 0,

		/// <summary>
		/// Cut Over Side
		/// </summary>
		CutOverSide,

		/// <summary>
		/// Empty Lack Side
		/// </summary>
		EmptyLackSide
	}
}
