// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// WebControlBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Kuick.Web
{
	public abstract class WebControlBase : WebControl
	{
		public new PageBase Page
		{
			get
			{
				return (PageBase)base.Page;
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			RenderContents(writer);
		}
	}
}