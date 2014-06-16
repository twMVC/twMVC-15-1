// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SelectTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class SelectTag : RichContainerHtmlTag
	{
		public SelectTag()
			: base()
		{
		}

		[Attr]
		public bool Multiple { get; set; }
	}
}
