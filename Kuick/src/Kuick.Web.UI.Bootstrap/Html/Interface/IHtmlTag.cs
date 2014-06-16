// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IHtmlTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-05 - Creation


using System;
using System.Collections.Generic;
using System.Text;

namespace Kuick.Web.UI.Bootstrap
{
	public interface IHtmlTag
	{
		TagName TagName { get; }
		bool WithCloseTag { get; }
		bool NeedClose { get; }
		List<Attr> Attrs { get; }
		Attr GetAttr(string propertyName);
		object GetAttrValue(string propertyName);
		object GetAttrValue(Attr attr);
		void Render(StringBuilder sb);
	}
}
