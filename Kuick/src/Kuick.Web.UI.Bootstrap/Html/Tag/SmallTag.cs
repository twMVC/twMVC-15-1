// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SmallTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class SmallTag : ContainerHtmlTag
	{
		public SmallTag()
			: base()
		{
		}

		public new bool WithCloseTag { get { return true; } }
	}
}
