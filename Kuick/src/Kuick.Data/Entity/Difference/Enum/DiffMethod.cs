// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DiffMethod.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-09-16 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Serializable]
	[Flags]
	[DefaultValue(DiffMethod.Add | DiffMethod.Modify | DiffMethod.Remove)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum DiffMethod
	{
		[Description("Add")]
		Add = 0,

		[Description("Modify")]
		Modify = 1,

		[Description("Remove")]
		Remove = 2
	}
}
