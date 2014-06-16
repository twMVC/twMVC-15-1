// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// LiTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class LiTag : RichContainerHtmlTag
	{
		public LiTag()
			: base()
		{
		}

		[Attr]
		public string DataTarget { get; set; }

		[Attr]
		public int DataSlideTo { get; set; }
	}
}
