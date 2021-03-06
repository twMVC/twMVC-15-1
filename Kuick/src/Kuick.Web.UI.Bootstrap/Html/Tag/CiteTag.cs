﻿// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// CiteTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class CiteTag : RichContainerHtmlTag
	{
		public CiteTag()
			: base()
		{
		}

		[Attr]
		public string Title { get; set; }
	}
}
