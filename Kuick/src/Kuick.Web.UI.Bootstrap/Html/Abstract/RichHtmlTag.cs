// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// RichHtmlTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Kuick.Web.UI.Bootstrap
{
	public abstract class RichHtmlTag : HtmlTag
	{
		public RichHtmlTag()
			: base()
		{
			this.Class = new List<string>();
			this.Style = new List<string>();
		}

		[Attr]
		public string Id { get; set; }

		[Attr]
		public List<string> Class { get; set; }

		[Attr]
		public List<string> Style { get; set; }

		[Attr]
		public int Tabindex { get; set; }

		public override void Render(StringBuilder sb)
		{
			if(null == sb) {
				throw new ArgumentNullException("StringBuilder");
			}
			Builder = sb;

			sb.AppendFormat(
				"<{0}",
				ParseTagName()
			);

			foreach(Attr attr in Attrs) {
				sb.Append(" ");
				RenderAttr(attr);
			}

			if(WithCloseTag) {
				sb.Append(">");
				RenderCloseTag();
			} else {
				if(NeedClose) {
					sb.Append("/>");
				} else {
					sb.Append(">");
				}
			}
		}
	}
}
