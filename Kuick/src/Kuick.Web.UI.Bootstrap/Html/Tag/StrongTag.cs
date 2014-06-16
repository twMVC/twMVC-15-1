// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// StrongTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;

namespace Kuick.Web.UI.Bootstrap
{
	public class StrongTag : ContainerHtmlTag
	{
		public StrongTag()
			: base()
		{
		}

		public new bool WithCloseTag { get { return true; } }
	}
}
