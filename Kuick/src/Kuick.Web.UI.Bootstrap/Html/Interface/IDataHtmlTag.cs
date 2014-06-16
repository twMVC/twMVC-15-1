// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IDataHtmlTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public interface IDataHtmlTag : IHtmlTag
	{
		string DataToggle { get; set; }
		string DataTarget { get; set; }
		string DataDismiss { get; set; }
		string DataParent { get; set; }
	}
}
