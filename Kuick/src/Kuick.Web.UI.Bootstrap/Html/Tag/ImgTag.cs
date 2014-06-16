// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ImgTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class ImgTag : RichHtmlTag
	{
		public ImgTag()
			: base()
		{
		}

		public override bool NeedClose { get { return false; } }

		[Attr]
		public string Src { get; set; }

		[Attr]
		public string Alt { get; set; }

		[Attr]
		public string DataSrc { get; set; }
	}
}
