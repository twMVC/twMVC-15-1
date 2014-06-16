// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ATag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class ATag : RichContainerHtmlTag, IDataHtmlTag
	{
		public ATag()
			: base()
		{
		}

		[Attr]
		public string Href { get; set; }

		[Attr]
		public int TabIndex { get; set; }

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

		[Attr]
		public string DataSlide { get; set; }

		[Attr]
		public string DataProvide { get; set; }
	}
}
