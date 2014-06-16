// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// RowStatus.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-26 - Creation


using System;
using System.ComponentModel;

namespace Kuick.Web.UI.Bootstrap
{
	[DefaultValue(RowStatus.Default)]
	public enum RowStatus
	{
		Default,
		Success,
		Error,
		Warning,
		Info
	}
}
