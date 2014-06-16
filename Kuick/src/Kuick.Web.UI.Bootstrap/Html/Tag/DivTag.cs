// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DivTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class DivTag : RichContainerHtmlTag, IDataHtmlTag, IAriaHtmlTag
	{
		public DivTag()
			: base()
		{
		}

		[Attr]
		public string Role { get; set; }

		#region IDataHtmlTag
		[Attr]
		public string DataToggle { get; set; }

		[Attr]
		public string DataTarget { get; set; }

		[Attr]
		public string DataDismiss { get; set; }

		[Attr]
		public string DataParent { get; set; }
		#endregion

		#region IAriaHtmlTag Members
		[Attr]
		public string AriaLabelledBy { get; set; }

		[Attr]
		public bool AriaHidden { get; set; }
		#endregion
	}
}
