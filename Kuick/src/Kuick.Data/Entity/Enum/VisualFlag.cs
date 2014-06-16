// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// VisualFlag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Flags]
	[DefaultValue(VisualFlag.Normal)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum VisualFlag
	{
		Normal = 0,
		ShowBulkEdit = 1,
		HideInAdd = 2,
		HideInModify = 4,
		HideInList = 8,
		SystemColumn = HideInAdd | HideInModify | HideInList
	}
}
