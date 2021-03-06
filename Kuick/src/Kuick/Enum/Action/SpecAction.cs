﻿// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SpecAction.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Kuick
{
	[DefaultValue(SpecAction.Preset)]
	[XmlType(Namespace = Constants.Xml.Namespace)]
	public enum SpecAction
	{
		Preset,
		Select,
		Insert,
		Update,
		Delete
	}
}
