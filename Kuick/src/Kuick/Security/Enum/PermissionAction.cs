// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// PermissionAction.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation


using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Kuick
{
	[Flags]
	[Serializable]
	[DefaultValue(PermissionAction.Non)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum PermissionAction
	{
		Non = 0,
		Read = 1,
		Write = Read | 2,
		Add = Read | 4,
		Delete = Read | 8,
		Export = Read | 16,
		Execute = Read | 32,
		Admin = Read | Write | Add | Delete | Export | Execute
	}
}
