// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EditMode.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-23 - Creation


using System;
using System.ComponentModel;

namespace Kuick.Web.UI
{
	[DefaultValue(EditMode.Add)]
	public enum EditMode
	{
		Add,
		Edit,
		Remove,
		Clone,
		Preview,
		Move,
		Sort
	}
}
