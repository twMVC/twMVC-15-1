// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IAriaHtmlTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public interface IAriaHtmlTag : IHtmlTag
	{
		string AriaLabelledBy { get; set; }
		bool AriaHidden { get; set; }
	}
}
