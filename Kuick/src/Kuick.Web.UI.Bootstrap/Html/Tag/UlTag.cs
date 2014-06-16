// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// UlTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class UlTag : RichContainerHtmlTag, IAriaHtmlTag
	{
		public UlTag()
			: base()
		{
		}

		[Attr]
		public string Role { get; set; }

		#region IAriaHtmlTag
		[Attr]
		public string AriaLabelledBy { get; set; }

		[Attr]
		public bool AriaHidden { get; set; }
		#endregion
	}
}
