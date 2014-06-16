// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// TextareaTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class TextareaTag : RichContainerHtmlTag
	{
		public TextareaTag()
			: base()
		{
			this.Rows = 3;
		}

		[Attr]
		public int Rows { get; set; }
	}
}
