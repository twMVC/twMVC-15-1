// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// LabelTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class LabelTag : RichContainerHtmlTag
	{
		public LabelTag()
			: base()
		{
		}

		[Attr]
		public string For { get; set; }
	}
}
