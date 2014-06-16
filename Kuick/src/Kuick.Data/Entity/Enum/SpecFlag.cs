// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SpecFlag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Kuick.Data
{
	[Flags]
	[Serializable]
	[DefaultValue(SpecFlag.None)]
	[XmlType(Namespace = DataConstants.Xml.Namespace)]
	public enum SpecFlag
	{
		None = 0,
		NotAllowNull = 1,
		ReadOnly = 2,
		Identity = 8,
		PrimaryKey = 16 | NotAllowNull | ReadOnly,
	}
}
